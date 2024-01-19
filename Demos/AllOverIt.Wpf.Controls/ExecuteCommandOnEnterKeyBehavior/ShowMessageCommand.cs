using System;
using System.Windows;
using System.Windows.Input;

namespace ExecuteCommandOnEnterKeyBehavior
{
    public class ShowMessageCommand : ICommand
    {
#pragma warning disable 0067        // CS0067: The event is never used
        public event EventHandler CanExecuteChanged;
#pragma warning restore 0067

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var dateTime = (DateTime) parameter;

            MessageBox.Show(dateTime.ToString("o"));
        }
    }
}
