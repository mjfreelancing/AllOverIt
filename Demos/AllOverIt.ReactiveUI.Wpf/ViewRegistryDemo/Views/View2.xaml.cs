using ReactiveUI;
using System.Reactive.Disposables;
using ViewRegistryDemo.ViewModels;

namespace ViewRegistryDemo.Views
{
    public partial class View2 : ReactiveWindow<View2ViewModel>
    {
        public View2(View2ViewModel viewModel)
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
