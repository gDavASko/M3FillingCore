
using UnityEngine;

public class SlotEvents
{
    public System.Action<ICellSlot> OnAddedSlot { get; set; }
    public System.Action OnSlotsViewCreated { get; set; }
    public System.Action OnAddedChip { get; set; }
    public System.Action<ICellLogic> OnGeneratorEmpty { get; set; }
}

public class GameEvents
{
    public System.Action OnGameStart { get; set; }
    public System.Action OnGameEnd { get; set; }
    public System.Action OnGameRestart { get; set; }
}

public class ViewEvents
{
    public System.Action<Vector2> OnGameViewReady { get; set; }
    public System.Action<CellsSlotsConfig> OnLoadView { get; set; }
}