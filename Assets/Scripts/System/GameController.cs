using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private ICellSlotsFactory _factory = null;
    private SlotEvents _slotEvents = null;
    private GameEvents _gameEvents = null;
    private ViewEvents _viewEvents = null;
    private M3CellsConfigs _configs = null;


    public void Construct(ICellSlotsFactory factory, M3CellsConfigs configs, SlotEvents slotEvents,
        GameEvents gameEvents, ViewEvents viewEvents)
    {
        _factory = factory;
        _configs = configs;

        _gameEvents = gameEvents;
        _viewEvents = viewEvents;

        _slotEvents = slotEvents;
        _slotEvents.OnSlotsViewCreated += OnSlotsCreated;

        StartGame();
    }

    private void OnSlotsCreated()
    {
        _gameEvents.OnGameStart?.Invoke();
    }

    private void StartGame()
    {
         _viewEvents.OnLoadView?.Invoke(_configs.GetRandomConfig());
    }

    private void OnDestroy()
    {
        _slotEvents.OnSlotsViewCreated -= OnSlotsCreated;
    }
}