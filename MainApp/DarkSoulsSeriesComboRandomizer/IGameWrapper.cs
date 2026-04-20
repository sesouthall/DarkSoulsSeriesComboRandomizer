namespace DarkSoulsSeriesComboRandomizer
{
    public delegate void ItemReactor(SoulsGame game, int itemId, int quantity);

    public interface IGameWrapper
    {
        event ItemReactor OnModItemPickUp;
        void Start();
        void Pause();
        void Resume();
    }
}