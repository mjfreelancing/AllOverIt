using AllOverIt.Wpf.Extensions;
using AllOverIt.Wpf.Utils;
using System.ComponentModel;
using System.Windows;
using ViewRegistryDemo.ViewModels;
using ViewRegistryDemo.ViewRegistry;

namespace ViewRegistryDemo
{
    public partial class MainWindow : Window
    {
        private readonly IDemoViewRegistry _viewRegistry;
        private WindowWrapper _windowWrapper = null!;

        public MainWindow(IDemoViewRegistry viewRegistry)
        {
            _viewRegistry = viewRegistry;

            InitializeComponent();
        }

        protected override void OnActivated(EventArgs eventArgs)
        {
            base.OnActivated(eventArgs);

            _windowWrapper = this
                .WrapWindow()
                .DisableMaximizeButton();
        }

        protected override void OnClosing(CancelEventArgs eventArgs)
        {
            eventArgs.Cancel = !_viewRegistry.TryCloseAllViews();
        }

        protected override void OnClosed(EventArgs eventArgs)
        {
            base.OnClosed(eventArgs);

            _windowWrapper.Dispose();
        }

        private void View1Click(object sender, RoutedEventArgs e)
        {
            // Maximum 1 window, starting with an Id of 1
            _viewRegistry.CreateOrActivateFor<View1ViewModel>(1, _ => 1, (vm, view, id) =>
            {
                (view as Window)!.Owner = this;

                // Set the new Id on the view model so it will be displayed on the Window
                vm.Id = id;
            });
        }

        private void View2Click(object sender, RoutedEventArgs eventArgs)
        {
            // Maximum 2 windows, maintaining Id of already open windows (will only ever be 1 or 2)
            _viewRegistry.CreateOrActivateFor<View2ViewModel>(2, viewItems =>
            {
                var id = 1;

                foreach (var viewItem in viewItems)
                {
                    if (id < viewItem.Id)
                    {
                        return id;
                    }

                    if (id == viewItem.Id)
                    {
                        id++;
                    }
                }

                return id;
            },
            (vm, view, id) =>
            {
                (view as Window)!.Owner = this;

                // Set the new Id on the view model so it will be displayed on the Window
                vm.Id = id;
            });
        }

        private void View3Click(object sender, RoutedEventArgs eventArgs)
        {
            // Maximum 3 windows, each new Id is always 1 more than the existing max
            _viewRegistry.CreateOrActivateFor<View3ViewModel>(3, viewItems =>
            {
                return viewItems.Count != 0
                    ? viewItems.Max(item => item.Id) + 1
                    : 1;
            },
            (vm, view, id) =>
            {
                (view as Window)!.Owner = this;

                // Set the new Id on the view model so it will be displayed on the Window
                vm.Id = id;
            });
        }
    }
}
