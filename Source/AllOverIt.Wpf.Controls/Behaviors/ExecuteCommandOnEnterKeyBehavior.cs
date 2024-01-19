using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Input;

namespace AllOverIt.Wpf.Controls.Behaviors
{
    /// <summary>Implements a behavior that invokes a command when the Enter key is pressed on the associated object.</summary>
    public class ExecuteCommandOnEnterKeyBehavior : Behavior<UIElement>
    {
        /// <summary>The <see cref="Command"/> dependency property.</summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                "Command",
                typeof(ICommand),
                typeof(ExecuteCommandOnEnterKeyBehavior),
                new PropertyMetadata(null));

        /// <summary>The <see cref="CommandParameter"/> dependency property.</summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                "CommandParameter",
                typeof(object),
                typeof(ExecuteCommandOnEnterKeyBehavior),
                new PropertyMetadata(null));

        /// <summary>The command to be invoked when the Enter key is pressed in the bound control.</summary>
        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>The (optional) parameter to be passed to the <see cref="Command"/>.</summary>
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        /// <inheritdoc />
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.KeyDown += OnAssociatedObjectKeyDown;
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            AssociatedObject.KeyDown -= OnAssociatedObjectKeyDown;

            base.OnDetaching();
        }

        private void OnAssociatedObjectKeyDown(object sender, KeyEventArgs eventArgs)
        {
            if (eventArgs.Key == Key.Enter &&
                Command is not null &&
                Command.CanExecute(CommandParameter))
            {
                Command.Execute(CommandParameter);

                eventArgs.Handled = true;
            }
        }
    }
}
