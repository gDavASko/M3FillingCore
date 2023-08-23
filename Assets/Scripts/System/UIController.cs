
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Button _buttonRestart = null;
    [SerializeField] private Button _buttonStartLevel = null;
    [SerializeField] private TextMeshProUGUI _textScore = null;
    [SerializeField] private PanelResult _panelResult = null;

    private GameEvents _gameEvents = null;
    private SlotEvents _slotEvents = null;
    private IStatistics _statistics = null;

    private bool started = false;


    public void Construct(GameEvents gameEvents, SlotEvents slotEvents, IStatistics statistics)
    {
        _gameEvents = gameEvents;
        _gameEvents.OnGameEnd += EndGame;

        _slotEvents = slotEvents;
        _slotEvents.OnSlotAffected += OnAddScore;

        _statistics = statistics;
    }

    private void EndGame()
    {
        _panelResult.ShowWithParams(_statistics.Score, () => OnRestartClick());
    }

    private void OnAddScore()
    {
        _textScore.text = _statistics.Score.ToString();
    }

    private void Awake()
    {
        _buttonRestart.onClick.AddListener(OnRestartClick);
        _buttonStartLevel.onClick.AddListener(OnStartLevelClick);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            OnStartLevelClick();
    }

    private void OnStartLevelClick()
    {
        if (!started)
        {
            _gameEvents.OnStartCommand?.Invoke();

            _textScore.text = _statistics.Score.ToString();
            started = true;
        }
    }

    private void OnRestartClick()
    {
        _gameEvents.OnGameRestart?.Invoke();
        started = false;
    }
}