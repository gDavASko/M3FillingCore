public interface IGameViewLogic
{
    void Construct(ICellSlotsFactory factory, SlotEvents slotEvents, GameEvents gameEvents, ViewEvents viewEvents);
}