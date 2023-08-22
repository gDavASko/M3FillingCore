using UnityEngine;
using UnityEngine.Pool;

public interface ICellSlotsFactory
{
    bool Inited { get; }
    void CreateSlots(CellsSlotsConfig configs);
    void ReleaseSlots();

    void Construct(IPooledCustomFactory<IChip> componentFactory, IPooledCustomFactory<ICover> coverFactory,
        IPooledCustomFactory<IGenerator> generatorFactory, SlotEvents slotEvents, GameParameters parameters);
}

[System.Serializable]
public struct CellsSlotsConfig
{
    [SerializeField] public Vector2 FieldSize;
    [SerializeField] public CellConfigs[] SlotCells;

    public CellsSlotsConfig(CellConfigs[] slotCells, Vector2 fieldSize)
    {
        FieldSize = fieldSize;
        SlotCells = slotCells;
    }
}

[System.Serializable]
public struct CellConfigs
{
    [SerializeField] public string CellType;
    [SerializeField] public CellConfig[] Cells;

    public CellConfigs(string cellType, CellConfig[] cells)
    {
        CellType = cellType;
        Cells = cells;
    }
}

[System.Serializable]
public struct CellConfig
{
    [SerializeField] public Vector2 Position; // cell position
    [SerializeField] public string FillType; // init component type filled
    [SerializeField] public string CoverType; // init cover type
    [SerializeField] public string GeneratorType; //Generator type id from config
    [SerializeField] public bool IsEmptySlot;

    public CellConfig(Vector2 position, string fillType, string coverType, string generatorType, bool isEmptySlot)
    {
        Position = position;
        FillType = fillType;
        CoverType = coverType;
        GeneratorType = generatorType;
        IsEmptySlot = isEmptySlot;
    }
}