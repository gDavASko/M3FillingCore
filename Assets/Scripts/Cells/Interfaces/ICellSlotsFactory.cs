public interface ICellSlotsFactory
{
    bool Inited { get; }
    void CreateSlots(CellsSlotsConfig configs);
    void ReleaseSlots();

    void Construct(IPooledCustomFactory<IChip> componentFactory, IPooledCustomFactory<ICover> coverFactory,
        IPooledCustomFactory<IGenerator> generatorFactory, SlotEvents slotEvents, GameParameters parameters);
}