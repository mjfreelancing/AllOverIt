using AllOverIt.Extensions;
using AllOverIt.Mapping;
using AllOverIt.Reflection;
using AllOverIt.Serialization.Json.Abstractions;
using AllOverIt.Serialization.Json.SystemText;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DtoMappingDemo
{
    internal class Program
    {
        static void Main()
        {
            //var options = new JsonSerializerOptions
            //{
            //    WriteIndented = true,
            //};

            var serializer = new SystemTextJsonSerializer(/*options*/);

            var source = new SourceType(5)
            {
                Prop1 = 10,
                Prop2 = true,
#pragma warning disable CA1861 // Avoid constant arrays as arguments
                Prop3 = new List<string>(new[] { "1", "2", "3" }),
                Prop3b = new List<string>(new[] { "4", "5", "6" }),
#pragma warning restore CA1861 // Avoid constant arrays as arguments
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

        private static void MapperCreateTargetUsingBindingOnOptions(SourceType source, IJsonSerializer serializer)
        {
            var mapperConfiguration = new ObjectMapperConfiguration();

            mapperConfiguration.Configure<SourceType, TargetType>(options =>
            {
                options.Binding = BindingOptions.DefaultScope | BindingOptions.Private | BindingOptions.DefaultAccessor | BindingOptions.DefaultVisibility;

                options.WithConversion(src => src.Prop3b, (mapper, value) =>
                {
                    return new ObservableCollection<string>(value);
                });
            });

            var objectMapper = new ObjectMapper(mapperConfiguration);

            var target = objectMapper.Map<TargetType>(source);

            PrintMapping("Create target, binding private properties only", source, target, serializer);
        }

        private static void MapperMapOntoExistingTargetUsingDefaultFilterOnOptions(SourceType source, IJsonSerializer serializer)
        {
            var mapperConfiguration = new ObjectMapperConfiguration();

            mapperConfiguration.Configure<SourceType, TargetType>(options =>
            {
                options.Filter = propInfo => propInfo.Name != nameof(SourceType.Prop1);

                options.WithConversion(src => src.Prop3b, (mapper, value) =>
                {
                    return new ObservableCollection<string>(value);
                });
            });

            var objectMapper = new ObjectMapper(mapperConfiguration);

            var target = new TargetType();
            _ = objectMapper.Map(source, target);

            PrintMapping("Existing target, filter out Prop1, default binding", source, target, serializer);
        }

        private static void MapperMapOntoExistingTargetUsingOptionsDuringConfigure(SourceType source, IJsonSerializer serializer)
        {
            var mapperConfiguration = new ObjectMapperConfiguration();

            mapperConfiguration.Configure<SourceType, TargetType>(options =>
            {
                // Not required since we are filtering to only Prop1, Prop3 and Prop5a
                //
                // opt.WithConversion(src => src.Prop3b, (mapper, value) =>
                // {
                //     return new ObservableCollection<string>(value);
                // });

                // This is the default, just showing it
                options.Binding = BindingOptions.Default;

                options.Filter = propInfo => propInfo.Name == nameof(SourceType.Prop1) ||
                                 propInfo.Name == nameof(SourceType.Prop3) ||
                                 propInfo.Name == nameof(SourceType.Prop5a);

                // Copy Prop5a onto Prop5b and Prop1 onto Prop6
                options.WithAlias(src => src.Prop5a, trg => trg.Prop5b)
                       .WithAlias(src => src.Prop1, trg => trg.Prop6);

                // With this commented out, Prop3 on the target will be an empty collection.
                // Remove the comment and Prop3 will be assigned the specified array,
                // opt.UseWhenNull(src => src.Prop3, new[] { "_" });
            });

            source.Prop3 = null;

            var objectMapper = new ObjectMapper(mapperConfiguration);
            var target = new TargetType();

            objectMapper.Map(source, target);

            PrintMapping("Existing target, filter Prop1 || Prop5a, aliased to Prop5b and Prop6, default binding", source, target, serializer);
        }

        private static void MapperMapOntoExistingTargetUsingConversionAndCloning(SourceType source, IJsonSerializer serializer)
        {
            var mapperConfiguration = new ObjectMapperConfiguration();

            // Collections are not cloned by default - this configuration will force a new collection to be created
            mapperConfiguration.Configure<ChildSourceType, ChildTargetType>(options =>
            {
                options.DeepCopy(src => src.Prop2a);
            });

            mapperConfiguration.Configure<SourceType, TargetType>(options =>
            {
                options.WithConversion(src => src.Prop3b, (mapper, value) =>
                {
                    return new ObservableCollection<string>(value);
                });

                options.WithConversion(src => src.Prop7, (mapper, value) =>
                {
                    return value.Reverse();
                });

                // Testing the child object property types that have a different name
                options.WithAlias(src => src.Prop9, trg => trg.Prop8a);

                options.DeepCopy(src => src.Prop9);

                options.WithConversion(src => src.Prop8, (mapper, value) =>
                   {
                       return mapper.Map<ChildTargetType>(value);
                   });

                options.WithConversion(src => src.Prop9, (mapper, value) =>
                {
                    // Can use the mapper, or explicitly create the return type

                    //return mapper.Map<ChildTargetType>(value);
                    return new ChildTargetType { Prop1 = value.Prop1 };
                });

            });

            source.Prop7 = new[] { "Val1", "Val2", "Val3" };
            var target = new TargetType();

            var objectMapper = new ObjectMapper(mapperConfiguration);

            objectMapper.Map(source, target);

            PrintMapping("Existing target, using conversion and cloning, alias child object property", source, target, serializer);

            if (ReferenceEquals(source.Prop8.Prop2a, target.Prop8?.Prop2a))
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
            // Showing the configuration can be provided at the time of construction
            var objectMapper = new ObjectMapper(configuration =>
            {
                configuration.Configure<SourceType, TargetType>(options =>
                {
                    options.WithConversion(src => src.Prop3b, (mapper, value) =>
                    {
                        return new ObservableCollection<string>(value);
                    });

                    // will exclude Prop7 because the item count matches. This means the target will be null
                    // (and not an empty collection) because the property is completely excluded from mapping
                    options.ExcludeWhen(src => src.Prop7, values => values.AsReadOnlyCollection().Count == 3);
                });
            });

            source.Prop7 = new[] { "Val1", "Val2", "Val3" };

            var target = new TargetType();

            objectMapper.Map(source, target);

            PrintMapping("Existing target, exclude a non-mappable IEnumerable, default binding", source, target, serializer);
        }

        private static void PrintMapping(string message, SourceType source, TargetType target, IJsonSerializer serializer)
        {
            Console.WriteLine(message);
            Console.WriteLine();
            Console.WriteLine($"  Source = {serializer.SerializeObject(source)} (Prop4 = {source.GetProp4()})");
            Console.WriteLine();
            Console.WriteLine($"  Target = {serializer.SerializeObject(target)}");
            Console.WriteLine();
            Console.WriteLine("====================================================");
            Console.WriteLine();
        }
    }
}
