using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameViewLogicM3 : MonoBehaviour, IGameViewLogic
{
    private ICellSlotsFactory _factory;

    private SlotEvents _slotEvents = null;
    private GameEvents _gameEvents = null;
    private ViewEvents _viewEvents = null;

    private List<ICellSlot> _slots = new List<ICellSlot>();
    private List<ICellLogic> _logics = new List<ICellLogic>();

    private CellsSlotsConfig? _currentConfig = null;

    public void Construct(ICellSlotsFactory factory, SlotEvents slotEvents, GameEvents gameEvents, ViewEvents viewEvents)
    {
        _factory = factory;

        _slotEvents = slotEvents;
        _slotEvents.OnAddedSlot += OnSlotAdded;
        _slotEvents.OnSlotsViewCreated += OnSlotsCreated;

        _gameEvents = gameEvents;
        _gameEvents.OnGameStart += OnGameStart;

        _viewEvents = viewEvents;
        _viewEvents.OnLoadView += OnLoadView;
    }

    private void OnSlotAdded(ICellSlot slot)
    {
        _slots.Add(slot);

        if (slot.transform.gameObject.TryGetComponent(typeof(ICellLogic), out Component logic))
        {
            _logics.Add(logic as ICellLogic);
        }
        else
        {
            ICellLogic newLogic = slot.transform.gameObject.AddComponent<CellLogicBase>();
            _logics.Add(newLogic);
        }
    }

    private void OnSlotsCreated()
    {
        foreach (var cellLogic in _logics)
        {
            cellLogic.BuildCellRefs();
        }

        _viewEvents.OnGameViewReady?.Invoke(_currentConfig.Value.FieldSize);
    }

    private void OnLoadView(CellsSlotsConfig config)
    {
        _currentConfig = config;
        _factory.CreateSlots(_currentConfig.Value);
    }

    private void OnGameStart()
    {

    }
}