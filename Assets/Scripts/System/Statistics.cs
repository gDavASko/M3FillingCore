

public class Statistics : IStatistics
{
    public int Score { get; private set; } = 0;

    private GameEvents _gameEvents = null;
    private SlotEvents _slotEvents = null;

    public void Construct(GameEvents gameEvents, SlotEvents slotEvents)
    {
        _gameEvents = gameEvents;
        _gameEvents.OnGameStart += StartGame;

        _slotEvents = slotEvents;
        _slotEvents.OnSlotAffected += OnAddScore;
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