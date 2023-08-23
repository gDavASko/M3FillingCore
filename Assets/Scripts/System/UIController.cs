
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Button _buttonRestart = null;

    private GameEvents _gameEvents = null;

    public void Construct(GameEvents gameEvents)
    {
        _gameEvents = gameEvents;
    }

    private void Awake()
    {
        _buttonRestart.onClick.AddListener(OnRestartClick);
    }

    private void OnRestartClick()
    {
        _gameEvents.OnGameRestart?.Invoke();
    }
}