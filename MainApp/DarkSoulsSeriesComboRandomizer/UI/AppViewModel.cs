using System.IO;

namespace DarkSoulsSeriesComboRandomizer.UI
{
    /// <summary>
    /// Holds all shared application state. Passed to each UserControl on
    /// construction. This is intentionally a plain class — no framework
    /// required. To move toward full MVVM later, implement INotifyPropertyChanged
    /// here and add bindings in the XAML panels.
    /// </summary>
    public class AppViewModel
    {
        // ── Constants ─────────────────────────────────────────────────────────────

        public const string DS1ExeName = "DarkSoulsRemastered.exe";
        public const string DS2ExeName = "DarkSoulsII.exe";
        public const string DS3ExeName = "DarkSoulsIII.exe";

        public static readonly string AppDataFolder =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "DarkSoulsSeriesComboRandomizer");

        public static readonly string SettingsFilePath =
            Path.Combine(AppDataFolder, "settings.json");

        // ── Setup state ───────────────────────────────────────────────────────────

        public string DS1Path { get; set; } = string.Empty;
        public string DS2Path { get; set; } = string.Empty;
        public string DS3Path { get; set; } = string.Empty;

        public bool DS1Enabled { get; set; } = true;
        public bool DS2Enabled { get; set; } = true;
        public bool DS3Enabled { get; set; } = true;

        public string Seed { get; set; } = string.Empty;
        public string SaveName { get; set; } = string.Empty;

        // How long (ms) to wait when switching between games; divided by three across
        // the pause/switch/resume points. Default chosen to preserve existing behaviour.
        public int GameSwitchDelayMs { get; set; } = 1500;

        // Whether games should be paused when minimized. Default true.
        public bool PauseMinimizedGames { get; set; } = true;

        // ── Bonfire connections ───────────────────────────────────────────────────

        public static readonly List<BonfireTriple> DefaultBonfireConnections =
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
        public List<BonfireTriple> BonfireTriples { get; set; } = [.. DefaultBonfireConnections];

        // ── Active session state (populated at Play time) ─────────────────────────

        public ModInstaller? Installer { get; set; }
        public GameCoordinationServer? Server { get; set; }

        // Captured at Play time for use by the error panel
        public string? ErrorDs1Dir { get; set; }
        public string? ErrorDs2Dir { get; set; }
        public string? ErrorDs3Dir { get; set; }
        public string? ErrorRunSaveName { get; set; }

        // ── Derived helpers ───────────────────────────────────────────────────────

        public List<SoulsGame> ActiveGames()
        {
            var games = new List<SoulsGame>();
            if (DS1Enabled) games.Add(SoulsGame.DSR);
            if (DS2Enabled) games.Add(SoulsGame.DS2S);
            if (DS3Enabled) games.Add(SoulsGame.DS3);
            return games;
        }

        public void ClearBonfireColumn(SoulsGame game)
        {
            for (int i = 0; i < BonfireTriples.Count; i++)
                BonfireTriples[i] = BonfireTriples[i].WithBonfireField(game, "NONE");
        }
    }
}