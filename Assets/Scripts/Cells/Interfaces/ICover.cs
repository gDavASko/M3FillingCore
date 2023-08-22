
public interface ICover: IPoolable<ICover>
{
    void DealDamage(System.Action OnDestroyCallback);
}