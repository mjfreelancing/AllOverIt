using AllOverIt.Extensions;
using System.Globalization;
using System.Windows;

namespace AllOverIt.Wpf.Controls.PreviewTextBox.Handlers
{
    /// <summary>Implements a numeric preview handler.</summary>
    public sealed class NumericInputHandler : DependencyObject, IPreviewHandler
    {
        private static char DecimalSeparator => NumberFormatInfo.CurrentInfo.NumberDecimalSeparator[0];
        private static char NegativeSign => NumberFormatInfo.CurrentInfo.NegativeSign[0];

        /// <summary>The AllowNegative dependency property.</summary>
        public static readonly DependencyProperty AllowNegativeProperty = DependencyProperty.Register(
            nameof(AllowNegative),
            typeof(bool),
            typeof(NumericInputHandler),
            new PropertyMetadata(false));

        /// <summary>Indicates if the text entered is allowed to be a negative value.</summary>
        public bool AllowNegative
        {
            get { return (bool) GetValue(AllowNegativeProperty); }
            set { SetValue(AllowNegativeProperty, value); }
        }

        /// <summary>The AllowDecimal dependency property.</summary>
        public static readonly DependencyProperty AllowDecimalProperty = DependencyProperty.Register(
           nameof(AllowDecimal),
           typeof(bool),
           typeof(NumericInputHandler),
           new PropertyMetadata(false));

        /// <summary>Indicates if the text entered is allowed to be a decimal value.</summary>
        public bool AllowDecimal
        {
            get { return (bool) GetValue(AllowDecimalProperty); }
            set { SetValue(AllowDecimalProperty, value); }
        }

        /// <summary>Indicates if the numerical value entered is valid based on the configured properties of this handler.</summary>
        /// <param name="text">The preview text to be validated.</param>
        /// <returns><see langword="True"/> if the text is valid, otherwise false. </returns>
        public bool IsValid(string text)
        {
            if (text.IsNullOrEmpty())
            {
                return true;
            }

            if (text[0] == NegativeSign)
            {
                if (!AllowNegative)
                {
                    return false;
                }

                if (text.Length == 1)
                {
                    return true;
                }

                if (HasMoreThanOne(text, NegativeSign))
                {
                    return false;
                }
            }

            if (!AllowDecimal)
            {
                return int.TryParse(text, out var _);
            }

            if (HasMoreThanOne(text, DecimalSeparator))
            {
                return false;
            }

            if (text.Length == 1 && text[0] is '-' or '.')
            {
                return true;
            }

            if (text.Length == 2 && text == "-.")
            {
                return true;
            }

            // Must be '[-](0-9)' or '[-](0-9).' (the latter parses ok)
            return double.TryParse(text, out var _);
        }

        private static bool HasMoreThanOne(string text, char character)
        {
            var count = 0;

            foreach (var ch in text)
            {
                if (ch == character)
                {
                    count++;

                    if (count == 2)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}