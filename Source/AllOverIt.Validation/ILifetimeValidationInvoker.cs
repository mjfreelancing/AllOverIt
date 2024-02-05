using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.Validation
{
    /// <summary>Represents a validation invoker that supports validators registered with a custom lifetime in a <see cref="ServiceCollection"/>.</summary>
    public interface ILifetimeValidationInvoker : IValidationInvoker
    {
    }
}