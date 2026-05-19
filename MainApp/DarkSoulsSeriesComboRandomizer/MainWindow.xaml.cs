using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace DarkSoulsSeriesComboRandomizer
{
    public partial class MainWindow : Window
    {
        // ── Bonfire connections state ─────────────────────────────────────────────

        private static readonly List<BonfireTriple> DefaultBonfireConnections =
        [
            new("Undead Asylum Courtyard",  "Fire Keepers' Dwelling", "Cemetery of Ash"),
            new("Firelink Shrine (DS1)",    "The Far Fire",           "Firelink Shrine (DS3)"),
            new("Darkroot Garden",          "Undead Refuge",          "Road of Sacrifices"),
            new("Anor Londo (DS1)",         "King's Gate",            "Central Irithyll"),
            new("Stone Dragon",             "Dragon Aerie",           "Archdragon Peak"),
            new("Painted World of Ariamis", "Outer Wall",             "Snowfield"),
            new("Oolacile Sanctuary",       "Sanctum Walk",           "The Dreg Heap"),
            new("Prison Tower (DS1)",       "Throne Floor",           "Grand Archives"),
        ];

        // Source of truth for bonfire connections. Always contains all three game
        // fields; "NONE" means no bonfire selected for that game in that row.
        // The panel ComboBoxes are just a view over this list.
        private readonly List<BonfireTriple> _bonfireTriples = [.. DefaultBonfireConnections];

        private static readonly IReadOnlyList<string> AllDS1Bonfires = CollectBonfires(MapData.DS1MapDefinitions);
        private static readonly IReadOnlyList<string> AllDS2Bonfires = CollectBonfires(MapData.DS2MapDefinitions);
        private static readonly IReadOnlyList<string> AllDS3Bonfires = CollectBonfires(MapData.DS3MapDefinitions);

        private static List<string> CollectBonfires(List<FileBackedMapDefinition> maps)
        {
            var result = new List<string>();
            foreach (var map in maps)
            {
                result.AddRange(map.Bonfires);
                foreach (var sub in map.SubMaps)
                    result.AddRange(sub.Bonfires);
            }
            return result;
        }

        private static IReadOnlyList<string> BonfiresForGame(SoulsGame g) => g switch
        {
            SoulsGame.DSR => AllDS1Bonfires,
            SoulsGame.DS2S => AllDS2Bonfires,
            SoulsGame.DS3 => AllDS3Bonfires,
            _ => []
        };

        private static string GameLabel(SoulsGame g) => g switch
        {
            SoulsGame.DSR => "Dark Souls Remastered",
            SoulsGame.DS2S => "Dark Souls II",
            SoulsGame.DS3 => "Dark Souls III",
            _ => ""
        };
        // ── Constants ────────────────────────────────────────────────────────────

        private const string DS1ExeName = "DarkSoulsRemastered.exe";
        private const string DS2ExeName = "DarkSoulsII.exe";
        private const string DS3ExeName = "DarkSoulsIII.exe";

        private static readonly string AppDataFolder =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "DarkSoulsSeriesComboRandomizer");

        private static readonly string SettingsFilePath =
            Path.Combine(AppDataFolder, "settings.json");

        // ── Active session state ──────────────────────────────────────────────────

        private ModInstaller? _installer;
        private GameCoordinationServer? _server;

        // Paths captured at Play time, used by error panel folder buttons
        private string? _errorDs1Dir;
        private string? _errorDs2Dir;
        private string? _errorDs3Dir;
        private string? _errorRunSaveName;

        // ── Construction ─────────────────────────────────────────────────────────

        public MainWindow()
        {
            InitializeComponent();
            LoadSavedPaths();
            InitializeSeed();
            PopulateSaveList();
            UpdatePlayButton();
        }

        // ── Initialisation helpers ────────────────────────────────────────────────

        private void InitializeSeed()
        {
            var rng = new Random();
            int seed = rng.Next(0, int.MaxValue);
            SeedBox.Text = seed.ToString();
            SaveNameBox.Text = seed.ToString();
        }

        // ── Path persistence ──────────────────────────────────────────────────────

        private record PathSettings(
            string DS1Path, string DS2Path, string DS3Path,
            bool DS1Enabled = true, bool DS2Enabled = true, bool DS3Enabled = true);

        private void LoadSavedPaths()
        {
            try
            {
                if (!File.Exists(SettingsFilePath))
                {
                    return;
                }
                var json = File.ReadAllText(SettingsFilePath);
                var settings = JsonSerializer.Deserialize<PathSettings>(json);
                if (settings == null)
                {
                    return;
                }
                if (!string.IsNullOrWhiteSpace(settings.DS1Path))
                {
                    DS1PathBox.Text = settings.DS1Path;
                }
                if (!string.IsNullOrWhiteSpace(settings.DS2Path))
                {
                    DS2PathBox.Text = settings.DS2Path;
                }
                if (!string.IsNullOrWhiteSpace(settings.DS3Path))
                {
                    DS3PathBox.Text = settings.DS3Path;
                }
                DS1EnabledCheckBox.IsChecked = settings.DS1Enabled;
                DS2EnabledCheckBox.IsChecked = settings.DS2Enabled;
                DS3EnabledCheckBox.IsChecked = settings.DS3Enabled;
                ApplyGameEnabledState();
            }
            catch
            {
                // Non-fatal; silently ignore corrupt/missing settings
            }
        }

        private void SavePaths()
        {
            if (DS1PathBox == null || DS2PathBox == null || DS3PathBox == null)
            {
                return;
            }
            try
            {
                Directory.CreateDirectory(AppDataFolder);
                var settings = new PathSettings(
                    DS1PathBox.Text, DS2PathBox.Text, DS3PathBox.Text,
                    DS1EnabledCheckBox.IsChecked == true,
                    DS2EnabledCheckBox.IsChecked == true,
                    DS3EnabledCheckBox.IsChecked == true);
                var json = JsonSerializer.Serialize(settings);
                File.WriteAllText(SettingsFilePath, json);
            }
            catch
            {
                // Non-fatal
            }
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
            bool ds1Enabled = DS1EnabledCheckBox?.IsChecked == true;
            bool ds2Enabled = DS2EnabledCheckBox?.IsChecked == true;
            bool ds3Enabled = DS3EnabledCheckBox?.IsChecked == true;

            // At least one game must be enabled
            if (!ds1Enabled && !ds2Enabled && !ds3Enabled)
            {
                return false;
            }

            if (ds1Enabled && !(DS1PathBox != null && FileExistsAndMatchesName(DS1PathBox.Text, DS1ExeName)))
            {
                return false;
            }
            if (ds2Enabled && !(DS2PathBox != null && FileExistsAndMatchesName(DS2PathBox.Text, DS2ExeName)))
            {
                return false;
            }
            if (ds3Enabled && !(DS3PathBox != null && FileExistsAndMatchesName(DS3PathBox.Text, DS3ExeName)))
            {
                return false;
            }
            return true;
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
            if (PlayButton == null)
            {
                return;
            }

            bool ds1Enabled = DS1EnabledCheckBox?.IsChecked == true;
            bool ds2Enabled = DS2EnabledCheckBox?.IsChecked == true;
            bool ds3Enabled = DS3EnabledCheckBox?.IsChecked == true;

            bool valid = GamePathsAreValid();
            PlayButton.IsEnabled = valid;

            if (!valid)
            {
                if (!ds1Enabled && !ds2Enabled && !ds3Enabled)
                {
                    StatusText.Text = "At least one game must be enabled.";
                }
                else
                {
                    StatusText.Text = "Please provide valid paths to all enabled game executables.";
                }
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
                SavePaths();
            }
        }

        // ── Game enabled checkbox handlers ────────────────────────────────────────

        private void GameEnabled_Changed(object sender, RoutedEventArgs e)
        {
            // When a game is unchecked, clear its column in every triple so that
            // the bonfire is no longer "taken" and re-enabling starts fresh (NONE).
            if (sender == DS1EnabledCheckBox && DS1EnabledCheckBox.IsChecked == false)
                ClearBonfireColumn(SoulsGame.DSR);
            else if (sender == DS2EnabledCheckBox && DS2EnabledCheckBox.IsChecked == false)
                ClearBonfireColumn(SoulsGame.DS2S);
            else if (sender == DS3EnabledCheckBox && DS3EnabledCheckBox.IsChecked == false)
                ClearBonfireColumn(SoulsGame.DS3);

            ApplyGameEnabledState();
            UpdatePlayButton();
            SavePaths();
        }

        private void ClearBonfireColumn(SoulsGame game)
        {
            for (int i = 0; i < _bonfireTriples.Count; i++)
            {
                _bonfireTriples[i] = _bonfireTriples[i].WithBonfireField(game, "NONE");
            }
        }

        private void ApplyGameEnabledState()
        {
            bool ds1Enabled = DS1EnabledCheckBox?.IsChecked == true;
            bool ds2Enabled = DS2EnabledCheckBox?.IsChecked == true;
            bool ds3Enabled = DS3EnabledCheckBox?.IsChecked == true;

            if (DS1PathBox != null) DS1PathBox.IsEnabled = ds1Enabled;
            if (DS1BrowseButton != null) DS1BrowseButton.IsEnabled = ds1Enabled;
            if (DS2PathBox != null) DS2PathBox.IsEnabled = ds2Enabled;
            if (DS2BrowseButton != null) DS2BrowseButton.IsEnabled = ds2Enabled;
            if (DS3PathBox != null) DS3PathBox.IsEnabled = ds3Enabled;
            if (DS3BrowseButton != null) DS3BrowseButton.IsEnabled = ds3Enabled;
        }

        // ── Text-changed handlers ─────────────────────────────────────────────────

        private void GamePath_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PlayButton != null)
            {
                UpdatePlayButton();
                SavePaths();
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
            StatusText.Text = "Creating randomized item placements…";

            // Capture for error panel use
            _errorDs1Dir = null;
            _errorDs2Dir = null;
            _errorDs3Dir = null;
            _errorRunSaveName = saveName;

            try
            {
                var ds1Dir = Path.GetDirectoryName(DS1PathBox.Text)!;
                var ds2Dir = Path.GetDirectoryName(DS2PathBox.Text)!;
                var ds3Dir = Path.GetDirectoryName(DS3PathBox.Text)!;

                var options = new RandomizerOptions(seed, saveName);

                var gameConfigs = new List<GameInstallSettings>();
                if (DS1EnabledCheckBox.IsChecked ?? false)
                {
                    gameConfigs.Add(new DarkSoulsRemastered.DSRInstallSettings(ds1Dir));
                }
                if (DS2EnabledCheckBox.IsChecked ?? false)
                {
                    gameConfigs.Add(new DarkSouls2SotFS.DS2SotFSInstallSettings(ds2Dir));
                }
                if (DS3EnabledCheckBox.IsChecked ?? false)
                {
                    gameConfigs.Add(new DarkSouls3.DS3InstallSettings(ds3Dir));
                }

                _errorDs1Dir = ds1Dir;
                _errorDs2Dir = ds2Dir;
                _errorDs3Dir = ds3Dir;
                var crossGameMappings = CrossGameMappings.New();

                _installer = ModInstaller.New(gameConfigs, options);

                await Task.Run(() =>
                {
                    _installer.CreateRandomizedRegulationFilesIfNeeded(crossGameMappings, _bonfireTriples);
                });

                Dispatcher.Invoke(() => StatusText.Text = "Installing mod files…");

                List<string>? installErrors = null;
                await Task.Run(() =>
                {
                    installErrors = _installer.InstallChanges();
                });

                if (installErrors != null && installErrors.Count != 0)
                {
                    var allErrors = string.Join(Environment.NewLine, installErrors);
                    // Roll back and show error recovery panel
                    await Dispatcher.Invoke(async () =>
                    {
                        // The server has to stop before we can uninstall the mod files
                        if (_server != null)
                        {
                            await _server.Stop();
                        }
                        _server?.Dispose();
                        _server = null;
                        _installer?.RevertChanges();
                        _installer = null;

                        ShowErrorPanel(allErrors);
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
                    DS3PathBox.Text,
                    crossGameMappings);

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
                // Roll back and show error recovery panel
                await Dispatcher.Invoke(async () =>
                {
                    // The server has to stop before we can uninstall the mod files
                    if (_server != null)
                    {
                        await _server.Stop();
                    }
                    _server?.Dispose();
                    _server = null;
                    _installer?.RevertChanges();
                    _installer = null;

                    ShowErrorPanel($"{ex.Message}\n{ex.StackTrace}");
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
                    MessageBox.Show("The mod will now resume each game, one at a time.\n" +
"Please close them as they come up.\n" +
"This ensures no data is lost from the save file and avoids the annoying start up notice.", "Shutdown Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    await _server.Stop();
                }

                _server?.Dispose();
                _server = null;

                _installer?.RevertChanges();
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
                    ShowErrorPanel($"{ex.Message}\n{ex.StackTrace}");
                });
            }
        }

        // ── UI helpers ────────────────────────────────────────────────────────────

        private void SetSetupControlsEnabled(bool enabled)
        {
            DS1EnabledCheckBox.IsEnabled = enabled;
            DS2EnabledCheckBox.IsEnabled = enabled;
            DS3EnabledCheckBox.IsEnabled = enabled;

            // Path boxes and browse buttons respect the checkbox state when re-enabling
            DS1PathBox.IsEnabled = enabled && DS1EnabledCheckBox.IsChecked == true;
            DS1BrowseButton.IsEnabled = enabled && DS1EnabledCheckBox.IsChecked == true;
            DS2PathBox.IsEnabled = enabled && DS2EnabledCheckBox.IsChecked == true;
            DS2BrowseButton.IsEnabled = enabled && DS2EnabledCheckBox.IsChecked == true;
            DS3PathBox.IsEnabled = enabled && DS3EnabledCheckBox.IsChecked == true;
            DS3BrowseButton.IsEnabled = enabled && DS3EnabledCheckBox.IsChecked == true;

            SeedBox.IsEnabled = enabled;
            SaveNameBox.IsEnabled = enabled;
            LoadSaveCombo.IsEnabled = enabled;
            PlayButton.IsEnabled = enabled && GamePathsAreValid();
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

        // ── Error panel ───────────────────────────────────────────────────────────

        private void ShowErrorPanel(string message)
        {
            ErrorMessageText.Text = message;

            // Show / hide folder buttons based on what info we have AND whether the game was enabled
            bool ds1Enabled = DS1EnabledCheckBox?.IsChecked == true;
            bool ds2Enabled = DS2EnabledCheckBox?.IsChecked == true;
            bool ds3Enabled = DS3EnabledCheckBox?.IsChecked == true;

            OpenDS1GameFolderButton.Visibility = ds1Enabled ? Visibility.Visible : Visibility.Collapsed;
            OpenDS2GameFolderButton.Visibility = ds2Enabled ? Visibility.Visible : Visibility.Collapsed;
            OpenDS3GameFolderButton.Visibility = ds3Enabled ? Visibility.Visible : Visibility.Collapsed;

            OpenDS1GameFolderButton.IsEnabled = ds1Enabled && _errorDs1Dir != null && Directory.Exists(_errorDs1Dir);
            OpenDS2GameFolderButton.IsEnabled = ds2Enabled && _errorDs2Dir != null && Directory.Exists(_errorDs2Dir);
            OpenDS3GameFolderButton.IsEnabled = ds3Enabled && _errorDs3Dir != null && Directory.Exists(_errorDs3Dir);

            OpenDS1SaveFolderButton.Visibility = ds1Enabled ? Visibility.Visible : Visibility.Collapsed;
            OpenDS2SaveFolderButton.Visibility = ds2Enabled ? Visibility.Visible : Visibility.Collapsed;
            OpenDS3SaveFolderButton.Visibility = ds3Enabled ? Visibility.Visible : Visibility.Collapsed;

            var runSaveFolder = _errorRunSaveName != null
                ? Path.Combine(AppDataFolder, _errorRunSaveName)
                : null;
            OpenRunSaveFolder.IsEnabled = runSaveFolder != null && Directory.Exists(runSaveFolder);

            SetupPanel.Visibility = Visibility.Collapsed;
            RunningPanel.Visibility = Visibility.Collapsed;
            ErrorPanel.Visibility = Visibility.Visible;
        }

        private void ReturnToSetup_Click(object sender, RoutedEventArgs e)
        {
            ErrorPanel.Visibility = Visibility.Collapsed;
            SetupPanel.Visibility = Visibility.Visible;

            SetSetupControlsEnabled(true);
            PopulateSaveList();
            UpdatePlayButton();
            StatusText.Text = "";

            _errorDs1Dir = null;
            _errorDs2Dir = null;
            _errorDs3Dir = null;
            _errorRunSaveName = null;
        }

        private static void OpenFolder(string? folder)
        {
            if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
            {
                MessageBox.Show($"Folder not found:\n{folder}", "Folder Not Found",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Process.Start("explorer.exe", folder);
        }

        private void OpenDS1GameFolder_Click(object sender, RoutedEventArgs e)
            => OpenFolder(_errorDs1Dir);

        private void OpenDS2GameFolder_Click(object sender, RoutedEventArgs e)
            => OpenFolder(_errorDs2Dir);

        private void OpenDS3GameFolder_Click(object sender, RoutedEventArgs e)
            => OpenFolder(_errorDs3Dir);

        private void OpenDS1SaveFolder_Click(object sender, RoutedEventArgs e)
            => OpenFolder(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "NBGI", "DARK SOULS REMASTERED"));

        private void OpenDS2SaveFolder_Click(object sender, RoutedEventArgs e)
            => OpenFolder(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DarkSoulsII"));

        private void OpenDS3SaveFolder_Click(object sender, RoutedEventArgs e)
            => OpenFolder(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DarkSoulsIII"));

        private void OpenVanillaBackups_Click(object sender, RoutedEventArgs e)
            => OpenFolder(Path.Combine(AppDataFolder, "BackupVanillaFiles"));

        private void OpenRunSaveFolder_Click(object sender, RoutedEventArgs e)
            => OpenFolder(_errorRunSaveName != null
                ? Path.Combine(AppDataFolder, _errorRunSaveName)
                : null);

        // ── Bonfire connections navigation ────────────────────────────────────────

        private void BonfireConnections_Click(object sender, RoutedEventArgs e)
        {
            RebuildBonfirePanel();
            SetupPanel.Visibility = Visibility.Collapsed;
            BonfireConnectionsPanel.Visibility = Visibility.Visible;
        }

        private void BackToSettings_Click(object sender, RoutedEventArgs e)
        {
            BonfireConnectionsPanel.Visibility = Visibility.Collapsed;
            SetupPanel.Visibility = Visibility.Visible;
        }

        // ── Bonfire panel construction ────────────────────────────────────────────

        // Returns the games that are currently enabled, in fixed DS1→DS2→DS3 order.
        private List<SoulsGame> ActiveGames()
        {
            var games = new List<SoulsGame>();
            if (DS1EnabledCheckBox.IsChecked == true) games.Add(SoulsGame.DSR);
            if (DS2EnabledCheckBox.IsChecked == true) games.Add(SoulsGame.DS2S);
            if (DS3EnabledCheckBox.IsChecked == true) games.Add(SoulsGame.DS3);
            return games;
        }

        private void RebuildBonfirePanel()
        {
            var activeGames = ActiveGames();

            // Rebuild column headers
            BonfireColumnHeaders.ColumnDefinitions.Clear();
            BonfireColumnHeaders.Children.Clear();
            for (int i = 0; i < activeGames.Count; i++)
            {
                BonfireColumnHeaders.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                if (i < activeGames.Count - 1)
                    BonfireColumnHeaders.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8) });

                var header = new TextBlock
                {
                    Text = GameLabel(activeGames[i]).ToUpperInvariant(),
                    Style = (Style)FindResource("SectionHeader"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0),
                };
                Grid.SetColumn(header, i * 2);
                BonfireColumnHeaders.Children.Add(header);
            }

            // Rebuild rows from _bonfireTriples
            BonfireRowsPanel.Children.Clear();
            for (int rowIndex = 0; rowIndex < _bonfireTriples.Count; rowIndex++)
                BonfireRowsPanel.Children.Add(BuildRowGrid(rowIndex, activeGames));

            RefreshAllDropdowns(activeGames);
        }

        private Grid BuildRowGrid(int rowIndex, List<SoulsGame> activeGames)
        {
            var rowGrid = new Grid { Margin = new Thickness(0, 0, 0, 6) };

            for (int i = 0; i < activeGames.Count; i++)
            {
                rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                if (i < activeGames.Count - 1)
                    rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8) });
            }

            for (int colIndex = 0; colIndex < activeGames.Count; colIndex++)
            {
                var game = activeGames[colIndex];
                var combo = new ComboBox
                {
                    Style = (Style)FindResource("DarkComboBox"),
                    Height = 30,
                    // Tag encodes which triple row and game this combo edits
                    Tag = (rowIndex, game),
                };

                combo.Items.Add("NONE");
                foreach (var b in BonfiresForGame(game))
                    combo.Items.Add(b);

                var current = _bonfireTriples[rowIndex].GetBonfireForGame(game);
                combo.SelectedItem = combo.Items.Contains(current) ? current : "NONE";

                combo.SelectionChanged += BonfireCombo_SelectionChanged;
                Grid.SetColumn(combo, colIndex * 2);
                rowGrid.Children.Add(combo);
            }

            return rowGrid;
        }

        // Repopulate every ComboBox's items, excluding bonfires already selected
        // in another row of the same column, then restore the current selection.
        private void RefreshAllDropdowns(List<SoulsGame> activeGames)
        {
            for (int colIndex = 0; colIndex < activeGames.Count; colIndex++)
            {
                var game = activeGames[colIndex];
                var allBonfires = BonfiresForGame(game);

                for (int rowIndex = 0; rowIndex < BonfireRowsPanel.Children.Count; rowIndex++)
                {
                    var rowGrid = (Grid)BonfireRowsPanel.Children[rowIndex];
                    // The combo for this column is at grid column colIndex*2
                    var combo = (ComboBox)rowGrid.Children[colIndex];

                    var current = _bonfireTriples[rowIndex].GetBonfireForGame(game);

                    // Bonfires taken by other rows in this column
                    var taken = new HashSet<string>();
                    for (int r = 0; r < _bonfireTriples.Count; r++)
                    {
                        if (r == rowIndex) continue;
                        var val = _bonfireTriples[r].GetBonfireForGame(game);
                        if (val != "NONE") taken.Add(val);
                    }

                    combo.SelectionChanged -= BonfireCombo_SelectionChanged;
                    combo.Items.Clear();
                    combo.Items.Add("NONE");
                    foreach (var b in allBonfires)
                        if (!taken.Contains(b))
                            combo.Items.Add(b);

                    combo.SelectedItem = combo.Items.Contains(current) ? current : "NONE";
                    combo.SelectionChanged += BonfireCombo_SelectionChanged;
                }
            }
        }

        private void BonfireCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not ComboBox combo || combo.Tag is not (int rowIndex, SoulsGame game))
                return;

            var selected = combo.SelectedItem as string ?? "NONE";
            _bonfireTriples[rowIndex] = _bonfireTriples[rowIndex].WithBonfireField(game, selected);

            RefreshAllDropdowns(ActiveGames());
        }

        // ── Add / Remove rows ─────────────────────────────────────────────────────

        private void AddBonfireRow_Click(object sender, RoutedEventArgs e)
        {
            var activeGames = ActiveGames();
            _bonfireTriples.Add(new BonfireTriple("NONE", "NONE", "NONE"));
            BonfireRowsPanel.Children.Add(BuildRowGrid(_bonfireTriples.Count - 1, activeGames));
            RefreshAllDropdowns(activeGames);
        }

        private void RemoveBonfireRow_Click(object sender, RoutedEventArgs e)
        {
            if (_bonfireTriples.Count == 0) return;
            _bonfireTriples.RemoveAt(_bonfireTriples.Count - 1);
            BonfireRowsPanel.Children.RemoveAt(BonfireRowsPanel.Children.Count - 1);
            RefreshAllDropdowns(ActiveGames());
        }

        // ── Restore defaults ──────────────────────────────────────────────────────

        private void RestoreBonfireDefaults_Click(object sender, RoutedEventArgs e)
        {
            _bonfireTriples.Clear();
            _bonfireTriples.AddRange(DefaultBonfireConnections);
            RebuildBonfirePanel();
        }
    }
}