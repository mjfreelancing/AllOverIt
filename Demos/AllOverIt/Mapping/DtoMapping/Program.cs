﻿using AllOverIt.Serialization.Abstractions;
using AllOverIt.Serialization.SystemTextJson;
using System;
using System.Collections.Generic;
using AllOverIt.Mapping;
using AllOverIt.Reflection;
using AllOverIt.Mapping.Extensions;

namespace DtoMapping
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var serializer = new SystemTextJsonSerializer();

            var source = new SourceType(5)
            {
                Prop1 = 10,
                Prop2 = true,
                Prop3 = new List<string>(new[] { "1", "2", "3" }),
                Prop5a = 20
            };

            // Private values are copied even though not serialized
            var options = new ObjectMapperOptions
            {
                Binding = BindingOptions.DefaultScope | BindingOptions.Private | BindingOptions.DefaultAccessor |BindingOptions.DefaultVisibility
            };

            // method 1 - maps source onto a newly constructed target
            var target = source.MapTo<TargetType>(options);

            PrintMapping(source, target, serializer);


            // method 2 - maps source onto the provided target
            target = new TargetType();
            _ = source.MapTo(target, options);

            PrintMapping(source, target, serializer);




            var objectMapper = new ObjectMapper
            {
                DefaultOptions =
                {
                    Binding = BindingOptions.DefaultScope | BindingOptions.Private | BindingOptions.DefaultAccessor |BindingOptions.DefaultVisibility
                }
            };

            // Will copy the private property
            target = objectMapper.Map<TargetType>(source);

            // Will NOT copy the private property
            target = objectMapper.Map<TargetType>(source, config => { config.Binding = BindingOptions.Default; });

            // Will copy the private property
            // Will use the cached mapper
            target = objectMapper.Map<TargetType>(source);

            // Will copy the private property but exclude Prop1
            target = new TargetType();
            target = objectMapper.Map(source, target, opt =>
            {
                opt.Binding = BindingOptions.DefaultScope | BindingOptions.Private | BindingOptions.DefaultAccessor | BindingOptions.DefaultVisibility;
                opt.Filter = propInfo => propInfo.Name != nameof(SourceType.Prop1);
            });








            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static void PrintMapping(SourceType source, TargetType target, IJsonSerializer serializer)
        {
            Console.WriteLine($"Source = {serializer.SerializeObject(source)} (Prop4 = {source.GetProp4()})");
            Console.WriteLine($"Target = {serializer.SerializeObject(target)}");
            Console.WriteLine();
        }
    }
}
