
using System;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class CellGenerator : MonoBehaviour, IGenerator
{
    [SerializeField] private string _id = "generator";
    [SerializeField] private string[] _chipVariants = null;

    private IObjectPool<IGenerator> _pool = null;

    public string ID => _id;
    public Action OnReleased { get; set; }
    public void Release()
    {
        OnReleased?.Invoke();
    }

    public void SetPoolContainer(IObjectPool<IGenerator> pool)
    {
        _pool = pool;
    }

    public string GetNextChipId()
    {
        return _chipVariants[Random.Range(0, _chipVariants.Length - 1)];
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}