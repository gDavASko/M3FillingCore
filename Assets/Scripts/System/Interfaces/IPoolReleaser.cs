public interface IPoolReleaser<T> where T: class, IPoolable<T>
{
    void ReleaseComponent(T component);
}