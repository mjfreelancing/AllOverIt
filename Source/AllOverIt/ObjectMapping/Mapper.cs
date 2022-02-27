using System.Linq;
using AllOverIt.Extensions;
using AllOverIt.Reflection;

namespace AllOverIt.ObjectMapping
{
    public static class Mapper
    {
        public static TTarget MapToType<TTarget>(this object source, BindingOptions bindingOptions = BindingOptions.Default) where TTarget : new()
        {
            var target = new TTarget();
            return MapSourceToTarget(source, target, bindingOptions);
        }

        public static TTarget MapTo<TSource, TTarget>(this TSource source, TTarget target, BindingOptions bindingOptions = BindingOptions.Default)
        {
            return MapSourceToTarget(source, target, bindingOptions);
        }

        private static TTarget MapSourceToTarget<TTarget>(object source, TTarget target, BindingOptions bindingOptions)
        {
            var sourceType = source.GetType();
            var targetType = typeof(TTarget);

            var sourceProps = sourceType.GetPropertyInfo(bindingOptions).Where(prop => prop.CanRead);
            var destProps = targetType.GetPropertyInfo(bindingOptions).Where(prop => prop.CanWrite);

            var matching = sourceProps
                .FindMatches(destProps, src => src.Name, dest => dest.Name)
                .AsReadOnlyCollection();

            foreach (var match in matching)
            {
                var value = source.GetPropertyValue(sourceType, match.Name, bindingOptions);
                target.SetPropertyValue(targetType, match.Name, value, bindingOptions);
            }

            return target;
        }
    }

    public interface IMapperCache
    {

    }

    public sealed class MapperCache : IMapperCache
    {

    }
}