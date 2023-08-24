
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Button _buttonRestart = null;
    [SerializeField] private Button _buttonStartLevel = null;
    [SerializeField] private Button _buttonCompletelevel = null;
    [SerializeField] private TextMeshProUGUI _textScore = null;
    [SerializeField] private TextMeshProUGUI _textTarget = null;
    [SerializeField] private PanelResult _panelResult = null;

    private GameEvents _gameEvents = null;
    private SlotEvents _slotEvents = null;
    private IStatistics _statistics = null;

    private bool started = false;

    private void Awake()
    {
        _buttonRestart.onClick.AddListener(OnRestartClick);
        _buttonStartLevel.onClick.AddListener(OnStartLevelClick);
        _buttonCompletelevel.onClick.AddListener(OnCompleteClick);

        _buttonCompletelevel.gameObject.SetActive(false);

        DontDestroyOnLoad(this);
    }

    public void Construct(GameEvents gameEvents, SlotEvents slotEvents, IStatistics statistics)
    {
        _gameEvents = gameEvents;
        _gameEvents.OnGameEnd += EndGame;
        _gameEvents.OnGameNextLevel += OnNextLevel;
        _gameEvents.OnGameRestart += OnRestartLevel;


        _slotEvents = slotEvents;
        _slotEvents.OnSlotAffected += OnAddScore;
        _slotEvents.OnSlotsViewCreated += UpdateDataOnStart;

        _statistics = statistics;
    }

    private void UpdateDataOnStart()
    {
        _textTarget.text = _statistics.CurrentTarget.ToString();
    }

    private void OnCompleteClick()
    {
        _panelResult.ShowWithParams(_statistics.Score, () => OnStartNext());
        _buttonCompletelevel.gameObject.SetActive(false);
    }

    private void OnRestartLevel()
    {
        started = false;
    }

    private void OnNextLevel()
    {
        started = false;
    }

    private void EndGame()
    {
        _buttonCompletelevel.gameObject.SetActive(true);
    }

    private void OnAddScore()
    {
        _textScore.text = _statistics.Score.ToString();
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
    }

    private void OnStartNext()
    {
        _gameEvents.OnGameNextLevel?.Invoke();
    }
}