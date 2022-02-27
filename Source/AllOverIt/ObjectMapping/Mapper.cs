using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AllOverIt.Extensions;
using AllOverIt.Reflection;

namespace AllOverIt.ObjectMapping
{
    public static class Mapper
    {
        public static TTarget MapTo<TTarget>(this object source, BindingOptions bindingOptions = BindingOptions.Default) where TTarget : new()
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

            var matching = GetMatchingProperties(sourceType, targetType, bindingOptions);

            foreach (var match in matching)
            {
                var value = source.GetPropertyValue(sourceType, match.Name, bindingOptions);
                target.SetPropertyValue(targetType, match.Name, value, bindingOptions);
            }

            return target;
        }

        internal static IReadOnlyCollection<PropertyInfo> GetMatchingProperties(Type sourceType, Type targetType, BindingOptions bindingOptions)
        {
            var sourceProps = sourceType.GetPropertyInfo(bindingOptions).Where(prop => prop.CanRead);
            var destProps = targetType.GetPropertyInfo(bindingOptions).Where(prop => prop.CanWrite);

            return sourceProps
                .FindMatches(destProps, src => src.Name, dest => dest.Name)
                .AsReadOnlyCollection();
        }
    }



    public interface IMapperCache
    {
        TTarget Map<TTarget>(object source, BindingOptions bindingOptions = BindingOptions.Default) where TTarget : new();

        TTarget Map<TSource, TTarget>(TSource source, TTarget target, BindingOptions bindingOptions = BindingOptions.Default);
    }



    public sealed class MapperCache : IMapperCache
    {
        private class MatchingPropertyMapper
        {
            private readonly Type _sourceType;
            private readonly Type _targetType;
            private readonly BindingOptions _bindingOptions;
            private readonly IReadOnlyCollection<PropertyInfo> _matching;

            public MatchingPropertyMapper(Type sourceType, Type targetType, BindingOptions bindingOptions)
            {
                _sourceType = sourceType;
                _targetType = targetType;
                _bindingOptions = bindingOptions;
                _matching = Mapper.GetMatchingProperties(_sourceType, _targetType, bindingOptions);
            }

            public void MapPropertyValues(object source, object target)
            {
                foreach (var match in _matching)
                {
                    var value = source.GetPropertyValue(_sourceType, match.Name, _bindingOptions);
                    target.SetPropertyValue(_targetType, match.Name, value, _bindingOptions);
                }
            }
        }

        // Not thread safe
        private readonly IDictionary<(Type, Type, BindingOptions), MatchingPropertyMapper> _mapperCache = new Dictionary<(Type, Type, BindingOptions), MatchingPropertyMapper>();

        public TTarget Map<TTarget>(object source, BindingOptions bindingOptions = BindingOptions.Default)
            where TTarget : new()
        {
            var sourceType = source.GetType();
            var targetType = typeof(TTarget);
            var target = new TTarget();

            return MapSourceToTarget(sourceType, source, targetType, target, bindingOptions);
        }

        public TTarget Map<TSource, TTarget>(TSource source, TTarget target, BindingOptions bindingOptions = BindingOptions.Default)
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            return MapSourceToTarget(sourceType, source, targetType, target, bindingOptions);
        }

        private TTarget MapSourceToTarget<TTarget>(Type sourceType, object source, Type targetType, TTarget target, BindingOptions bindingOptions)
        {
            var mappingKey = (sourceType, targetType, bindingOptions);

            if (!_mapperCache.TryGetValue(mappingKey, out var mapper))
            {
                mapper = new MatchingPropertyMapper(sourceType, targetType, bindingOptions);
                _mapperCache.Add(mappingKey, mapper);
            }

            mapper.MapPropertyValues(source, target);

            return target;
        }
    }
}