using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace ViewRegistryDemo.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterViewTransient<TViewModel, TView>(this IServiceCollection services)
            where TViewModel : class
            where TView : ReactiveWindow<TViewModel>
        {
            services.AddTransient<TViewModel>();
            services.AddTransient<IViewFor<TViewModel>, TView>();

            return services;
        }

        //public static IServiceCollection RegisterViewScoped<TViewModel, TView>(this IServiceCollection services)
        //    where TViewModel : class
        //    where TView : ReactiveWindow<TViewModel>
        //{
        //    services.AddScoped<TViewModel>();
        //    services.AddScoped<IViewFor<TViewModel>, TView>();

        //    return services;
        //}

        //public static IServiceCollection RegisterViewSingleton<TViewModel, TView>(this IServiceCollection services)
        //    where TViewModel : class
        //    where TView : ReactiveWindow<TViewModel>
        //{
        //    services.AddSingleton<TViewModel>();
        //    services.AddSingleton<IViewFor<TViewModel>, TView>();

        //    return services;
        //}
    }
}