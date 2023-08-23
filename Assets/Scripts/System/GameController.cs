using UnityEngine;

public class GameController : MonoBehaviour
{
    private ICellSlotsFactory _factory = null;
    private IStatistics _statistics = null;
    private SlotEvents _slotEvents = null;
    private GameEvents _gameEvents = null;
    private ViewEvents _viewEvents = null;
    private M3CellsConfigs _configs = null;

    private CellsSlotsConfig _currentConfig = default;

    public void Construct(ICellSlotsFactory factory, M3CellsConfigs configs, SlotEvents slotEvents,
        GameEvents gameEvents, ViewEvents viewEvents, IStatistics statistics)
    {
        _factory = factory;
        _configs = configs;

        _gameEvents = gameEvents;
        _gameEvents.OnGameNextLevel += OnNextLoad;
        _gameEvents.OnGameRestart += OnRestartLevel;

        _viewEvents = viewEvents;

        _slotEvents = slotEvents;
        _slotEvents.OnSlotsViewCreated += OnSlotsCreated;
        _slotEvents.OnSlotAffected += AddScoreAndCheck;

        _statistics = statistics;

        StartGame();
    }

    private void OnRestartLevel()
    {
        _viewEvents.OnLoadView?.Invoke(_currentConfig);
    }

    private void OnNextLoad()
    {
        StartGame();
    }

    private void AddScoreAndCheck()
    {
        if(_statistics.Score >= _currentConfig.TargetScore)
            _gameEvents.OnGameEnd?.Invoke();
    }

    private void OnSlotsCreated()
    {
        _gameEvents.OnGameStart?.Invoke();
    }

    private void StartGame()
    {
        _currentConfig = _configs.GetRandomConfig();
        _viewEvents.OnLoadView?.Invoke(_currentConfig);
    }

    private void OnDestroy()
    {
        _gameEvents.OnGameNextLevel -= OnNextLoad;

        _slotEvents.OnSlotsViewCreated -= OnSlotsCreated;
        _slotEvents.OnSlotAffected -= AddScoreAndCheck;
    }
}