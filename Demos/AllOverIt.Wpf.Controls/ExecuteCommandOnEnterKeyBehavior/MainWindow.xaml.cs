using System.Windows;

namespace ExecuteCommandOnEnterKeyBehavior
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var viewModel = DataContext as MainWindowViewModel;
            viewModel!.OnClosing();
        }
    }
}
