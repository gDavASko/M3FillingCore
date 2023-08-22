using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour, ICameraController
{
    private Camera _camera = null;
    private ViewEvents _gameEvents = null;

    public Camera Camera => _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    public void Construct(ViewEvents gameEvents)
    {
        _gameEvents = gameEvents;
        _gameEvents.OnGameViewReady += OnGameViewReady;
    }

    public void UpdateCameraDistance(Vector2 fieldSize)
    {
        float maxLength = Mathf.Max(fieldSize.x, fieldSize.y);
        _camera.orthographicSize = maxLength * 0.5f * (Screen.height / (float)Screen.width) + 0.3f;
    }

    private void OnGameViewReady(Vector2 viewSize)
    {
        UpdateCameraDistance(viewSize);
    }
}