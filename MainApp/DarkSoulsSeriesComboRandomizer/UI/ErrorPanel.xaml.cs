using DarkSoulsSeriesComboRandomizer.UI;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DarkSoulsSeriesComboRandomizer
{
    public partial class ErrorPanel : UserControl
    {
        private readonly AppViewModel _vm;

        // Raised when the user clicks Return to Setup.
        public event EventHandler? ReturnToSetupRequested;

        public ErrorPanel(AppViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
        }

        public void ShowError(string message)
        {
            ErrorMessageText.Text = message;

            // Show/hide and enable/disable folder buttons based on which
            // games were enabled and whether their directories were captured.
            OpenDS1GameFolderButton.Visibility = _vm.DS1Enabled ? Visibility.Visible : Visibility.Collapsed;
            OpenDS2GameFolderButton.Visibility = _vm.DS2Enabled ? Visibility.Visible : Visibility.Collapsed;
            OpenDS3GameFolderButton.Visibility = _vm.DS3Enabled ? Visibility.Visible : Visibility.Collapsed;

            OpenDS1GameFolderButton.IsEnabled = _vm.DS1Enabled && _vm.ErrorDs1Dir != null && Directory.Exists(_vm.ErrorDs1Dir);
            OpenDS2GameFolderButton.IsEnabled = _vm.DS2Enabled && _vm.ErrorDs2Dir != null && Directory.Exists(_vm.ErrorDs2Dir);
            OpenDS3GameFolderButton.IsEnabled = _vm.DS3Enabled && _vm.ErrorDs3Dir != null && Directory.Exists(_vm.ErrorDs3Dir);

            OpenDS1SaveFolderButton.Visibility = _vm.DS1Enabled ? Visibility.Visible : Visibility.Collapsed;
            OpenDS2SaveFolderButton.Visibility = _vm.DS2Enabled ? Visibility.Visible : Visibility.Collapsed;
            OpenDS3SaveFolderButton.Visibility = _vm.DS3Enabled ? Visibility.Visible : Visibility.Collapsed;

            var runSaveFolder = _vm.ErrorRunSaveName != null
                ? Path.Combine(AppViewModel.AppDataFolder, _vm.ErrorRunSaveName)
                : null;
            OpenRunSaveFolder.IsEnabled = runSaveFolder != null && Directory.Exists(runSaveFolder);
        }

        // ── Folder helpers ────────────────────────────────────────────────────────

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

        // ── Click handlers ────────────────────────────────────────────────────────

        private void OpenDS1GameFolder_Click(object sender, RoutedEventArgs e)
            => OpenFolder(_vm.ErrorDs1Dir);

        private void OpenDS2GameFolder_Click(object sender, RoutedEventArgs e)
            => OpenFolder(_vm.ErrorDs2Dir);

        private void OpenDS3GameFolder_Click(object sender, RoutedEventArgs e)
            => OpenFolder(_vm.ErrorDs3Dir);

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
            => OpenFolder(Path.Combine(AppViewModel.AppDataFolder, "BackupVanillaFiles"));

        private void OpenRunSaveFolder_Click(object sender, RoutedEventArgs e)
            => OpenFolder(_vm.ErrorRunSaveName != null
                ? Path.Combine(AppViewModel.AppDataFolder, _vm.ErrorRunSaveName)
                : null);

        private void ReturnToSetup_Click(object sender, RoutedEventArgs e)
        {
            _vm.ErrorDs1Dir = null;
            _vm.ErrorDs2Dir = null;
            _vm.ErrorDs3Dir = null;
            _vm.ErrorRunSaveName = null;
            ReturnToSetupRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}