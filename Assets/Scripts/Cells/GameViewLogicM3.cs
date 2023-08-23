using System;
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

    private List<ICellLogic> _logics = new List<ICellLogic>();
    private List<ICellLogic> _cellGenerators = new List<ICellLogic>();

    private CellsSlotsConfig? _currentConfig = null;

    public void Construct(ICellSlotsFactory factory, IPooledCustomFactory<IChip> chipFactory, SlotEvents slotEvents,
        GameEvents gameEvents, ViewEvents viewEvents)
    {
        _factory = factory;
        _chipFactory = chipFactory;

        _slotEvents = slotEvents;
        _slotEvents.OnAddedSlot += OnSlotAdded;
        _slotEvents.OnSlotsViewCreated += OnSlotsCreated;
        _slotEvents.OnGeneratorEmpty += OnSlotGeneratorGenerate;

        _gameEvents = gameEvents;
        _gameEvents.OnGameStart += OnGameStart;
        _gameEvents.OnGameRestart += OnGameRestart;
        _gameEvents.OnStartCommand += CheckGenerators;
        _gameEvents.OnGameEnd += OnGameEnd;

        _viewEvents = viewEvents;
        _viewEvents.OnLoadView += OnLoadView;
    }

    private void OnGameEnd()
    {
        foreach (var slot in _logics)
        {
            slot.Interactive = false;
        }
    }

    private void OnGameRestart()
    {
        _viewEvents.OnLoadView?.Invoke(_currentConfig.Value);
    }

    private void OnSlotGeneratorGenerate(ICellLogic slot)
    {
        StartCoroutine(TryGenerateNewChip(slot));
    }

    private IEnumerator TryGenerateNewChip(ICellLogic slot)
    {
        yield return new WaitForFixedUpdate();

        if (slot.Slot.Chip == null)
        {
            if (slot.Slot.Generator != null)
            {
                IChip newChip = _chipFactory.GetComponentByID(slot.Slot.Generator.GetNextChipId());
                newChip.SetPoolReleaser(_chipFactory);
                newChip.transform.gameObject.SetActive(true);
                slot.ProcessNewChip(newChip);
            }
        }
    }

    private void OnSlotAdded(ICellSlot slot)
    {
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
        _cellGenerators.Clear();
        foreach (var cellLogic in _logics)
        {
            cellLogic.BuildCellRefs();

            if (cellLogic.Slot.Generator != null)
            {
                _cellGenerators.Add(cellLogic);
            }
        }

        _viewEvents.OnGameViewReady?.Invoke(_currentConfig.Value.FieldSize);
    }

    private void CheckGenerators()
    {
        foreach (var generator in _cellGenerators)
        {
            StartCoroutine(TryGenerateNewChip(generator));
        }
    }

    private void OnLoadView(CellsSlotsConfig config)
    {
        ClearView();

        _currentConfig = config;
        _factory.CreateSlots(_currentConfig.Value);
    }

    private void ClearView()
    {
        foreach (var logic in _logics)
        {
            logic.Dispose();
        }
        _cellGenerators.Clear();
        _logics.Clear();
    }

    private void OnGameStart()
    {
        foreach (var slot in _logics)
        {
            slot.Interactive = true;
        }
    }

    private void OnDestroy()
    {
        _slotEvents.OnAddedSlot -= OnSlotAdded;
        _slotEvents.OnSlotsViewCreated -= OnSlotsCreated;
        _slotEvents.OnGeneratorEmpty -= OnSlotGeneratorGenerate;

        _gameEvents.OnGameStart -= OnGameStart;
        _gameEvents.OnGameRestart -= OnGameRestart;
        _gameEvents.OnStartCommand -= CheckGenerators;
        _gameEvents.OnGameEnd -= OnGameEnd;

        _viewEvents.OnLoadView -= OnLoadView;
    }
}