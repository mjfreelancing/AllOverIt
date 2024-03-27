using AllOverIt.Assertion;
using System;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="ErdOptions.CardinalityOptions"/>.</summary>
    public static class CardinalityExtensions
    {
        /// <summary>Provides the ability to configure a <see cref="LabelStyle"/> via a configuration action.</summary>
        /// <param name="cardinality">The <see cref="ErdOptions.CardinalityOptions"/> containing the <see cref="LabelStyle"/> to configure.</param>
        /// <param name="configure">The configuration action.</param>
        public static void SetLabelStyle(this ErdOptions.CardinalityOptions cardinality, Action<LabelStyle> configure)
        {
            _ = cardinality.WhenNotNull(nameof(cardinality));
            _ = configure.WhenNotNull(nameof(configure));

            configure.Invoke(cardinality.LabelStyle);
        }

        /// <summary>Assigns a <see cref="LabelStyle"/> to the provided <see cref="ErdOptions.CardinalityOptions"/>.</summary>
        /// <param name="cardinality">The <see cref="ErdOptions.CardinalityOptions"/> containing the <see cref="LabelStyle"/> to assign.</param>
        /// <param name="labelStyle">The <see cref="LabelStyle"/> to assign to the provided options.</param>
        public static void SetLabelStyle(this ErdOptions.CardinalityOptions cardinality, LabelStyle labelStyle)
        {
            _ = cardinality.WhenNotNull(nameof(cardinality));
            _ = labelStyle.WhenNotNull(nameof(labelStyle));

            cardinality.LabelStyle = labelStyle;
        }
    }
}