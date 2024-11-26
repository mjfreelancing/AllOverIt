using AllOverIt.ReactiveUI.Factories;
using AllOverIt.ReactiveUI.Wpf.Extensions;
using CountdownTimerAppDemo.ViewModels;
using CountdownTimerAppDemo.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace CountdownTimerAppDemo
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

                            // Without AllOverIt.ReactiveUI.Wpf, the call to RegisterWindowTransient() is the same as:
                            //
                            //   services.AddTransient<DoneWindowViewModel>();
                            //   services.AddTransient<IViewFor<DoneWindowViewModel>, DoneWindow>();
                            services.RegisterWindowTransient<DoneWindowViewModel, DoneWindow>();
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
