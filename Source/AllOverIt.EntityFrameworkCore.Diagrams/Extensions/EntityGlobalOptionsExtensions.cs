using AllOverIt.Assertion;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="ErdOptions.EntityGlobalOptions"/>.</summary>
    public static class EntityGlobalOptionsExtensions
    {
        /// <summary>Provides the ability to configure a <see cref="ShapeStyle"/> via a configuration action.</summary>
        /// <param name="options">The <see cref="ErdOptions.EntityGlobalOptions"/> containing the <see cref="ShapeStyle"/> to configure.</param>
        /// <param name="configure">The configuration action.</param>
        public static void SetShapeStyle(this ErdOptions.EntityGlobalOptions options, Action<ShapeStyle> configure)
        {
            _ = options.WhenNotNull();
            _ = configure.WhenNotNull();

            configure.Invoke(options.ShapeStyle);
        }

        /// <summary>Assigns a <see cref="ShapeStyle"/> to the provided <see cref="ErdOptions.EntityGlobalOptions"/>.</summary>
        /// <param name="options">The <see cref="ErdOptions.EntityGlobalOptions"/> containing the <see cref="ShapeStyle"/> to assign.</param>
        /// <param name="shapeStyle">The <see cref="ShapeStyle"/> to assign to the provided options.</param>
        public static void SetShapeStyle(this ErdOptions.EntityGlobalOptions options, ShapeStyle shapeStyle)
        {
            _ = options.WhenNotNull();
            _ = shapeStyle.WhenNotNull();

            options.ShapeStyle = shapeStyle;
        }
    }
}