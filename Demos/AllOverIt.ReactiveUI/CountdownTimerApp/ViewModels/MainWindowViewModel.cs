using AllOverIt.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace CountdownTimerApp.ViewModels
{
    public class MainWindowViewModel : ActivatableViewModel
    {
        private const int CountdownSeconds = 10;
        private ICountdownTimer _countdownTimer;

        [Reactive]
        public double RemainingSeconds { get; private set; }

        [Reactive]
        public bool IsRunning { get; set; }

        [Reactive]
        public bool IsDone { get; set; }

        public ReactiveCommand<Unit, Unit> ResumeTimerCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> PauseTimerCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> StopTimerCommand { get; private set; }

        public MainWindowViewModel()
        {
            RemainingSeconds = CountdownSeconds;
        }

        protected override void OnActivated(CompositeDisposable disposables)
        {
            _countdownTimer = new CountdownTimer().DisposeWith(disposables);

            _countdownTimer
                .WhenAnyValue(timer => timer.IsRunning)
                .Subscribe(isRunning =>
                {
                    IsRunning = isRunning;
                })
                .DisposeWith(disposables);

            _countdownTimer
                // Not using WhenAnyValue() as it causes the subscription fire, resetting the RemainingSeconds to 0
                //.WhenAnyValue(timer => timer.RemainingTimeSpan)
                .ObservableForProperty(timer => timer.RemainingTimeSpan)
                .Select(item => item.Value)
                .Subscribe(remaining =>
                {
                    RemainingSeconds = remaining.TotalSeconds;
                })
                .DisposeWith(disposables);

            _countdownTimer
                .WhenCompleted()
                .Subscribe(_ =>                 // true when completed, false when cancelled
                {
                    RemainingSeconds = CountdownSeconds;
                    IsDone = true;
                })
                .DisposeWith(disposables);

            var canRun = _countdownTimer.WhenAnyValue(timer => timer.IsRunning).Select(isRunning => !isRunning);
            var canPause = _countdownTimer.WhenAnyValue(timer => timer.IsRunning).Select(isRunning => isRunning);

            ResumeTimerCommand = ReactiveCommand
                .Create(() =>
                {
                    _countdownTimer.Configure(TimeSpan.FromSeconds(RemainingSeconds), TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler);
                    _countdownTimer.Start();

                    IsDone = false;
                    IsRunning = true;
                }, canRun)
                .DisposeWith(disposables);

            PauseTimerCommand = ReactiveCommand
                .Create(() =>
                {
                    StopCountdownTimer(false);
                }, canPause)
                .DisposeWith(disposables);

            StopTimerCommand = ReactiveCommand
                .Create(() =>
                {
                    StopCountdownTimer(true);
                }, canPause)
                .DisposeWith(disposables);
        }

        private void StopCountdownTimer(bool resetRemainingTime)
        {
            _countdownTimer.Stop();
            IsRunning = false;

            if (resetRemainingTime)
            {
                RemainingSeconds = CountdownSeconds;
            }
        }
    }
}