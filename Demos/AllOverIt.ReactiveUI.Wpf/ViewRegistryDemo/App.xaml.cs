using AllOverIt.ReactiveUI.Factories;
using AllOverIt.ReactiveUI.ViewRegistry;
using AllOverIt.ReactiveUI.Wpf;
using AllOverIt.ReactiveUI.Wpf.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI.Builder;
using System.Windows;
using ViewRegistryDemo.ViewModels;
using ViewRegistryDemo.ViewRegistry;
using ViewRegistryDemo.Views;

namespace ViewRegistryDemo
{
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            // ReactiveUI v23 requires explicit builder initialization before reactive mixins are used.
            var reactiveUiBuilder = RxAppBuilder.CreateReactiveUIBuilder();

            // Core services initialize ReactiveUI; platform services add WPF activation/view services.
            reactiveUiBuilder.WithCoreServices();
            reactiveUiBuilder.WithWpf();
            reactiveUiBuilder.BuildApp();

            _host = new HostBuilder()
                        .ConfigureServices((context, services) =>
                        {
                            services.AddSingleton<MainWindow>();
                            services.AddSingleton<IViewFactory, ViewFactory>();
                            services.AddSingleton<IViewHandler, ReactiveWindowViewHandler>();
                            services.AddSingleton<IDemoViewRegistry, DemoViewRegistry>();
                            services.RegisterWindowTransient<View1ViewModel, View1>();
                            services.RegisterWindowTransient<View2ViewModel, View2>();
                            services.RegisterWindowTransient<View3ViewModel, View3>();
                        })
                        .Build();
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();

            var mainWindow = _host.Services.GetService<MainWindow>()!;
            mainWindow.Show();
        }
    }
}
