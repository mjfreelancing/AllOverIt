using AllOverIt.Wpf.Controls.PreviewTextBox.Handlers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace AllOverIt.Wpf.Controls.PreviewTextBox
{
    /// <summary>A <see cref="TextBox"/> that supports previewing the text to be assigned so it can be validated.
    /// Validation of the value can be performed by assigning a <see cref="PreviewHandler"/> or implementing an
    /// <see cref="OnPreviewTextChanged(PreviewTextChangedEventArgs)"/> event handler.</summary>
    public class PreviewTextBox : TextBox
    {
        private readonly Dictionary<ICommand, Action<RoutedEventArgs>> _commandHandlers = [];

        /// <summary>The <see cref="PreviewTextChanged"/> routed event.</summary>
        public static readonly RoutedEvent PreviewTextChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(PreviewTextChanged),
            RoutingStrategy.Tunnel,
            typeof(PreviewTextChangedEventHandler),
            typeof(PreviewTextBox));

        /// <summary>An event handler that receives a preview of the text to be assigned for the purpose
        /// of validating its content. To accept the value set the event args (of type <see cref="PreviewTextChangedEventArgs"/>)
        /// <see cref="RoutedEventArgs.Handled"/> property top <see langword="false"/>, otherwise <see langword="true"/>.</summary>
        public event PreviewTextChangedEventHandler PreviewTextChanged
        {
            add => AddHandler(PreviewTextChangedEvent, value);
            remove => RemoveHandler(PreviewTextChangedEvent, value);
        }

        /// <summary>The <see cref="PreviewHandler"/> dependency property.</summary>
        public static readonly DependencyProperty PreviewHandlerProperty =
            DependencyProperty.Register(
                nameof(PreviewHandler),
                typeof(IPreviewHandler),
                typeof(PreviewTextBox));

        /// <summary>An optional handler that can be used to validate a preview of the text to be assigned.
        /// If the handler returns <see langword="true"/> from its <see cref="IPreviewHandler.IsValid(string)"/>
        /// method then the value will be accepted. If <see langword="false"/> is returned then the preview
        /// text value will be forwarded to any assigned <see cref="OnPreviewTextChanged(PreviewTextChangedEventArgs)"/>
        /// event handlers. The event handlers will not be called if the <see cref="PreviewHandler"/> rejects the text.</summary>
        public IPreviewHandler PreviewHandler
        {
            get { return (IPreviewHandler) GetValue(PreviewHandlerProperty); }
            set { SetValue(PreviewHandlerProperty, value); }
        }

        /// <summary>Constructor.</summary>
        public PreviewTextBox()
        {
            _commandHandlers.Add(ApplicationCommands.Cut, HandleDeleteCommand);
            _commandHandlers.Add(ApplicationCommands.Delete, HandleDeleteCommand);
            _commandHandlers.Add(ApplicationCommands.Paste, HandleApplicationPasteCommand);
            _commandHandlers.Add(EditingCommands.Backspace, HandleBackspaceCommand);
            _commandHandlers.Add(EditingCommands.Delete, HandleDeleteCommand);
            _commandHandlers.Add(EditingCommands.DeleteNextWord, HandleDeleteNextWord);
            _commandHandlers.Add(EditingCommands.DeletePreviousWord, HandleDeletePreviousWord);
        }

        /// <summary>Raised when the control is initialized.</summary>
        /// <param name="eventArgs">The event args.</param>
        protected override void OnInitialized(EventArgs eventArgs)
        {
            // handle application and editing commands
            AddHandler(CommandManager.PreviewExecutedEvent, new ExecutedRoutedEventHandler(PreviewExecutedEvent), true);

            base.OnInitialized(eventArgs);
        }

        /// <summary>Raised when a key is pressed down.</summary>
        /// <param name="eventArgs">The event args that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs eventArgs)
        {
            if (eventArgs.Key == Key.Space)
            {
                OnTextInput(eventArgs, " ");
            }

            base.OnKeyDown(eventArgs);
        }

        /// <summary>Invoked when an unhandled PreviewTextInput attached event reaches an element in its route.</summary>
        /// <param name="eventArgs">The events args that contains the event data.</param>
        protected override void OnPreviewTextInput(TextCompositionEventArgs eventArgs)
        {
            OnTextInput(eventArgs, eventArgs.Text);

            base.OnPreviewTextInput(eventArgs);
        }

        /// <summary>Invoked when an unhandled PreviewTextChanged attached event reaches an element in its route.</summary>
        /// <param name="eventArgs">The events args that contains the event data.</param>
        protected virtual void OnPreviewTextChanged(PreviewTextChangedEventArgs eventArgs)
        {
            RaiseEvent(eventArgs);
        }

        private void PreviewExecutedEvent(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            if (_commandHandlers.TryGetValue(eventArgs.Command, out var handler))
            {
                handler.Invoke(eventArgs);
            }
        }

        private void HandleApplicationPasteCommand(RoutedEventArgs eventArgs)
        {
            var data = Clipboard.GetDataObject()?.GetData(typeof(string));

            if (data is string text)
            {
                OnTextInput(eventArgs, text);
            }
        }

        private void HandleBackspaceCommand(RoutedEventArgs eventArgs)
        {
            if (SelectionLength > 0 || SelectionStart > 0)
            {
                if (SelectionLength > 0)
                {
                    OnTextDelete(eventArgs, SelectionStart, SelectionLength);
                }
                else
                {
                    OnTextDelete(eventArgs, SelectionStart - 1, 1);
                }
            }
        }

        private void HandleDeleteCommand(RoutedEventArgs eventArgs)
        {
            if (SelectionLength > 0 || SelectionStart < Text.Length)
            {
                if (SelectionLength > 0)
                {
                    OnTextDelete(eventArgs, SelectionStart, SelectionLength);
                }
                else
                {
                    OnTextDelete(eventArgs, SelectionStart, 1);
                }
            }
        }

        private void HandleDeleteNextWord(RoutedEventArgs eventArgs)
        {
            var text = Text;
            var length = text.Length;
            var start = CaretIndex;
            var end = start;

            // go to end of current word
            while (end < length && !char.IsWhiteSpace(text[end]))
            {
                end++;
            }

            // go to end of white space before subsequent word
            while (end < length && char.IsWhiteSpace(text[end]))
            {
                end++;
            }

            if (end > start)
            {
                OnTextDelete(eventArgs, start, end - start);
            }

        }

        private void HandleDeletePreviousWord(RoutedEventArgs eventArgs)
        {
            var text = Text;
            var end = CaretIndex;
            var start = end;

            // go to end of previous word
            while (start > 0 && char.IsWhiteSpace(text[start - 1]))
            {
                start--;
            }

            // go to start of previous word
            while (start > 0 && !char.IsWhiteSpace(text[start - 1]))
            {
                start--;
            }

            if (end > start)
            {
                OnTextDelete(eventArgs, start, end - start);
            }
        }

        private void OnTextInput(RoutedEventArgs eventArgs, string text)
        {
            if (SelectionLength > 0)
            {
                OnTextReplace(eventArgs, SelectionStart, SelectionLength, text);
            }
            else
            {
                OnTextInsert(eventArgs, SelectionStart, text);
            }
        }

        private void OnTextDelete(RoutedEventArgs eventArgs, int startIndex, int count)
        {
            OnTextChange(eventArgs, PreviewTextChangedType.Delete, Text.Remove(startIndex, count));
        }

        private void OnTextInsert(RoutedEventArgs eventArgs, int startIndex, string text)
        {
            OnTextChange(eventArgs, PreviewTextChangedType.Insert, Text.Insert(startIndex, text));
        }

        private void OnTextReplace(RoutedEventArgs eventArgs, int startIndex, int count, string text)
        {
            OnTextChange(eventArgs, PreviewTextChangedType.Replace, Text.Remove(startIndex, count).Insert(startIndex, text));
        }

        private void OnTextChange(RoutedEventArgs eventArgs, PreviewTextChangedType type, string text)
        {
            if (string.CompareOrdinal(Text, text) != 0)
            {
                eventArgs.Handled = OnTextChangeHandled(type, text);
            }
        }

        private bool OnTextChangeHandled(PreviewTextChangedType type, string text)
        {
            if (!PreviewHandler?.IsValid(text) ?? false)
            {
                // Consider the event handled, meaning a validator rejected the text
                return true;
            }

            var eventArgs = new PreviewTextChangedEventArgs(PreviewTextChangedEvent, this, type, text);

            OnPreviewTextChanged(eventArgs);

            return eventArgs.Handled;
        }
    }
}