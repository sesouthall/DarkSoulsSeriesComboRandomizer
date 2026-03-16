namespace DarkSoulsSeriesComboRandomizer
{
    public interface IGameWrapper
    {
        event ItemReactor OnModItemPickUp;
        void Start();
        void Pause();
        void Resume();
    }
}