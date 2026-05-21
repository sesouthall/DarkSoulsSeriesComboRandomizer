using DarkSoulsSeriesComboRandomizer.UI;
using System.Windows;
using System.Windows.Controls;

namespace DarkSoulsSeriesComboRandomizer
{
    public partial class RunningPanel : UserControl
    {
        private readonly AppViewModel _vm;

        // Raised when the session has been fully stopped. MainWindow handles cleanup.
        public event EventHandler? StopRequested;

        public RunningPanel(AppViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
        }

        public void ShowSession(string seed, string saveName, string hintFilePath)
        {
            RunSeedDisplay.Text = seed;
            RunSaveDisplay.Text = saveName;
            RunHintDisplay.Text = hintFilePath;
            StopButton.IsEnabled = true;
            StopStatusText.Text = "";
        }

        public void SetStatusText(string text) => StopStatusText.Text = text;

        public void SetStopButtonEnabled(bool enabled) => StopButton.IsEnabled = enabled;

        private void Stop_Click(object sender, RoutedEventArgs e)
            => StopRequested?.Invoke(this, EventArgs.Empty);
    }
}