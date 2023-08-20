public interface IGeneratorFactory
{
    IGenerator GetGenerator(string slotInfoGeneratorType);
}