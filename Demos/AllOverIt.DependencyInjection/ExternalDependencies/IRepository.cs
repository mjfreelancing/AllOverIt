namespace ExternalDependencies
{
    public interface IRepository
    {
    }

    public abstract class RepositoryBase : IRepository
    {
    }

    internal sealed class ConcreteRepository : RepositoryBase
    {
    }
}