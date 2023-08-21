using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private ICellSlotsFactory _factory = null;
    private SlotEvents _slotEvents = null;
    private M3CellsConfigs _configs = null;

    private List<ICellSlot> _slots = new List<ICellSlot>();
    private List<ICellLogic> _logics = new List<ICellLogic>();

    public void Construct(ICellSlotsFactory factory, SlotEvents slotEvents, M3CellsConfigs configs)
    {
        _factory = factory;
        _configs = configs;

        _slotEvents = slotEvents;
        _slotEvents.OnAddedSlot += OnSlotAdded;
        _slotEvents.OnSlotsViewCreated += OnSlotsCreated;

        StartGame();
    }

    private void OnSlotsCreated()
    {
        foreach (var cellLogic in _logics)
        {
            cellLogic.BuildCellRefs();
        }
    }

    private void StartGame()
    {
        _factory.CreateSlots(_configs.GetRandomConfig());
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

    private void OnDestroy()
    {
        _slotEvents.OnAddedSlot -= OnSlotAdded;
        _slotEvents.OnSlotsViewCreated -= OnSlotsCreated;
    }
}