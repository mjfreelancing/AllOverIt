﻿using AllOverIt.Mapping;
using AllOverIt.Reflection;
using AllOverIt.Serialization.Abstractions;
using AllOverIt.Serialization.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DtoMapping
{
    internal class Program
    {
        static void Main()
        {
            var serializer = new SystemTextJsonSerializer();

            var source = new SourceType(5)
            {
                Prop1 = 10,
                Prop2 = true,
                Prop3 = new List<string>(new[] { "1", "2", "3" }),
                Prop3b = new List<string>(new[] { "4", "5", "6" }),
                Prop5a = 20,
                Prop7 = null,
                Prop8 =
                {
                    Prop1 = 99,
                    Prop2a = new[]
                    {
                        new ChildChildSourceType { Prop1 = 10 },
                        new ChildChildSourceType { Prop1 = 20 }
                    },
                    Prop2b = new[]
                    {
                        new ChildChildSourceType { Prop1 = 30 },
                        new ChildChildSourceType { Prop1 = 40 }
                    }
                },
                Prop9 =
                {
                    Prop1 = 88
                }
            };

            MapperCreateTargetUsingBindingOnOptions(source, serializer);
            Console.WriteLine();

            MapperMapOntoExistingTargetUsingDefaultFilterOnOptions(source, serializer);
            Console.WriteLine();

            MapperMapOntoExistingTargetUsingOptionsDuringConfigure(source, serializer);
            Console.WriteLine();

            MapperMapOntoExistingTargetUsingConversionAndCloning(source, serializer);
            Console.WriteLine();

            MapperMapOntoExistingTargetUsingExcludeDuringConfigure(source, serializer);
            Console.WriteLine();

            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static void ApplyCommonMapperConfiguration(IObjectMapper objectMapper)
        {
            objectMapper.Configure<SourceType, TargetType>(opt =>
            {
                opt.WithConversion(src => src.Prop3b, (mapper, value) =>
                {
                    return new ObservableCollection<string>(value);
                });
            });
        }

        private static void MapperCreateTargetUsingBindingOnOptions(SourceType source, IJsonSerializer serializer)
        {
            var objectMapper = new ObjectMapper
            {
                DefaultOptions =
                {
                    Binding = BindingOptions.DefaultScope | BindingOptions.Private | BindingOptions.DefaultAccessor |BindingOptions.DefaultVisibility
                }
            };

            ApplyCommonMapperConfiguration(objectMapper);

            var target = objectMapper.Map<TargetType>(source);

            PrintMapping("Create target, binding private properties only", source, target, serializer);
        }

        private static void MapperMapOntoExistingTargetUsingDefaultFilterOnOptions(SourceType source, IJsonSerializer serializer)
        {
            var objectMapper = new ObjectMapper
            {
                DefaultOptions =
                {
                    Filter = propInfo => propInfo.Name != nameof(SourceType.Prop1)
                }
            };

            ApplyCommonMapperConfiguration(objectMapper);

            var target = new TargetType();
            _ = objectMapper.Map(source, target);

            PrintMapping("Existing target, filter out Prop1, default binding", source, target, serializer);
        }

        private static void MapperMapOntoExistingTargetUsingOptionsDuringConfigure(SourceType source, IJsonSerializer serializer)
        {
            var objectMapper = new ObjectMapper();
            var target = new TargetType();

            objectMapper.Configure<SourceType, TargetType>(opt =>
            {
                opt.WithConversion(src => src.Prop3b, (mapper, value) =>
                {
                    return new ObservableCollection<string>(value);
                });

                // This is the default, just showing it
                opt.Binding = BindingOptions.Default;

                opt.Filter = propInfo => propInfo.Name == nameof(SourceType.Prop1) ||
                                         propInfo.Name == nameof(SourceType.Prop5a);

                // Copy Prop5a onto Prop5b and Prop1 onto Prop6
                opt.WithAlias(src => src.Prop5a, trg => trg.Prop5b)
                   .WithAlias(src => src.Prop1, trg => trg.Prop6);
            });

            objectMapper.Map(source, target);

            PrintMapping("Existing target, filter Prop1 || Prop5a, aliased to Prop5b and Prop6, default binding", source, target, serializer);
        }

        private static void MapperMapOntoExistingTargetUsingConversionAndCloning(SourceType source, IJsonSerializer serializer)
        {
            var objectMapper = new ObjectMapper();

            // Collections are not cloned by default - this configuration will force a new collection to be created
            objectMapper.Configure<ChildSourceType, ChildTargetType>(opt =>
            {
                opt.DeepClone(src => src.Prop2a);
            });

            var target = new TargetType();

            source.Prop7 = new[] {"Val1", "Val2", "Val3"};

            objectMapper.Configure<SourceType, TargetType>(opt =>
            {
                opt.WithConversion(src => src.Prop3b, (mapper, value) =>
                {
                    return new ObservableCollection<string>(value);
                });

                opt.WithConversion(src => src.Prop7, (mapper, value) =>
                {
                    return value.Reverse();
                });

                // Testing the child object property types that have a different name
                opt.WithAlias(src => src.Prop9, trg => trg.Prop8a);

                opt.DeepClone(src => src.Prop9);

                opt.WithConversion(src => src.Prop8, (mapper, value) =>
                   {
                       return mapper.Map<ChildTargetType>(value);
                   });

                opt.WithConversion(src => src.Prop9, (mapper, value) =>
                {
                    // Can use the mapper, or explicitly create the return type

                    return mapper.Map<ChildTargetType>(value);
                    //return new ChildTargetType { Prop1 = value.Prop1 };
                });

            });

            objectMapper.Map(source, target);

            PrintMapping("Existing target, using conversion and cloning, alias child object property", source, target, serializer);

            if (ReferenceEquals(source.Prop8.Prop2a, target.Prop8.Prop2a))
            {
                Console.WriteLine();
                Console.WriteLine("*** CLONE FAILED ***");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("*** CLONE worked as expected ***");
            }
        }

        private static void MapperMapOntoExistingTargetUsingExcludeDuringConfigure(SourceType source, IJsonSerializer serializer)
        {
            var objectMapper = new ObjectMapper();
            var target = new TargetType();

            source.Prop7 = new[] { "Val1", "Val2", "Val3" };

            objectMapper.Configure<SourceType, TargetType>(opt =>
            {
                opt.WithConversion(src => src.Prop3b, (mapper, value) =>
                {
                    return new ObservableCollection<string>(value);
                });

                opt.Exclude(src => src.Prop7);
            });

            objectMapper.Map(source, target);

            PrintMapping("Existing target, exclude a non-mappable IEnumerable, default binding", source, target, serializer);
        }

        private static void PrintMapping(string message, SourceType source, TargetType target, IJsonSerializer serializer)
        {
            Console.WriteLine(message);
            Console.WriteLine($"  Source = {serializer.SerializeObject(source)} (Prop4 = {source.GetProp4()})");
            Console.WriteLine($"  Target = {serializer.SerializeObject(target)}");
        }
    }
}
