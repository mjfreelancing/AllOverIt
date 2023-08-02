using ReactiveUI;
using System;
using System.Reactive.Disposables;
using ViewRegistryDemo.ViewModels;

namespace ViewRegistryDemo.Views
{
    public partial class View3 : ReactiveWindow<View3ViewModel>
    {
        public View3(View3ViewModel viewModel)
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
