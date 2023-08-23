using System;
using UnityEngine;

public class DIController : MonoBehaviour
{
    [SerializeField] private GameParameters _parameters = null;
    [SerializeField] private GameController _gameController = null;
    [SerializeField] private UIController _uiController = null;
    [SerializeField] private CameraController _cameraController = null;
    [SerializeField] private GameViewLogicM3 _gamelogic = null;

    [SerializeField] private M3CellsConfigs _configs = null;

    [SerializeReference] private CellSlotsFactory _factory = null;
    [SerializeReference] private ChipFactory _chipFactory;
    [SerializeReference] private CoverFactory _coverFactory;
    [SerializeReference] private GeneratorFactory _generatorFactory;

    private SlotEvents _slotEvents = new SlotEvents();
    private GameEvents _gameEvents = new GameEvents();
    private ViewEvents _viewEvents = new ViewEvents();

    private void Awake()
    {
        _factory.Construct(_chipFactory, _coverFactory, _generatorFactory, _slotEvents, _parameters);
        _gamelogic.Construct(_factory, _chipFactory, _slotEvents, _gameEvents, _viewEvents);
        _cameraController.Construct(_viewEvents);
        _gameController.Construct(_factory,  _configs, _slotEvents, _gameEvents, _viewEvents);
        _uiController.Construct(_gameEvents);
    }
}