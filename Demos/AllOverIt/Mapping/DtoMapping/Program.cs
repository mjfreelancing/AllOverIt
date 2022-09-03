using AllOverIt.Mapping;
using AllOverIt.Mapping.Extensions;
using AllOverIt.Reflection;
using AllOverIt.Serialization.Abstractions;
using AllOverIt.Serialization.SystemTextJson;
using System;
using System.Collections.Generic;
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
                //Prop1 = 10,
                //Prop2 = true,
                Prop3 = new List<string>(new[] { "1", "2", "3" }),
                //Prop3b = new List<string>(new[] { "4", "5", "6" }),
                //Prop5a = 20,
                //Prop7 = null,
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

            //StaticCreateTargetUsingBindingOnOptions(source, serializer);
            //Console.WriteLine();

            //StaticMapOntoExistingTargetUsingBindingOnOptions(source, serializer);
            //Console.WriteLine();

            //MapperCreateTargetUsingBindingOnOptions(source, serializer);
            //Console.WriteLine();

            //MapperMapOntoExistingTargetUsingDefaultFilterOnOptions(source, serializer);
            //Console.WriteLine();

            //MapperMapOntoExistingTargetUsingOptionsDuringConfigure(source, serializer);
            //Console.WriteLine();

            MapperMapOntoExistingTargetUsingConversionDuringConfigure(source, serializer);
            Console.WriteLine();

            //MapperMapOntoExistingTargetUsingExcludeDuringConfigure(source, serializer);
            //Console.WriteLine();

            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        //private static void StaticCreateTargetUsingBindingOnOptions(SourceType source, IJsonSerializer serializer)
        //{
        //    var options = new ObjectMapperOptions
        //    {
        //        Binding = BindingOptions.DefaultScope | BindingOptions.Private | BindingOptions.DefaultAccessor | BindingOptions.DefaultVisibility
        //    };

        //    // Nested objects and collections are not supported by the static MapTo() methods.
        //    options.Exclude(nameof(SourceType.Prop3), nameof(SourceType.Prop3b), nameof(SourceType.Prop8), nameof(SourceType.Prop9));

        //    var target = source.MapTo<TargetType>(options);

        //    PrintMapping("Static create target, binding private properties only", source, target, serializer);
        //}

        //private static void StaticMapOntoExistingTargetUsingBindingOnOptions(SourceType source, IJsonSerializer serializer)
        //{
        //    var options = new ObjectMapperOptions
        //    {
        //        Binding = BindingOptions.DefaultScope | BindingOptions.Private | BindingOptions.DefaultAccessor | BindingOptions.DefaultVisibility
        //    };

        //    // Nested objects and collections are not supported by the static MapTo() methods.
        //    options.Exclude(nameof(SourceType.Prop3), nameof(SourceType.Prop3b), nameof(SourceType.Prop8), nameof(SourceType.Prop9));

        //    var target = new TargetType();
        //    _ = source.MapTo(target, options);

        //    PrintMapping("Static existing target, binding private properties only", source, target, serializer);
        //}

        //private static void MapperCreateTargetUsingBindingOnOptions(SourceType source, IJsonSerializer serializer)
        //{
        //    var objectMapper = new ObjectMapper
        //    {
        //        DefaultOptions =
        //        {
        //            Binding = BindingOptions.DefaultScope | BindingOptions.Private | BindingOptions.DefaultAccessor |BindingOptions.DefaultVisibility
        //        }
        //    };

        //    objectMapper.Configure<SourceType, TargetType>();

        //    var target = objectMapper.Map<TargetType>(source);

        //    PrintMapping("Create target, binding private properties only", source, target, serializer);
        //}

        //private static void MapperMapOntoExistingTargetUsingDefaultFilterOnOptions(SourceType source, IJsonSerializer serializer)
        //{
        //    var objectMapper = new ObjectMapper
        //    {
        //        DefaultOptions =
        //        {
        //            Filter = propInfo => propInfo.Name != nameof(SourceType.Prop1)
        //        }
        //    };

        //    objectMapper.Configure<SourceType, TargetType>();

        //    var target = new TargetType();
        //    _ = objectMapper.Map(source, target);

        //    PrintMapping("Existing target, filter out Prop1, default binding", source, target, serializer);
        //}

        //private static void MapperMapOntoExistingTargetUsingOptionsDuringConfigure(SourceType source, IJsonSerializer serializer)
        //{
        //    var objectMapper = new ObjectMapper();
        //    var target = new TargetType();

        //    objectMapper.Configure<SourceType, TargetType>(opt =>
        //    {
        //        // This is the default, just showing it
        //        opt.Binding = BindingOptions.Default;

        //        opt.Filter = propInfo => propInfo.Name == nameof(SourceType.Prop1) ||
        //                                 propInfo.Name == nameof(SourceType.Prop5a);

        //        // Copy Prop5a onto Prop5b and Prop1 onto Prop6
        //        opt.WithAlias(src => src.Prop5a, trg => trg.Prop5b)
        //           .WithAlias(src => src.Prop1, trg => trg.Prop6);
        //    });

        //    objectMapper.Map(source, target);

        //    PrintMapping("Existing target, filter Prop1 || Prop5a, aliased to Prop5b and Prop6, default binding", source, target, serializer);
        //}

        private static void MapperMapOntoExistingTargetUsingConversionDuringConfigure(SourceType source, IJsonSerializer serializer)
        {
            var objectMapper = new ObjectMapper();
            
            //objectMapper.Configure<ChildSourceType, ChildTargetType>(opt =>
            //{
            //    opt.DeepClone(src => src.Prop2a);
            //});

            objectMapper.Configure<ChildChildSourceType, ChildChildTargetType>();

            var target = new TargetType();

            //source.Prop7 = new[] {"Val1", "Val2", "Val3"};

            // This is for one of the child property types. It is optional. If not defined here then it will be defined
            // when Configure<SourceType, TargetType>() is called - as part of determining the matching properties.
            //objectMapper.Configure<ChildSourceType, ChildTargetType>();

            objectMapper.Configure<SourceType, TargetType>(opt =>
            {
                //opt.WithConversion(src => src.Prop7, value =>
                //{
                //    return value.Reverse();
                //});


                // Testing the child object property types that have a different name
                opt.WithAlias(src => src.Prop9, trg => trg.Prop8a);

                //opt.DeepClone(src => src.Prop9);

                //opt.WithConversion(src => src.Prop8, (mapper, value) =>
                //   {
                //       return mapper.Map<ChildTargetType>(value);
                //   });

                opt.WithConversion(src => src.Prop9, (mapper, value) =>
                {
                    return mapper.Map<ChildTargetType>(value);
                    //return new ChildTargetType { Prop1 = value.Prop1 };
                });

            });

            objectMapper.Map(source, target);

            PrintMapping("Existing target, using conversion IEnumerable to IReadOnlyCollection, alias child object property", source, target, serializer);
        }

        //private static void MapperMapOntoExistingTargetUsingExcludeDuringConfigure(SourceType source, IJsonSerializer serializer)
        //{
        //    var objectMapper = new ObjectMapper();
        //    var target = new TargetType();

        //    source.Prop7 = new[] { "Val1", "Val2", "Val3" };

        //    objectMapper.Configure<SourceType, TargetType>(opt =>
        //    {
        //        opt.Exclude(src => src.Prop7);
        //    });

        //    objectMapper.Map(source, target);

        //    PrintMapping("Existing target, exclude a non-mappable IEnumerable, default binding", source, target, serializer);
        //}

        private static void PrintMapping(string message, SourceType source, TargetType target, IJsonSerializer serializer)
        {
            Console.WriteLine(message);
            //Console.WriteLine($"  Source = {serializer.SerializeObject(source)} (Prop4 = {source.GetProp4()})");
            Console.WriteLine($"  Target = {serializer.SerializeObject(target)}");
        }
    }
}
