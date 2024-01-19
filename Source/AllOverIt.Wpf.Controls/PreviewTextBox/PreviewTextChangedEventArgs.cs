using System.Windows;

namespace AllOverIt.Wpf.Controls.PreviewTextBox
{
    /// <summary>The argument provided to the <see cref="PreviewTextBox.OnPreviewTextChanged(PreviewTextChangedEventArgs)"/>
    /// event handler.</summary>
    public sealed class PreviewTextChangedEventArgs : RoutedEventArgs
    {
        /// <summary>Indicates the reason for the text change.</summary>
        public PreviewTextChangedType Type { get; }

        /// <summary>A preview of the text value to be assigned. To accept this value, set the <see cref="RoutedEventArgs.Handled"/>
        /// property to <see langword="false"/>, otherwise <see langword="true"/>.</summary>
        public string Text { get; }

        /// <summary>Constructor.</summary>
        /// <param name="routedEvent">The routed event.</param>
        /// <param name="type">The reason for the text change.</param>
        /// <param name="text">A preview of the text value to be assigned.</param>
        public PreviewTextChangedEventArgs(RoutedEvent routedEvent, PreviewTextChangedType type, string text)
            : base(routedEvent)
        {
            Type = type;
            Text = text;
        }

        /// <summary>Constructor.</summary>
        /// <param name="routedEvent">The routed event.</param>
        /// <param name="source">The source of the event.</param>
        /// <param name="type">The reason for the text change.</param>
        /// <param name="text">A preview of the text value to be assigned.</param>
        public PreviewTextChangedEventArgs(RoutedEvent routedEvent, object source, PreviewTextChangedType type, string text)
            : base(routedEvent, source)
        {
            Type = type;
            Text = text;
        }
    }
}