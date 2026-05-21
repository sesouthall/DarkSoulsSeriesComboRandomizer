using DarkSoulsSeriesComboRandomizer.UI;
using Microsoft.Win32;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace DarkSoulsSeriesComboRandomizer
{
    public partial class SetupPanel : UserControl
    {
        private readonly AppViewModel _vm;

        // Raised when the user clicks Play. MainWindow handles the async work.
        public event EventHandler? PlayRequested;

        // Raised when the user clicks the bonfire connections button.
        public event EventHandler? BonfireConnectionsRequested;

        public SetupPanel(AppViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            LoadSavedSettings();
            InitializeSeed();
            PopulateSaveList();
            UpdatePlayButton();
        }

        // ── Initialisation ────────────────────────────────────────────────────────

        public void InitializeSeed()
        {
            var rng = new Random();
            int seed = rng.Next(0, int.MaxValue);
            SeedBox.Text = seed.ToString();
            SaveNameBox.Text = seed.ToString();
        }

        // ── Settings persistence ──────────────────────────────────────────────────

        private record PathSettings(
            string DS1Path, string DS2Path, string DS3Path,
            bool DS1Enabled = true, bool DS2Enabled = true, bool DS3Enabled = true);

        private void LoadSavedSettings()
        {
            try
            {
                if (!File.Exists(AppViewModel.SettingsFilePath)) return;
                var json = File.ReadAllText(AppViewModel.SettingsFilePath);
                var settings = JsonSerializer.Deserialize<PathSettings>(json);
                if (settings == null) return;

                if (!string.IsNullOrWhiteSpace(settings.DS1Path)) DS1PathBox.Text = settings.DS1Path;
                if (!string.IsNullOrWhiteSpace(settings.DS2Path)) DS2PathBox.Text = settings.DS2Path;
                if (!string.IsNullOrWhiteSpace(settings.DS3Path)) DS3PathBox.Text = settings.DS3Path;

                DS1EnabledCheckBox.IsChecked = settings.DS1Enabled;
                DS2EnabledCheckBox.IsChecked = settings.DS2Enabled;
                DS3EnabledCheckBox.IsChecked = settings.DS3Enabled;
                ApplyGameEnabledState();
            }
            catch { /* Non-fatal; silently ignore corrupt/missing settings */ }
        }

        public void SaveSettings()
        {
            if (DS1PathBox == null || DS2PathBox == null || DS3PathBox == null) return;
            try
            {
                Directory.CreateDirectory(AppViewModel.AppDataFolder);
                var settings = new PathSettings(
                    DS1PathBox.Text, DS2PathBox.Text, DS3PathBox.Text,
                    DS1EnabledCheckBox.IsChecked == true,
                    DS2EnabledCheckBox.IsChecked == true,
                    DS3EnabledCheckBox.IsChecked == true);
                File.WriteAllText(AppViewModel.SettingsFilePath, JsonSerializer.Serialize(settings));
            }
            catch { /* Non-fatal */ }
        }

        public void PopulateSaveList()
        {
            LoadSaveCombo.Items.Clear();
            LoadSaveCombo.Items.Add(new ComboBoxItem { Content = "— New Run —", IsSelected = true });

            if (!Directory.Exists(AppViewModel.AppDataFolder)) return;

            var saves = Directory.GetDirectories(AppViewModel.AppDataFolder)
                                 .Where(path => File.Exists(Path.Combine(path, "seed.txt")))
                                 .Select(Path.GetFileName)
                                 .OrderBy(name => name);
            foreach (var save in saves)
                LoadSaveCombo.Items.Add(new ComboBoxItem { Content = save });
        }

        // ── Validation ────────────────────────────────────────────────────────────

        private bool GamePathsAreValid()
        {
            bool ds1 = DS1EnabledCheckBox?.IsChecked == true;
            bool ds2 = DS2EnabledCheckBox?.IsChecked == true;
            bool ds3 = DS3EnabledCheckBox?.IsChecked == true;

            if (!ds1 && !ds2 && !ds3) return false;
            if (ds1 && !FileExistsAndMatchesName(DS1PathBox.Text, AppViewModel.DS1ExeName)) return false;
            if (ds2 && !FileExistsAndMatchesName(DS2PathBox.Text, AppViewModel.DS2ExeName)) return false;
            if (ds3 && !FileExistsAndMatchesName(DS3PathBox.Text, AppViewModel.DS3ExeName)) return false;
            return true;
        }

        private static bool FileExistsAndMatchesName(string path, string expectedName)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;
            if (!string.Equals(Path.GetFileName(path), expectedName, StringComparison.OrdinalIgnoreCase)) return false;
            return File.Exists(path);
        }

        public void UpdatePlayButton()
        {
            if (PlayButton == null) return;

            bool ds1 = DS1EnabledCheckBox?.IsChecked == true;
            bool ds2 = DS2EnabledCheckBox?.IsChecked == true;
            bool ds3 = DS3EnabledCheckBox?.IsChecked == true;
            bool valid = GamePathsAreValid();

            PlayButton.IsEnabled = valid;
            StatusText.Text = valid ? "" :
                (!ds1 && !ds2 && !ds3)
                    ? "At least one game must be enabled."
                    : "Please provide valid paths to all enabled game executables.";
        }

        // ── Enabled state ─────────────────────────────────────────────────────────

        private void ApplyGameEnabledState()
        {
            bool ds1 = DS1EnabledCheckBox?.IsChecked == true;
            bool ds2 = DS2EnabledCheckBox?.IsChecked == true;
            bool ds3 = DS3EnabledCheckBox?.IsChecked == true;

            if (DS1PathBox != null) DS1PathBox.IsEnabled = ds1;
            if (DS1BrowseButton != null) DS1BrowseButton.IsEnabled = ds1;
            if (DS2PathBox != null) DS2PathBox.IsEnabled = ds2;
            if (DS2BrowseButton != null) DS2BrowseButton.IsEnabled = ds2;
            if (DS3PathBox != null) DS3PathBox.IsEnabled = ds3;
            if (DS3BrowseButton != null) DS3BrowseButton.IsEnabled = ds3;

            if (_vm == null) return;

            for (int i = 0; i < _vm.BonfireTriples.Count; i++)
            {
                _vm.BonfireTriples[i] = _vm.BonfireTriples[i] with
                {
                    DS1Bonfire = ds1 ? _vm.BonfireTriples[i].DS1Bonfire : "NONE",
                    DS2Bonfire = ds2 ? _vm.BonfireTriples[i].DS2Bonfire : "NONE",
                    DS3Bonfire = ds3 ? _vm.BonfireTriples[i].DS3Bonfire : "NONE",
                };
            }
        }

        public void SetControlsEnabled(bool enabled)
        {
            DS1EnabledCheckBox.IsEnabled = enabled;
            DS2EnabledCheckBox.IsEnabled = enabled;
            DS3EnabledCheckBox.IsEnabled = enabled;
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

        public void SetStatusText(string text) => StatusText.Text = text;

        // ── View model sync ───────────────────────────────────────────────────────

        /// <summary>Writes current UI values into the view model before Play.</summary>
        public void SyncToViewModel()
        {
            _vm.DS1Path = DS1PathBox.Text;
            _vm.DS2Path = DS2PathBox.Text;
            _vm.DS3Path = DS3PathBox.Text;
            _vm.DS1Enabled = DS1EnabledCheckBox.IsChecked == true;
            _vm.DS2Enabled = DS2EnabledCheckBox.IsChecked == true;
            _vm.DS3Enabled = DS3EnabledCheckBox.IsChecked == true;
            _vm.Seed = SeedBox.Text.Trim();
            _vm.SaveName = SaveNameBox.Text.Trim();
        }

        // ── Browse handlers ───────────────────────────────────────────────────────

        private void BrowseDS1_Click(object sender, RoutedEventArgs e)
            => BrowseForExe(AppViewModel.DS1ExeName, DS1PathBox);

        private void BrowseDS2_Click(object sender, RoutedEventArgs e)
            => BrowseForExe(AppViewModel.DS2ExeName, DS2PathBox);

        private void BrowseDS3_Click(object sender, RoutedEventArgs e)
            => BrowseForExe(AppViewModel.DS3ExeName, DS3PathBox);

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

            var current = targetBox.Text;
            if (!string.IsNullOrWhiteSpace(current))
            {
                var dir = Path.GetDirectoryName(current);
                if (dir != null && Directory.Exists(dir))
                    dlg.InitialDirectory = dir;
            }

            if (dlg.ShowDialog() == true)
            {
                targetBox.Text = dlg.FileName;
                UpdatePlayButton();
                SaveSettings();
            }
        }

        // ── Event handlers ────────────────────────────────────────────────────────

        private void GameEnabled_Changed(object sender, RoutedEventArgs e)
        {
            // Keep the view model's bonfire data in sync when a game is disabled
            if (sender == DS1EnabledCheckBox && DS1EnabledCheckBox.IsChecked == false)
                _vm.ClearBonfireColumn(SoulsGame.DSR);
            else if (sender == DS2EnabledCheckBox && DS2EnabledCheckBox.IsChecked == false)
                _vm.ClearBonfireColumn(SoulsGame.DS2S);
            else if (sender == DS3EnabledCheckBox && DS3EnabledCheckBox.IsChecked == false)
                _vm.ClearBonfireColumn(SoulsGame.DS3);

            ApplyGameEnabledState();
            UpdatePlayButton();
            SaveSettings();
        }

        private void GamePath_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePlayButton();
            SaveSettings();
        }

        private void Seed_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SaveNameBox != null && int.TryParse(SaveNameBox.Text, out _))
                SaveNameBox.Text = SeedBox.Text;
        }

        private void LoadSave_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LoadSaveCombo.SelectedItem is not ComboBoxItem item) return;
            var saveName = item.Content?.ToString();
            if (string.IsNullOrEmpty(saveName)) return;

            if (saveName == "— New Run —")
            {
                InitializeSeed();
                return;
            }

            SaveNameBox.Text = saveName;

            var seedFilePath = Path.Combine(AppViewModel.AppDataFolder, saveName, "seed.txt");
            if (File.Exists(seedFilePath))
                SeedBox.Text = File.ReadAllText(seedFilePath).Trim();
        }

        private void BonfireConnections_Click(object sender, RoutedEventArgs e)
        {
            SyncToViewModel();
            BonfireConnectionsRequested?.Invoke(this, EventArgs.Empty);
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            // Validate locally before handing off
            if (string.IsNullOrWhiteSpace(SaveNameBox.Text.Trim()))
            {
                StatusText.Text = "Save name must not be empty.";
                return;
            }
            if (!int.TryParse(SeedBox.Text.Trim(), out _))
            {
                StatusText.Text = "Seed must be an integer.";
                return;
            }

            SyncToViewModel();
            PlayRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}