using AllOverIt.Wpf.Utils;
using System;
using System.Windows;

namespace WindowWrapperDemo
{
    public partial class MainWindow : Window
    {
        private WindowWrapper _wrapper;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            _wrapper = new WindowWrapper(this);
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);

            _wrapper.Dispose();
            _wrapper = null;
        }

        private void DisableMinimize_Click(object sender, RoutedEventArgs e)
        {
            _wrapper.DisableMinimizeButton();
        }

        private void EnableMinimize_Click(object sender, RoutedEventArgs e)
        {
            _wrapper.EnableMinimizeButton();
        }

        private void DisableMaximize_Click(object sender, RoutedEventArgs e)
        {
            _wrapper.DisableMaximizeButton();
        }

        private void EnableMaximize_Click(object sender, RoutedEventArgs e)
        {
            _wrapper.EnableMaximizeButton();
        }

        private void HideMinimizeMaximize_Click(object sender, RoutedEventArgs e)
        {
            _wrapper.HideMinimizeAndMaximizeButtons();
        }

        private void ShowMinimizeMaximize_Click(object sender, RoutedEventArgs e)
        {
            _wrapper.ShowMinimizeAndMaximizeButtons();
        }

        private void HideClose_Click(object sender, RoutedEventArgs e)
        {
            _wrapper.DisableCloseButton();
        }

        private void EnableClose_Click(object sender, RoutedEventArgs e)
        {
            _wrapper.EnableCloseButton();
        }

        private void HideAll_Click(object sender, RoutedEventArgs e)
        {
            _wrapper.HideAllButtons();
        }

        private void ShowAll_Click(object sender, RoutedEventArgs e)
        {
            _wrapper.ShowAllButtons();
        }
    }
}