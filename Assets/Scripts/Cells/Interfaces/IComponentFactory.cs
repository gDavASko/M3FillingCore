using UnityEngine.Pool;

public interface IComponentFactory<T> where T: class, IPoolable<T>
{
    T GetComponentByID(string slotInfoFillType);
}