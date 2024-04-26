using ReactiveUI;
using System.Reactive.Disposables;
using ViewRegistryDemo.ViewModels;

namespace ViewRegistryDemo.Views
{
    public partial class View1 : ReactiveWindow<View1ViewModel>
    {
        public View1(View1ViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
            ViewModel = viewModel;

            this.WhenActivated(disposables =>
            {
                ViewModel
                    .WhenAnyValue(vm => vm.Id)
                    .Subscribe(id =>
                    {
                        IdText.Text = $"Id: {id}";
                    })
                    .DisposeWith(disposables);
            });
        }
    }
}
