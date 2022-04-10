using System;

namespace AllOverIt.DependencyInjection
{
    /// <summary>Service registrar options that determine what service and implementation types are registered.</summary>
    public interface IServiceRegistrarOptions
    {
        /// <summary>Used to filter service or implementation types from registration.</summary>
        /// <param name="filter">The filter delegate. The first argument is the potential service type and the second argument is the potential implementation type.</param>
        void Filter(Func<Type, Type, bool> filter);
    }
}