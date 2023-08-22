public interface IGameViewLogic
{
    void Construct(ICellSlotsFactory factory, IPooledCustomFactory<IChip> chipFactory, SlotEvents slotEvents, GameEvents gameEvents, ViewEvents viewEvents);
}