public interface IGenerator: IPoolable<IGenerator>
{
    string GetNextChipId();
}