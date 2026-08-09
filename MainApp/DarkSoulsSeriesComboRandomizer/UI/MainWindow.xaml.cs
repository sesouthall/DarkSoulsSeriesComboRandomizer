using DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS;
using DarkSoulsSeriesComboRandomizer.DarkSouls3;
using DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered;
using DarkSoulsSeriesComboRandomizer.UI;
using Serilog;
using System.IO;
using System.Windows;

namespace DarkSoulsSeriesComboRandomizer
{
    public partial class MainWindow : Window
    {
        private readonly AppViewModel _vm;
        private readonly SetupPanel _setupPanel;
        private readonly RunningPanel _runningPanel;
        private readonly ErrorPanel _errorPanel;
        private readonly BonfireConnectionsPanel _bonfirePanel;

        public MainWindow()
        {
            InitializeComponent();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(AppViewModel.AppDataFolder, "log.txt"))
                .CreateLogger();

            _vm = new AppViewModel();

            _setupPanel = new SetupPanel(_vm);
            _runningPanel = new RunningPanel(_vm);
            _errorPanel = new ErrorPanel(_vm);
            _bonfirePanel = new BonfireConnectionsPanel(_vm);

            // All panels live in the same Grid cell; Visibility controls which shows.
            RootGrid.Children.Add(_setupPanel);
            RootGrid.Children.Add(_runningPanel);
            RootGrid.Children.Add(_errorPanel);
            RootGrid.Children.Add(_bonfirePanel);

            // Wire cross-panel events
            _setupPanel.PlayRequested += OnPlayRequested;
            _setupPanel.BonfireConnectionsRequested += OnBonfireConnectionsRequested;
            _runningPanel.StopRequested += OnStopRequested;
            _errorPanel.ReturnToSetupRequested += OnReturnToSetupRequested;
            _bonfirePanel.BackToSettingsRequested += OnBackToSettingsRequested;

            ShowPanel(_setupPanel);
        }

        // ── Panel switching ───────────────────────────────────────────────────────

        private void ShowPanel(UIElement panel)
        {
            foreach (UIElement child in RootGrid.Children)
                child.Visibility = child == panel ? Visibility.Visible : Visibility.Collapsed;
        }

        // ── Event handlers ────────────────────────────────────────────────────────

        private void OnBonfireConnectionsRequested(object? sender, EventArgs e)
        {
            _bonfirePanel.Refresh();
            ShowPanel(_bonfirePanel);
        }

        private void OnBackToSettingsRequested(object? sender, EventArgs e)
            => ShowPanel(_setupPanel);

        private void OnReturnToSetupRequested(object? sender, EventArgs e)
        {
            _setupPanel.SetControlsEnabled(true);
            _setupPanel.PopulateSaveList();
            _setupPanel.UpdatePlayButton();
            _setupPanel.SetStatusText("");
            ShowPanel(_setupPanel);
        }

