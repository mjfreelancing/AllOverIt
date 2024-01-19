using System.Windows;
using System.Windows.Threading;

namespace ThreadBindingDemo
{
    public partial class App : Application
    {
        private void HandleDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs eventArgs)
        {
            Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(eventArgs.Exception.Message, "Global Exception Handler", MessageBoxButton.OK, MessageBoxImage.Error);
                eventArgs.Handled = true;
            });
        }
    }
}
