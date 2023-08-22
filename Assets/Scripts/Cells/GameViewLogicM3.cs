using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameViewLogicM3 : MonoBehaviour, IGameViewLogic
{
    private ICellSlotsFactory _factory = null;
    private IPooledCustomFactory<IChip> _chipFactory = null;

    private SlotEvents _slotEvents = null;
    private GameEvents _gameEvents = null;
    private ViewEvents _viewEvents = null;

    private List<ICellSlot> _slots = new List<ICellSlot>();
    private List<ICellLogic> _logics = new List<ICellLogic>();

    private CellsSlotsConfig? _currentConfig = null;

    public void Construct(ICellSlotsFactory factory, IPooledCustomFactory<IChip> chipFactory, SlotEvents slotEvents, GameEvents gameEvents, ViewEvents viewEvents)
    {
        _factory = factory;
        _chipFactory = chipFactory;

        _slotEvents = slotEvents;
        _slotEvents.OnAddedSlot += OnSlotAdded;
        _slotEvents.OnSlotsViewCreated += OnSlotsCreated;
        _slotEvents.OnSlotAffected += OnSlotAffected;

        _gameEvents = gameEvents;
        _gameEvents.OnGameStart += OnGameStart;

        _viewEvents = viewEvents;
        _viewEvents.OnLoadView += OnLoadView;
    }

    private void OnSlotAffected(ICellLogic slot)
    {
        if (slot.Slot.CurrentChip == null)
        {
            if (slot.Slot.Generator != null)
            {
                IChip newChip = _chipFactory.GetComponentByID(slot.Slot.Generator.GetNextChipId());
                newChip.SetPoolReleaser(_chipFactory);
                slot.ProcessNewChip(newChip);
            }
        }
    }

    private void OnSlotAdded(ICellSlot slot)
    {
        _slots.Add(slot);

        if (slot.transform.gameObject.TryGetComponent(typeof(ICellLogic), out Component logic))
        {
            ICellLogic logicInst = logic as ICellLogic;
            logicInst.Init(_slotEvents);
            _logics.Add(logicInst);
        }
        else
        {
            ICellLogic newLogic = slot.transform.gameObject.AddComponent<CellLogicBase>();
            newLogic.Init(_slotEvents);
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
        foreach (var slot in _logics)
        {
            slot.Interactive = true;
        }
    }
}