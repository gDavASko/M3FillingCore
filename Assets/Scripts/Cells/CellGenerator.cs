
using System;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class CellGenerator : MonoBehaviour, IGenerator
{
    [SerializeField] private string _id = "generator";
    [SerializeField] private string[] _chipVariants = null;

    private IPoolReleaser<IGenerator> _pool = null;

    public string ID => _id;
    public Action OnReleased { get; set; }
    public void Release()
    {
        OnReleased?.Invoke();

        if (_pool != null)
        {
            _pool.ReleaseComponent(this);
        }
        else
        {
            DestroySelf();
        }
    }

    public void SetPoolReleaser(IPoolReleaser<IGenerator> pool)
    {
        _pool = pool;
    }

    public string GetNextChipId()
    {
        return _chipVariants[Random.Range(0, 1000) % _chipVariants.Length];
    }

    public void DestroySelf()
    {
        Debug.LogError($"[{name}] DestroySelf!");
        Destroy(this.gameObject);
    }
}