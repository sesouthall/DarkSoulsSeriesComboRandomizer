using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DarkSoulsSeriesComboRandomizer
{
    public partial class MainWindow : Window
    {
        // ── Constants ────────────────────────────────────────────────────────────

        private const string DS1ExeName = "DarkSoulsRemastered.exe";
        private const string DS2ExeName = "DarkSoulsII.exe";
        private const string DS3ExeName = "DarkSoulsIII.exe";

        private static readonly string AppDataFolder =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "DarkSoulsSeriesComboRandomizer");

        // ── Active session state ──────────────────────────────────────────────────

        private ModInstaller? _installer;
        private GameCoordinationServer? _server;

        // ── Construction ─────────────────────────────────────────────────────────

        public MainWindow()
        {
            InitializeComponent();
            InitializeSeed();
            PopulateSaveList();
            UpdatePlayButton();

            CrossGameMappings.Initialize();
        }

        // ── Initialisation helpers ────────────────────────────────────────────────

        private void InitializeSeed()
        {
            var rng = new Random();
            int seed = rng.Next(0, int.MaxValue);
            SeedBox.Text = seed.ToString();
            SaveNameBox.Text = seed.ToString();
        }

        private void PopulateSaveList()
        {
            LoadSaveCombo.Items.Clear();
            LoadSaveCombo.Items.Add(new ComboBoxItem { Content = "— New Run —", IsSelected = true });

            if (!Directory.Exists(AppDataFolder))
            {
                return;
            }

            var saves = Directory.GetDirectories(AppDataFolder)
                                 // If the run didn't get far enough to create a seed file, it's broken and shouldn't be loadable
                                 .Where(path => File.Exists(Path.Combine(path, "seed.txt")))
                                 .Select(Path.GetFileName)
                                 .OrderBy(name => name);

            foreach (var save in saves)
            {
                LoadSaveCombo.Items.Add(new ComboBoxItem { Content = save });
            }
        }

        // ── Validation ────────────────────────────────────────────────────────────

        private bool GamePathsAreValid()
        {
            return (DS1PathBox != null && FileExistsAndMatchesName(DS1PathBox.Text, DS1ExeName))
                && (DS2PathBox != null && FileExistsAndMatchesName(DS2PathBox.Text, DS2ExeName))
                && (DS3PathBox != null && FileExistsAndMatchesName(DS3PathBox.Text, DS3ExeName));
        }

        private static bool FileExistsAndMatchesName(string path, string expectedName)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }
            if (!string.Equals(Path.GetFileName(path), expectedName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return File.Exists(path);
        }

        private void UpdatePlayButton()
        {
            PlayButton.IsEnabled = GamePathsAreValid();

            if (!PlayButton.IsEnabled)
            {
                StatusText.Text = GamePathsAreValid() ? "" :
                    "Please provide valid paths to all three game executables.";
            }
            else
            {
                StatusText.Text = "";
            }
        }

        // ── Browse handlers ───────────────────────────────────────────────────────

        private void BrowseDS1_Click(object sender, RoutedEventArgs e)
            => BrowseForExe(DS1ExeName, DS1PathBox);

        private void BrowseDS2_Click(object sender, RoutedEventArgs e)
            => BrowseForExe(DS2ExeName, DS2PathBox);

        private void BrowseDS3_Click(object sender, RoutedEventArgs e)
            => BrowseForExe(DS3ExeName, DS3PathBox);

        private void BrowseForExe(string exeName, TextBox targetBox)
        {
            var dlg = new OpenFileDialog
            {
                Title = $"Locate {exeName}",
                Filter = $"{exeName}|{exeName}",
                FilterIndex = 1,
                CheckFileExists = true,
                CheckPathExists = true,
            };

            // Try to open at the current directory if it already looks reasonable
            var current = targetBox.Text;
            if (!string.IsNullOrWhiteSpace(current))
            {
                var dir = Path.GetDirectoryName(current);
                if (dir != null && Directory.Exists(dir))
                {
                    dlg.InitialDirectory = dir;
                }
            }

            if (dlg.ShowDialog() == true)
            {
                targetBox.Text = dlg.FileName;
                UpdatePlayButton();
            }
        }

        // ── Text-changed handlers ─────────────────────────────────────────────────

        private void GamePath_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PlayButton != null)
            {
                UpdatePlayButton();
            }
        }

        private void Seed_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Keep save name in sync with seed only while save name hasn't been
            // manually edited to something different from the previous seed value
            // (simple heuristic: sync whenever SaveNameBox is a number).
            if (SaveNameBox != null && int.TryParse(SaveNameBox.Text, out _))
            {
                SaveNameBox.Text = SeedBox.Text;
            }
        }

        // ── Load previous save handler ────────────────────────────────────────────

        private void LoadSave_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LoadSaveCombo.SelectedItem is not ComboBoxItem item)
            {
                return;
            }
            var saveName = item.Content?.ToString();
            if (string.IsNullOrEmpty(saveName))
            {
                return;
            }
            if (saveName == "— New Run —")
            {
                InitializeSeed();
                return;
            }

            SaveNameBox.Text = saveName;

            // Check for a seed file in the save.
            var seedFilePath = Path.Combine(AppDataFolder, saveName, "seed.txt");
            if (File.Exists(seedFilePath))
            {
                var seedString = File.ReadAllText(seedFilePath).Trim();
                SeedBox.Text = seedString;
            }
        }

        // ── Play ──────────────────────────────────────────────────────────────────

        private async void Play_Click(object sender, RoutedEventArgs e)
        {
            var saveName = SaveNameBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(saveName))
            {
                StatusText.Text = "Save name must not be empty.";
                return;
            }

            if (!int.TryParse(SeedBox.Text.Trim(), out int seed))
            {
                StatusText.Text = "Seed must be an integer.";
                return;
            }

            var saveFolder = Path.Combine(AppDataFolder, saveName);
            // If the save folder already exists...
            if (Directory.Exists(saveFolder))
            {
                var seedFile = Path.Combine(saveFolder, "seed.txt");
                // And it has a seed file...
                if (File.Exists(seedFile))
                {
                    var savedSeedString = File.ReadAllText(seedFile);
                    // And the seed file differs from the selected seed...
                    if (int.TryParse(savedSeedString, out int savedSeed) && savedSeed != seed)
                    {
                        // Check if the user meant to overwrite an old save
                        var result = MessageBox.Show("A save with that name already exists. Overwrite?", "Overwrite?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            Directory.Delete(saveFolder, true);
                        }
                        else
                        {
                            return;
                        }
                    }
                    // If the seeds match, just use the old save
                }
                // If the seed file doesn't exist, the save is malformed.
                // Delete it.
                else
                {
                    Directory.Delete(saveFolder, true);
                }
            }
            // If the save folder doesn't exist, Great!, it's a new save

            // Grey everything out
            SetSetupControlsEnabled(false);
            StatusText.Text = "Installing mod files…";

            try
            {
                var options = new RandomizerOptions(seed, saveName);

                var ds1Dir = Path.GetDirectoryName(DS1PathBox.Text)!;
                var ds2Dir = Path.GetDirectoryName(DS2PathBox.Text)!;
                var ds3Dir = Path.GetDirectoryName(DS3PathBox.Text)!;

                _installer = new ModInstaller(ds1Dir, ds2Dir, ds3Dir, options);

                await Task.Run(() =>
                {
                    _installer.InstallStaticChanges();
                });

                Dispatcher.Invoke(() => StatusText.Text = "Creating randomized item placements…");

                await Task.Run(() =>
                {
                    _installer.CreateRandomizedRegulationFilesIfNeeded();
                });

                Dispatcher.Invoke(() => StatusText.Text = "Installing randomized item placements…");

                await Task.Run(() =>
                {
                    _installer.InstallRegulationFiles();
                });

                Dispatcher.Invoke(() => StatusText.Text = "Backing up save files…");

                SuccessOrError? saveBackupResults = null;
                await Task.Run(() =>
                {
                    saveBackupResults = _installer.InstallModSaveFiles();
                });

                if (saveBackupResults == null)
                {
                    MessageBox.Show("Something went wrong while switching to the randomizer's save files. Stopping randomizer.", "Unknown Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    _installer.Dispose();
                    _installer = null;
                    Dispatcher.Invoke(() =>
                    {
                        SetSetupControlsEnabled(true);
                        UpdatePlayButton();
                    });
                    return;
                }
                else if (!saveBackupResults.Succeeded)
                {
                    MessageBox.Show(string.Join("\n\n", saveBackupResults.Errors), "Error with game saves", MessageBoxButton.OK, MessageBoxImage.Error);
                    _installer.Dispose();
                    _installer = null;
                    Dispatcher.Invoke(() =>
                    {
                        SetSetupControlsEnabled(true);
                        UpdatePlayButton();
                    });
                    return;
                }

                Dispatcher.Invoke(() => StatusText.Text = "Starting coordination server…");

                MessageBox.Show("The mod will now launch each game, one at a time.\n" +
"If this is a new save, please create a character in each game. In DS2, proceed through character creation with the Fire Keepers.\n" +
"If this is an existing save, just load the existing characters.\n" +
"Once a character has been loaded, open the start menu. The game will be paused and minimized, and the next will start.\n" +
"Once all three games have loaded characters, DS1 will be resumed for new saves, or the last game you were in for existing saves.", "Start Info", MessageBoxButton.OK, MessageBoxImage.Information);

                _server = new GameCoordinationServer(
                    DS1PathBox.Text,
                    DS2PathBox.Text,
                    DS3PathBox.Text);

                var lastGame = _installer.GetLastGame();
                _server.Start(lastGame);

                // Switch to running panel
                Dispatcher.Invoke(() =>
                {
                    RunSeedDisplay.Text = seed.ToString();
                    RunSaveDisplay.Text = saveName;
                    RunHintDisplay.Text = Path.Combine(_installer.SaveFolderPath, "Hints.txt");
                    StopButton.IsEnabled = true;
                    StopStatusText.Text = "";

                    SetupPanel.Visibility = Visibility.Collapsed;
                    RunningPanel.Visibility = Visibility.Visible;
                });
            }
            catch (Exception ex)
            {
                // Roll back and re-enable UI on failure
                await Dispatcher.Invoke(async () =>
                {
                    StatusText.Text = $"Error: {ex.Message}";
                    MessageBox.Show($"Error: {ex.Message}\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    SetSetupControlsEnabled(true);
                    UpdatePlayButton();

                    // The server has to stop before we can uninstall the mod files
                    if (_server != null)
                    {
                        await _server.Stop();
                    }
                    _server?.Dispose();
                    _server = null;
                    _installer?.Dispose();
                    _installer = null;
                });
            }
        }

        // ── Stop ──────────────────────────────────────────────────────────────────

        private async void Stop_Click(object sender, RoutedEventArgs e)
        {
            StopButton.IsEnabled = false;
            StopStatusText.Text = "Saving progress…";

            try
            {
                _installer?.SaveLastGame(_server?.ActiveGame ?? SoulsGame.DSR);

                StopStatusText.Text = "Stopping coordination server…";

                if (_server != null)
                {
                    await _server.Stop();
                }

                _server?.Dispose();
                _server = null;

                var result = _installer?.RestoreVanillaSaveFiles();

                if (result == null)
                {
                    MessageBox.Show("Something went wrong while switching back to unmodded save files.", "Unknown Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (!result.Succeeded)
                {
                    MessageBox.Show(string.Join("\n\n", result.Errors, "Error restoring unmodded saves", MessageBoxButton.OK, MessageBoxImage.Error));
                }

                _installer?.Dispose();
                _installer = null;

                // Return to setup panel
                Dispatcher.Invoke(() =>
                {
                    RunningPanel.Visibility = Visibility.Collapsed;
                    SetupPanel.Visibility = Visibility.Visible;

                    SetSetupControlsEnabled(true);
                    PopulateSaveList();  // refresh in case a new save was created
                    UpdatePlayButton();
                    StatusText.Text = "";
                });
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    StopStatusText.Text = $"Error while stopping: {ex.Message}";
                    StopButton.IsEnabled = true;   // let user try again
                });
            }
        }

        // ── UI helpers ────────────────────────────────────────────────────────────

        private void SetSetupControlsEnabled(bool enabled)
        {
            DS1PathBox.IsEnabled = enabled;
            DS2PathBox.IsEnabled = enabled;
            DS3PathBox.IsEnabled = enabled;
            SeedBox.IsEnabled = enabled;
            SaveNameBox.IsEnabled = enabled;
            LoadSaveCombo.IsEnabled = enabled;
            PlayButton.IsEnabled = enabled && GamePathsAreValid();

            // Browse buttons – find them by walking the visual tree via Tag or name
            foreach (var btn in FindVisualChildren<Button>(SetupPanel))
            {
                if (btn != PlayButton)
                {
                    btn.IsEnabled = enabled;
                }
            }
        }

        private static IEnumerable<T> FindVisualChildren<T>(
            DependencyObject parent) where T : DependencyObject
        {
            if (parent == null)
            {
                yield break;
            }
            int count = System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                if (child is T t)
                {
                    yield return t;
                }
                foreach (var grandchild in FindVisualChildren<T>(child))
                {
                    yield return grandchild;
                }
            }
        }
    }
}
