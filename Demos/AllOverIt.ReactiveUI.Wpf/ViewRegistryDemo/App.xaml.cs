using AllOverIt.ReactiveUI.Factories;
using AllOverIt.ReactiveUI.ViewRegistry;
using AllOverIt.ReactiveUI.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using System.Windows;
using ViewRegistryDemo.Extensions;
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
            _host = new HostBuilder()
                        .ConfigureServices((context, services) =>
                        {
                            services.AddSingleton<MainWindow>();
                            services.AddSingleton<IViewFactory, ViewFactory>();
                            services.AddSingleton<IViewHandler, WpfViewHandler>();
                            services.AddSingleton<IDemoViewRegistry, DemoViewRegistry>();                              
                            services.RegisterViewTransient<View1ViewModel, View1>();
                            services.RegisterViewTransient<View2ViewModel, View2>();
                            services.RegisterViewTransient<View3ViewModel, View3>();
                        })
                        .Build();
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();

            var mainWindow = _host.Services.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}
