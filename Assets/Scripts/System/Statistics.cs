

public class Statistics : IStatistics
{
    public int Score { get; private set; } = 0;
    public int CurrentTarget { get; private set; }

    private GameEvents _gameEvents = null;
    private SlotEvents _slotEvents = null;
    private ViewEvents _viewEvents = null;

    public void Construct(GameEvents gameEvents, SlotEvents slotEvents, ViewEvents viewEvents)
    {
        _gameEvents = gameEvents;
        _gameEvents.OnGameStart += StartGame;

        _slotEvents = slotEvents;
        _slotEvents.OnSlotAffected += OnAddScore;

        _viewEvents = viewEvents;
        _viewEvents.OnLoadView += UpdateLoadInfo;
    }

    private void UpdateLoadInfo(CellsSlotsConfig config)
    {
        CurrentTarget = config.TargetScore;
    }

    private void StartGame()
    {
        Score = 0;
    }

    private void OnAddScore()
    {
        Score++;
    }
}