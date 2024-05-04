using AllOverIt.Aws.Cdk.AppSync.Exceptions;
using AllOverIt.Extensions;
using AllOverIt.Reflection;
using System.Reflection;
using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync
{
    internal static class SchemaUtils
    {
        public static void AssertContainsNoProperties<TType>()
        {
            AssertNoProperties(typeof(TType));
        }

        public static void AssertNoProperties(SystemType type)
        {
            var elementType = type.IsArray
              ? type.GetElementType()
              : type;

            CheckForProperties(elementType);
        }

        public static IEnumerable<MethodInfo> GetTypeMethods<TType>()
        {
            var schemaType = typeof(TType);

            var methods = schemaType.GetMethodInfo();

            if (schemaType.IsInterface)
            {
                var inheritedMethods = schemaType.GetInterfaces().SelectMany(item => item.GetMethods());
                methods = methods.Concat(inheritedMethods);
            }

            return methods;
        }

        private static void CheckForProperties(SystemType type)
        {
            // not expecting class types to be used, but check anyway
            if (type != CommonTypes.StringType && !type.IsEnrichedEnum() && (type.IsClass || type.IsInterface))
            {
                if (type.IsInterface)
                {
                    foreach (var intf in type.GetInterfaces())
                    {
                        CheckForProperties(intf);
                    }
                }

                var properties = type.GetProperties();

                if (properties.Length > 0)
                {
                    throw new SchemaException($"The schema is expected to be declared in terms of methods (not properties). See '{type.GetFriendlyName()}'.");
                }
            }
        }
    }
}