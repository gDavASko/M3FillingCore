using System;
using UnityEngine;

public class DIController : MonoBehaviour
{
    [SerializeField] private GameParameters _parameters = null;
    [SerializeField] private GameController _gameController = null;
    [SerializeField] private UIController _uiController = null;

    [SerializeField] private M3CellsConfigs _configs = null;

    [SerializeReference] private CellSlotsFactory _factory = null;
    [SerializeReference] private ChipFactory _chipFactory;
    [SerializeReference] private CoverFactory _coverFactory;
    [SerializeReference] private GeneratorFactory _generatorFactory;

    private SlotEvents _slotEvents = new SlotEvents();

    private void Awake()
    {


        _factory.Construct(_chipFactory, _coverFactory, _generatorFactory, _slotEvents, _parameters);

        _gameController.Construct(_factory, _slotEvents, _configs);
    }
}