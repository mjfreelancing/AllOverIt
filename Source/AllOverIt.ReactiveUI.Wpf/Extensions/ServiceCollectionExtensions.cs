using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace AllOverIt.ReactiveUI.Wpf.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterWindowTransient<TViewModel, TView>(this IServiceCollection services)
            where TViewModel : class
            where TView : ReactiveWindow<TViewModel>
        {
            services.AddTransient<TViewModel>();
            services.AddTransient<IViewFor<TViewModel>, TView>();

            return services;
        }

        public static IServiceCollection RegisterWindowScoped<TViewModel, TView>(this IServiceCollection services)
            where TViewModel : class
            where TView : ReactiveWindow<TViewModel>
        {
            services.AddScoped<TViewModel>();
            services.AddScoped<IViewFor<TViewModel>, TView>();

            return services;
        }

        public static IServiceCollection RegisterWindowSingleton<TViewModel, TView>(this IServiceCollection services)
            where TViewModel : class
            where TView : ReactiveWindow<TViewModel>
        {
            services.AddSingleton<TViewModel>();
            services.AddSingleton<IViewFor<TViewModel>, TView>();

            return services;
        }



        public static IServiceCollection RegisterUserControlTransient<TViewModel, TView>(this IServiceCollection services)
            where TViewModel : class
            where TView : ReactiveUserControl<TViewModel>
        {
            services.AddTransient<TViewModel>();
            services.AddTransient<IViewFor<TViewModel>, TView>();

            return services;
        }

        public static IServiceCollection RegisterUserControlScoped<TViewModel, TView>(this IServiceCollection services)
            where TViewModel : class
            where TView : ReactiveUserControl<TViewModel>
        {
            services.AddScoped<TViewModel>();
            services.AddScoped<IViewFor<TViewModel>, TView>();

            return services;
        }

        public static IServiceCollection RegisterUserControlSingleton<TViewModel, TView>(this IServiceCollection services)
            where TViewModel : class
            where TView : ReactiveUserControl<TViewModel>
        {
            services.AddSingleton<TViewModel>();
            services.AddSingleton<IViewFor<TViewModel>, TView>();

            return services;
        }
    }
}