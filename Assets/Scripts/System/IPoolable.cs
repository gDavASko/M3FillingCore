using UnityEngine;
using UnityEngine.Pool;

public interface IPoolable<T>: IDestroyable, IIDGetter where T : class
{
    Transform transform { get; }

    System.Action OnReleased { get; set; }

    void Release();

    void SetPoolContainer(IObjectPool<T> pool);
}

public interface IDestroyable
{
    void DestroySelf();
}