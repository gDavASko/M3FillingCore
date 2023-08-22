using UnityEngine;
using UnityEngine.Pool;

public interface IPoolable<T>: IDestroyable, IIDGetter where T : class, IPoolable<T>
{
    Transform transform { get; }

    void Release();

    void SetPoolReleaser(IPoolReleaser<T> pool);
}