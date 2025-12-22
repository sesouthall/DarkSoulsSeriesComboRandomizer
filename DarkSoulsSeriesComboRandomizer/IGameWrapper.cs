namespace DarkSoulsSeriesComboRandomizer
{
    public interface IGameWrapper
    {
        event EventFlagReactor OnModEventSet;
        event ItemReactor OnModItemPickUp;
        void Start();
        void Pause();
        void Resume();
    }
}