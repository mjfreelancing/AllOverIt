namespace AllOverIt.Wpf.Controls.PreviewTextBox.Handlers
{
    /// <summary>Represents a PreviewTextBox handler.</summary>
    public interface IPreviewHandler
    {
        /// <summary>Determines if the preview text change is valid or not.</summary>
        /// <param name="text">The preview text to be validated.</param>
        /// <returns><see langword="True"/> if the text is valid, otherwise false.</returns>
        bool IsValid(string text);
    }
}