        private async void OnPlayRequested(object? sender, EventArgs e)
        {
            // _vm.Seed and _vm.SaveName were written by SetupPanel.SyncToViewModel()
            if (!int.TryParse(_vm.Seed, out int seed)) return; // already validated in SetupPanel

            var saveName = _vm.SaveName;
            var saveFolder = Path.Combine(AppViewModel.AppDataFolder, saveName);

            if (Directory.Exists(saveFolder))
            {
                var seedFile = Path.Combine(saveFolder, "seed.txt");
                if (File.Exists(seedFile))
                {
                    var savedSeedString = File.ReadAllText(seedFile);
                    if (int.TryParse(savedSeedString, out int savedSeed) && savedSeed != seed)
                    {
                        var result = MessageBox.Show(
                            "A save with that name already exists. Overwrite?",
                            "Overwrite?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                            Directory.Delete(saveFolder, true);
                        else
                            return;
                    }
                }
                else
                {
                    Directory.Delete(saveFolder, true);
                }
            }

            _setupPanel.SetControlsEnabled(false);
            _setupPanel.SetStatusText("Creating randomized item placements...");

            _vm.ErrorDs1Dir = null;
            _vm.ErrorDs2Dir = null;
            _vm.ErrorDs3Dir = null;
            _vm.ErrorRunSaveName = saveName;

            try
            {
                var ds1Dir = Path.GetDirectoryName(_vm.DS1Path)!;
                var ds2Dir = Path.GetDirectoryName(_vm.DS2Path)!;
                var ds3Dir = Path.GetDirectoryName(_vm.DS3Path)!;

                var options = new RandomizerOptions(seed, saveName);
                var gameConfigs = new List<GameInstallSettings>();
                var games = new List<SoulsGameWrapper>();

                if (_vm.DS1Enabled)
                {
                    gameConfigs.Add(new DSRInstallSettings(ds1Dir));
                    games.Add(new DSRWrapper(_vm.DS1Path));
                }
                if (_vm.DS2Enabled)
                {
                    gameConfigs.Add(new DS2SotFSInstallSettings(ds2Dir));
                    games.Add(new DS2SotFSWrapper(_vm.DS2Path));
                }
                if (_vm.DS3Enabled)
                {
                    gameConfigs.Add(new DS3InstallSettings(ds3Dir));
                    games.Add(new DS3Wrapper(_vm.DS3Path));
                }

                _vm.ErrorDs1Dir = ds1Dir;
                _vm.ErrorDs2Dir = ds2Dir;
                _vm.ErrorDs3Dir = ds3Dir;

                var crossGameMappings = CrossGameMappings.New();
                _vm.Installer = ModInstaller.New(gameConfigs, options);

                await Task.Run(() =>
                    _vm.Installer.CreateRandomizedRegulationFilesIfNeeded(
                        crossGameMappings, _vm.BonfireTriples));

                _setupPanel.SaveSettingsToSaveFolder(saveFolder);

                Dispatcher.Invoke(() => _setupPanel.SetStatusText("Installing mod files..."));

                List<string>? installErrors = null;
                await Task.Run(() => installErrors = _vm.Installer.InstallChanges());

                if (installErrors is { Count: > 0 })
                {
                    await Dispatcher.Invoke(async () =>
                    {
                        var revertFailures = await StopServerAndRevert();
                        ShowError(string.Join(Environment.NewLine, [..installErrors, ..revertFailures.Select(ex => $"{ex.Message}\n{ex.StackTrace}")]));
                    });
                    return;
                }

                Dispatcher.Invoke(() => _setupPanel.SetStatusText("Starting coordination server..."));

                MessageBox.Show(
                    "The mod will now launch each game, one at a time.\n" +
                    "If this is a new save, please create a character in each game. In DS2, proceed through character creation with the Fire Keepers.\n" +
                    "If this is an existing save, just load the existing characters.\n" +
                    "Once a character has been loaded, open the start menu. The game will be paused and minimized, and the next will start.\n" +
                    "Once all three games have loaded characters, DS1 will be resumed for new saves, or the last game you were in for existing saves.",
                    "Start Info", MessageBoxButton.OK, MessageBoxImage.Information);

                _vm.Server = new GameCoordinationServer(
                    games,
                    crossGameMappings,
                    (message) => { MessageBox.Show(message, "Randomizer Alert", MessageBoxButton.OK, MessageBoxImage.Warning); },
                    _vm.GameSwitchDelayMs);
                _vm.Server.GameClosed += OnStopRequested;
                await _vm.Server.Start(_vm.Installer.GetLastGame());

                Dispatcher.Invoke(() =>
                {
                    _runningPanel.ShowSession(
                        seed.ToString(), saveName,
                        Path.Combine(_vm.Installer.SaveFolderPath, "Hints.txt"));
                    ShowPanel(_runningPanel);
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while starting the randomizer");
                await Dispatcher.Invoke(async () =>
                {
                    var revertFailures = await StopServerAndRevert();
                    List<Exception> allExceptions = [ex, ..revertFailures];
                    ShowError(string.Join("\n", allExceptions.Select(ex => $"{ex.Message}\n{ex.StackTrace}")));
                });
            }
        }

        private async void OnStopRequested(object? sender, EventArgs e)
        {
            _runningPanel.SetStopButtonEnabled(false);
            _runningPanel.SetStatusText("Saving progress...");

            try
            {
                _vm.Installer?.SaveLastGame(_vm.Server?.ActiveGame.Game ?? SoulsGame.DSR);

                _runningPanel.SetStatusText("Stopping coordination server...");

                if (_vm.Server != null)
                {
                    MessageBox.Show(
                        "The mod will now resume each game, one at a time.\n" +
                        "Please close them as they come up.\n" +
                        "This ensures no data is lost from the save file and avoids the annoying start up notice.",
                        "Shutdown Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    await _vm.Server.Stop();
                }

                _vm.Server?.Dispose();
                _vm.Server = null;
                var revertFailures = _vm.Installer?.RevertChanges() ?? [];
                _vm.Installer = null;

                if (revertFailures.Count == 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        _setupPanel.SetControlsEnabled(true);
                        _setupPanel.PopulateSaveList();
                        _setupPanel.UpdatePlayButton();
                        _setupPanel.SetStatusText("");
                        ShowPanel(_setupPanel);
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        ShowError(string.Join("\n", revertFailures.Select(ex => $"{ex.Message}\n{ex.StackTrace}")));
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while stopping the randomizer.");
                Dispatcher.Invoke(() => ShowError($"{ex.Message}\n{ex.StackTrace}"));
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private async Task<List<Exception>> StopServerAndRevert()
        {
            if (_vm.Server != null) await _vm.Server.Stop();
            _vm.Server?.Dispose();
            _vm.Server = null;
            var revertFailures = _vm.Installer?.RevertChanges() ?? [];
            _vm.Installer = null;
            return revertFailures;
        }

        private void ShowError(string message)
        {
            _errorPanel.ShowError(message);
            ShowPanel(_errorPanel);
        }
    }
}