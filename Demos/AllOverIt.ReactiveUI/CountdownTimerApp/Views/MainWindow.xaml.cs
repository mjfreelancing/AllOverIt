using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using CountdownTimerApp.ViewModels;
using ReactiveUI;

namespace CountdownTimerApp.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainWindowViewModel();
            DataContext = ViewModel;

            this.WhenActivated(disposables =>
            {
                // The view model configured the countdown timer to notify on the main thread
                ViewModel
                    .WhenAnyValue(vm => vm.RemainingSeconds)
                    .Select(seconds => $"{TimeSpan.FromSeconds(seconds):hh\\:mm\\:ss}")
                    .Subscribe(formatted =>
                    {
                        RemainingTime.Text = formatted;
                    })
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.ResumeTimerCommand, view => view.StartButton)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.PauseTimerCommand, view => view.PauseButton)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.StopTimerCommand, view => view.StopButton)
                    .DisposeWith(disposables);
            });
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (ViewModel!.IsRunning)
            {
                MessageBox.Show("Pause or Stop the timer first");
            }

            e.Cancel = ViewModel.IsRunning;
        }
    }
}
