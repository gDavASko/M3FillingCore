public interface IPooledCustomFactory<T> : IComponentFactory<T>, IPoolReleaser<T> where T : class, IPoolable<T>
{

}