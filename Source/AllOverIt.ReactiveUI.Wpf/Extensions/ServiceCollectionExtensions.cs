using AllOverIt.ReactiveUI.Factories;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace AllOverIt.ReactiveUI.Wpf.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IServiceCollection"/>.</summary>
    public static class ServiceCollectionExtensions
    {
        #region RegisterWindow

        /// <summary>Registers the window and associated view model types with the service collection with a lifetime scope of <see cref="ServiceLifetime.Transient"/>.
        /// The view can later be resolved via the serive collection's <see cref="IServiceProvider"/> or via a <see cref="IViewFactory"/>.</summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <typeparam name="TView">The view type. Must be a <see cref="ReactiveWindow{TViewModel}"/>.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The original service collection to allow for a fluent syntax.</returns>
        public static IServiceCollection RegisterWindowTransient<TViewModel, TView>(this IServiceCollection serviceCollection)
            where TViewModel : class
            where TView : ReactiveWindow<TViewModel>
        {
            serviceCollection.AddTransient<TViewModel>();
            serviceCollection.AddTransient<IViewFor<TViewModel>, TView>();

            return serviceCollection;
        }

        /// <summary>Registers the window and associated view model types with the service collection with a lifetime scope of <see cref="ServiceLifetime.Scoped"/>.
        /// The view can later be resolved via the serive collection's <see cref="IServiceProvider"/> or via a <see cref="IViewFactory"/>.</summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <typeparam name="TView">The view type. Must be a <see cref="ReactiveWindow{TViewModel}"/>.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The original service collection to allow for a fluent syntax.</returns>
        public static IServiceCollection RegisterWindowScoped<TViewModel, TView>(this IServiceCollection serviceCollection)
            where TViewModel : class
            where TView : ReactiveWindow<TViewModel>
        {
            serviceCollection.AddScoped<TViewModel>();
            serviceCollection.AddScoped<IViewFor<TViewModel>, TView>();

            return serviceCollection;
        }

        /// <summary>Registers the window and associated view model types with the service collection with a lifetime scope of <see cref="ServiceLifetime.Singleton"/>.
        /// The view can later be resolved via the serive collection's <see cref="IServiceProvider"/> or via a <see cref="IViewFactory"/>.</summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <typeparam name="TView">The view type. Must be a <see cref="ReactiveWindow{TViewModel}"/>.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The original service collection to allow for a fluent syntax.</returns>
        public static IServiceCollection RegisterWindowSingleton<TViewModel, TView>(this IServiceCollection serviceCollection)
            where TViewModel : class
            where TView : ReactiveWindow<TViewModel>
        {
            serviceCollection.AddSingleton<TViewModel>();
            serviceCollection.AddSingleton<IViewFor<TViewModel>, TView>();

            return serviceCollection;
        }

        #endregion

        #region RegisterUserControl

        /// <summary>Registers the user control and associated view model types with the service collection with a lifetime scope of <see cref="ServiceLifetime.Transient"/>.
        /// The view can later be resolved via the serive collection's <see cref="IServiceProvider"/> or via a <see cref="IViewFactory"/>.</summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <typeparam name="TView">The view type. Must be a <see cref="ReactiveWindow{TViewModel}"/>.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The original service collection to allow for a fluent syntax.</returns>
        public static IServiceCollection RegisterUserControlTransient<TViewModel, TView>(this IServiceCollection serviceCollection)
            where TViewModel : class
            where TView : ReactiveUserControl<TViewModel>
        {
            serviceCollection.AddTransient<TViewModel>();
            serviceCollection.AddTransient<IViewFor<TViewModel>, TView>();

            return serviceCollection;
        }

        /// <summary>Registers the user control and associated view model types with the service collection with a lifetime scope of <see cref="ServiceLifetime.Scoped"/>.
        /// The view can later be resolved via the serive collection's <see cref="IServiceProvider"/> or via a <see cref="IViewFactory"/>.</summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <typeparam name="TView">The view type. Must be a <see cref="ReactiveWindow{TViewModel}"/>.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The original service collection to allow for a fluent syntax.</returns>
        public static IServiceCollection RegisterUserControlScoped<TViewModel, TView>(this IServiceCollection serviceCollection)
            where TViewModel : class
            where TView : ReactiveUserControl<TViewModel>
        {
            serviceCollection.AddScoped<TViewModel>();
            serviceCollection.AddScoped<IViewFor<TViewModel>, TView>();

            return serviceCollection;
        }

        /// <summary>Registers the user control and associated view model types with the service collection with a lifetime scope of <see cref="ServiceLifetime.Singleton"/>.
        /// The view can later be resolved via the serive collection's <see cref="IServiceProvider"/> or via a <see cref="IViewFactory"/>.</summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <typeparam name="TView">The view type. Must be a <see cref="ReactiveWindow{TViewModel}"/>.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The original service collection to allow for a fluent syntax.</returns>
        public static IServiceCollection RegisterUserControlSingleton<TViewModel, TView>(this IServiceCollection serviceCollection)
            where TViewModel : class
            where TView : ReactiveUserControl<TViewModel>
        {
            serviceCollection.AddSingleton<TViewModel>();
            serviceCollection.AddSingleton<IViewFor<TViewModel>, TView>();

            return serviceCollection;
        }

        #endregion
    }
}