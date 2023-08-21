using System;
using UnityEngine;

public class GameParameters : MonoBehaviour
{
    [SerializeField] private Camera _gameViewCamera = null;

    public Vector3 _bottomLeftPoint { get; private set; } = default;
    public Vector3 _topRightPoint { get; private set; } = default;

    private void Awake()
    {
        CalculateCorners();
    }

    private void CalculateCorners()
    {
        var point = _gameViewCamera.ScreenToWorldPoint(Vector3.zero);
        point.z = 0;
        _bottomLeftPoint = point;

        point = _gameViewCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        point.z = 0;
        _topRightPoint = point;
    }
}