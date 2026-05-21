using DarkSoulsSeriesComboRandomizer.UI;
using System.Windows;
using System.Windows.Controls;

namespace DarkSoulsSeriesComboRandomizer
{
    public partial class BonfireConnectionsPanel : UserControl
    {
        private readonly AppViewModel _vm;

        public event EventHandler? BackToSettingsRequested;

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

        public BonfireConnectionsPanel(AppViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
        }

        // Called by MainWindow whenever the panel is made visible.
        public void Refresh() => RebuildPanel();

        // ── Panel construction ────────────────────────────────────────────────────

        private void RebuildPanel()
        {
            var activeGames = _vm.ActiveGames();

            // Column headers
            BonfireColumnHeaders.ColumnDefinitions.Clear();
            BonfireColumnHeaders.Children.Clear();
            for (int i = 0; i < activeGames.Count; i++)
            {
                BonfireColumnHeaders.ColumnDefinitions.Add(
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                if (i < activeGames.Count - 1)
                    BonfireColumnHeaders.ColumnDefinitions.Add(
                        new ColumnDefinition { Width = new GridLength(8) });

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

            // Rows
            BonfireRowsPanel.Children.Clear();
            for (int rowIndex = 0; rowIndex < _vm.BonfireTriples.Count; rowIndex++)
                BonfireRowsPanel.Children.Add(BuildRowGrid(rowIndex, activeGames));

            RefreshAllDropdowns(activeGames);
        }

        private Grid BuildRowGrid(int rowIndex, List<SoulsGame> activeGames)
        {
            var rowGrid = new Grid { Margin = new Thickness(0, 0, 0, 6) };

            for (int i = 0; i < activeGames.Count; i++)
            {
                rowGrid.ColumnDefinitions.Add(
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                if (i < activeGames.Count - 1)
                    rowGrid.ColumnDefinitions.Add(
                        new ColumnDefinition { Width = new GridLength(8) });
            }

            for (int colIndex = 0; colIndex < activeGames.Count; colIndex++)
            {
                var game = activeGames[colIndex];
                var combo = new ComboBox
                {
                    Style = (Style)FindResource("DarkComboBox"),
                    Height = 30,
                    Tag = (rowIndex, game),
                };

                combo.Items.Add("NONE");
                foreach (var b in BonfiresForGame(game))
                    combo.Items.Add(b);

                var current = _vm.BonfireTriples[rowIndex].GetBonfireForGame(game);
                combo.SelectedItem = combo.Items.Contains(current) ? current : "NONE";

                combo.SelectionChanged += BonfireCombo_SelectionChanged;
                Grid.SetColumn(combo, colIndex * 2);
                rowGrid.Children.Add(combo);
            }

            return rowGrid;
        }

        private void RefreshAllDropdowns(List<SoulsGame> activeGames)
        {
            for (int colIndex = 0; colIndex < activeGames.Count; colIndex++)
            {
                var game = activeGames[colIndex];
                var allBonfires = BonfiresForGame(game);

                for (int rowIndex = 0; rowIndex < BonfireRowsPanel.Children.Count; rowIndex++)
                {
                    var rowGrid = (Grid)BonfireRowsPanel.Children[rowIndex];
                    var combo = (ComboBox)rowGrid.Children[colIndex];
                    var current = _vm.BonfireTriples[rowIndex].GetBonfireForGame(game);

                    var taken = new HashSet<string>();
                    for (int r = 0; r < _vm.BonfireTriples.Count; r++)
                    {
                        if (r == rowIndex) continue;
                        var val = _vm.BonfireTriples[r].GetBonfireForGame(game);
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

        // ── Event handlers ────────────────────────────────────────────────────────

        private void BonfireCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not ComboBox combo || combo.Tag is not (int rowIndex, SoulsGame game))
                return;

            var selected = combo.SelectedItem as string ?? "NONE";
            _vm.BonfireTriples[rowIndex] = _vm.BonfireTriples[rowIndex].WithBonfireField(game, selected);

            RefreshAllDropdowns(_vm.ActiveGames());
        }

        private void AddBonfireRow_Click(object sender, RoutedEventArgs e)
        {
            var activeGames = _vm.ActiveGames();
            _vm.BonfireTriples.Add(new BonfireTriple("NONE", "NONE", "NONE"));
            BonfireRowsPanel.Children.Add(BuildRowGrid(_vm.BonfireTriples.Count - 1, activeGames));
            RefreshAllDropdowns(activeGames);
        }

        private void RemoveBonfireRow_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.BonfireTriples.Count == 0) return;
            _vm.BonfireTriples.RemoveAt(_vm.BonfireTriples.Count - 1);
            BonfireRowsPanel.Children.RemoveAt(BonfireRowsPanel.Children.Count - 1);
            RefreshAllDropdowns(_vm.ActiveGames());
        }

        private void RestoreBonfireDefaults_Click(object sender, RoutedEventArgs e)
        {
            _vm.BonfireTriples.Clear();
            _vm.BonfireTriples.AddRange(AppViewModel.DefaultBonfireConnections
                .Select(bonfireTriple => bonfireTriple with
                {
                    DS1Bonfire = _vm.DS1Enabled ? bonfireTriple.DS1Bonfire : "NONE",
                    DS2Bonfire = _vm.DS2Enabled ? bonfireTriple.DS2Bonfire : "NONE",
                    DS3Bonfire = _vm.DS3Enabled ? bonfireTriple.DS3Bonfire : "NONE",
                }));
            RebuildPanel();
        }

        private void BackToSettings_Click(object sender, RoutedEventArgs e)
            => BackToSettingsRequested?.Invoke(this, EventArgs.Empty);
    }
}