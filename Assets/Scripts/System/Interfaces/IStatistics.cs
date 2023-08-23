public interface IStatistics
{
    int Score { get; }

    void Construct(GameEvents gameEvents, SlotEvents slotEvents);
}