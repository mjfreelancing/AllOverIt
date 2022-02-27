using System.Linq;
using AllOverIt.Extensions;

namespace AllOverIt.ObjectMapping
{
    public static class Mapper
    {
        public static TTarget MapToType<TTarget>(this object source) where TTarget : new()
        {
            var target = new TTarget();
            return MapSourceToTarget(source, target);
        }

        public static TTarget MapTo<TSource, TTarget>(this TSource source, TTarget target)
        {
            return MapSourceToTarget(source, target);
        }

        private static TTarget MapSourceToTarget<TTarget>(object source, TTarget target)
        {
            var sourceType = source.GetType();
            var targetType = typeof(TTarget);

            var sourceProps = sourceType.GetPropertyInfo().Where(prop => prop.CanRead);
            var destProps = targetType.GetProperties().Where(prop => prop.CanWrite);

            var matching = sourceProps
                .FindMatches(destProps, src => src.Name, dest => dest.Name)
                .AsReadOnlyCollection();

            foreach (var match in matching)
            {
                var value = source.GetPropertyValue(sourceType, match.Name);
                target.SetPropertyValue(targetType, match.Name, value);
            }

            return target;
        }
    }
}