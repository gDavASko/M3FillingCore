public interface IStatistics
{
    int Score { get; }
    int CurrentTarget { get; }

    void Construct(GameEvents gameEvents, SlotEvents slotEvents, ViewEvents viewEvents);
}