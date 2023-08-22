public interface IGameViewLogic
{
    void Construct(ICellSlotsFactory factory, IComponentFactory<IChip> chipFactory, SlotEvents slotEvents, GameEvents gameEvents, ViewEvents viewEvents);
}