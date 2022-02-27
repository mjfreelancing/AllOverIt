using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AllOverIt.Extensions;
using AllOverIt.Reflection;

namespace AllOverIt.Mapping
{
    public static class ObjectExtensions
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

            var matching = ObjectMapperHelper.GetMappableProperties(sourceType, targetType, bindingOptions);

            foreach (var match in matching)
            {
                var value = source.GetPropertyValue(sourceType, match.Name, bindingOptions);
                target.SetPropertyValue(targetType, match.Name, value, bindingOptions);
            }

            return target;
        }
    }


    public static class ObjectMapperHelper
    {
        public static IReadOnlyCollection<PropertyInfo> GetMappableProperties(Type sourceType, Type targetType, BindingOptions bindingOptions)
        {
            var sourceProps = sourceType.GetPropertyInfo(bindingOptions).Where(prop => prop.CanRead);
            var destProps = targetType.GetPropertyInfo(bindingOptions).Where(prop => prop.CanWrite);

            return sourceProps
                .FindMatches(destProps, src => $"{src.Name}.{src.PropertyType.GetFriendlyName()}", target => $"{target.Name}.{target.PropertyType.GetFriendlyName()}")
                .AsReadOnlyCollection();
        }
    }



    public interface IObjectMapper
    {
        void Configure<TSource, TTarget>(Action<ObjectMapperOptions> configure = default);
        TTarget Map<TTarget>(object source, Action<ObjectMapperOptions> configure = default) where TTarget : new();
        TTarget Map<TSource, TTarget>(TSource source, TTarget target, Action<ObjectMapperOptions> configure = default);
    }


    public sealed class ObjectMapperOptions
    {
        public BindingOptions Binding { get; set; } = BindingOptions.Default;
    }


    public sealed class ObjectMapper : IObjectMapper
    {
        private class MatchingPropertyMapper
        {
            private readonly Type _sourceType;
            private readonly Type _targetType;
            private readonly BindingOptions _bindingOptions;
            private readonly IReadOnlyCollection<PropertyInfo> _matching;

            internal MatchingPropertyMapper(Type sourceType, Type targetType, BindingOptions bindingOptions)
            {
                _sourceType = sourceType;
                _targetType = targetType;
                _bindingOptions = bindingOptions;
                _matching = ObjectMapperHelper.GetMappableProperties(_sourceType, _targetType, bindingOptions);
            }

            internal void MapPropertyValues(object source, object target)
            {
                foreach (var match in _matching)
                {
                    var value = source.GetPropertyValue(_sourceType, match.Name, _bindingOptions);
                    target.SetPropertyValue(_targetType, match.Name, value, _bindingOptions);
                }
            }
        }

        // Not thread safe - if to be used across multiple threads then configure the mappings in advance
        private readonly IDictionary<(Type, Type, BindingOptions), MatchingPropertyMapper> _mapperCache = new Dictionary<(Type, Type, BindingOptions), MatchingPropertyMapper>();

        public ObjectMapperOptions DefaultOptions { get; } = new();

        public void Configure<TSource, TTarget>(Action<ObjectMapperOptions> configure = default)
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            _ = TryCacheMapper(sourceType, targetType, configure);
        }

        public TTarget Map<TTarget>(object source, Action<ObjectMapperOptions> configure = default)
            where TTarget : new()
        {
            var sourceType = source.GetType();
            var targetType = typeof(TTarget);
            var target = new TTarget();

            return MapSourceToTarget(sourceType, source, targetType, target, configure);
        }

        public TTarget Map<TSource, TTarget>(TSource source, TTarget target, Action<ObjectMapperOptions> configure = default)
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            return MapSourceToTarget(sourceType, source, targetType, target, configure);
        }

        private TTarget MapSourceToTarget<TTarget>(Type sourceType, object source, Type targetType, TTarget target, Action<ObjectMapperOptions> configure = default)
        {
            var mapper = TryCacheMapper(sourceType, targetType, configure);
            mapper.MapPropertyValues(source, target);

            return target;
        }

        private MatchingPropertyMapper TryCacheMapper(Type sourceType, Type targetType, Action<ObjectMapperOptions> configure)
        {
            var mapperOptions = GetConfiguredOptions(configure);

            var mappingKey = (sourceType, targetType, mapperOptions.Binding);

            if (!_mapperCache.TryGetValue(mappingKey, out var mapper))
            {
                mapper = new MatchingPropertyMapper(sourceType, targetType, mapperOptions.Binding);
                _mapperCache.Add(mappingKey, mapper);
            }

            return mapper;
        }

        private ObjectMapperOptions GetConfiguredOptions(Action<ObjectMapperOptions> configure)
        {
            if (configure == null)
            {
                return DefaultOptions;
            }

            var mapperOptions = new ObjectMapperOptions();
            configure.Invoke(mapperOptions);

            return mapperOptions;
        }
    }
}