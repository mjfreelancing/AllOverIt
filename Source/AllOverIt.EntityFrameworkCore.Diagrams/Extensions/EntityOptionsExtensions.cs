using AllOverIt.Assertion;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="ErdOptions.EntityOptions"/>.</summary>
    public static class EntityOptionsExtensions
    {
        /// <summary>Provides the ability to configure a <see cref="ShapeStyle"/> via a configuration action.</summary>
        /// <param name="options">The <see cref="ErdOptions.EntityOptions"/> containing the <see cref="ShapeStyle"/> to configure.</param>
        /// <param name="configure">The configuration action.</param>
        public static void SetShapeStyle(this ErdOptions.EntityOptions options, Action<ShapeStyle> configure)
        {
            _ = options.WhenNotNull(nameof(options));
            _ = configure.WhenNotNull(nameof(configure));

            configure.Invoke(options.ShapeStyle);
        }

        /// <summary>Assigns a <see cref="ShapeStyle"/> to the provided <see cref="ErdOptions.EntityOptions"/>.</summary>
        /// <param name="options">The <see cref="ErdOptions.EntityOptions"/> containing the <see cref="ShapeStyle"/> to assign.</param>
        /// <param name="shapeStyle">The <see cref="ShapeStyle"/> to assign to the provided options.</param>
        public static void SetShapeStyle(this ErdOptions.EntityOptions options, ShapeStyle shapeStyle)
        {
            _ = options.WhenNotNull(nameof(options));
            _ = shapeStyle.WhenNotNull(nameof(shapeStyle));

            options.ShapeStyle = shapeStyle;
        }
    }
}