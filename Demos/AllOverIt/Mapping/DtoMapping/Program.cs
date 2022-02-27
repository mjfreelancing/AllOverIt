using AllOverIt.Serialization.Abstractions;
using AllOverIt.Serialization.SystemTextJson;
using System;
using System.Collections.Generic;
using AllOverIt.ObjectMapping;
using AllOverIt.Reflection;

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
                Prop3 = new List<string>(new[] { "1", "2", "3" })
            };

            // Private values are copied even though not serialized
            var bindingOptions = BindingOptions.DefaultScope | BindingOptions.Private | BindingOptions.DefaultAccessor | BindingOptions.DefaultVisibility;

            // method 1 - maps source onto a newly constructed target
            var target = source.MapTo<TargetType>(bindingOptions);

            PrintMapping(source, target, serializer);


            // method 2 - maps source onto the provided target
            target = new TargetType();
            _ = source.MapTo(target, bindingOptions);

            PrintMapping(source, target, serializer);




            var mapperCache = new MapperCache();

            target = mapperCache.Map<TargetType>(source, bindingOptions);
            target = mapperCache.Map<TargetType>(source);
            target = mapperCache.Map<TargetType>(source, bindingOptions);     // will use the cached mapper


            target = new TargetType();
            target = mapperCache.Map(source, target, bindingOptions);








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
