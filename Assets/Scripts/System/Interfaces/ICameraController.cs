using UnityEngine;

public interface ICameraController
{
    Camera Camera { get; }

    void Construct(ViewEvents gameEvents);
    void UpdateCameraDistance(Vector2 fieldSize);
}