using CountdownTimerAppDemo.ViewModels;
using ReactiveUI;

namespace CountdownTimerAppDemo.Views
{
    public partial class DoneWindow : ReactiveWindow<DoneWindowViewModel>
    {
        public DoneWindow(DoneWindowViewModel viewModel)
        {
            ViewModel = viewModel;      // Not actually used

            InitializeComponent();
        }
    }
}
