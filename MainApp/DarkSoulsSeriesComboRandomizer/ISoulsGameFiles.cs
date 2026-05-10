using SoulsFormats;

namespace DarkSoulsSeriesComboRandomizer
{
    public interface ISoulsGameFiles
    {
        PARAM EnemyItemLotParam { get; }
        PARAM TreasureItemLotParam { get; }

        IEnumerable<int> GetEnemyItemLotIds(MapName mapName);
        IEnumerable<int> GetTreasureItemLotIds(MapName mapName);
        void SaveItemLotChanges(string destinationFilePath);
    }
}
