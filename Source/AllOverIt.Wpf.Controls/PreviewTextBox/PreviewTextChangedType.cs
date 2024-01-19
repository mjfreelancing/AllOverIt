namespace AllOverIt.Wpf.Controls.PreviewTextBox
{
    /// <summary>Indicates the reason for the text change.</summary>
    public enum PreviewTextChangedType
    {
        /// <summary>Text was deleted.</summary>
        Delete,

        /// <summary>Text was inserted.</summary>
        Insert,

        /// <summary>Text was replaced.</summary>
        Replace,
    }
}