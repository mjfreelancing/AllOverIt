using AllOverIt.Expressions;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Reflection;
using FluentAssertions;
using System.Reflection;

namespace AllOverIt.Tests.Reflection
{
    public partial class BindingOptionsHelperFixture : FixtureBase
    {
        public class BuildPropertyOrMethodBindingPredicate : BindingOptionsHelperFixture
        {
            private abstract class BindingDummy
            {
                public bool Prop1 { get; }
                protected bool Prop2 { get; }
                private bool Prop3 { get; }
                internal bool Prop4 { get; }

                public static bool StaticProp1 { get; }
                protected static bool StaticProp2 { get; }
                private static bool StaticProp3 { get; }
                internal static bool StaticProp4 { get; }

                public abstract bool AbstractProp1 { get; }
                protected abstract bool AbstractProp2 { get; }
                //private abstract not applicable
                internal abstract bool AbstractProp3 { get; }

                public virtual bool VirtualProp1 { get; }
                protected virtual bool VirtualProp2 { get; }
                //private virtual not applicable
                internal virtual bool VirtualProp3 { get; }
            }

            private class PropertyMetadata
            {
                public bool IsStatic { get; set; }
                public bool IsInstance { get; set; }
                public bool IsAbstract { get; set; }
                public bool IsVirtual { get; set; }
                public bool IsNonVirtual { get; set; }
                public bool IsPublic { get; set; }
                public bool IsProtected { get; set; }
                public bool IsPrivate { get; set; }
                public bool IsInternal { get; set; }
            }

            private readonly IDictionary<string, PropertyMetadata> _propertyMetadata;

            public BuildPropertyOrMethodBindingPredicate()
            {
                _propertyMetadata = new Dictionary<string, PropertyMetadata>();

                _propertyMetadata.Add("Prop1", new PropertyMetadata { IsPublic = true, IsNonVirtual = true, IsInstance = true });
                _propertyMetadata.Add("Prop2", new PropertyMetadata { IsProtected = true, IsNonVirtual = true, IsInstance = true });
                _propertyMetadata.Add("Prop3", new PropertyMetadata { IsPrivate = true, IsNonVirtual = true, IsInstance = true });
                _propertyMetadata.Add("Prop4", new PropertyMetadata { IsInternal = true, IsNonVirtual = true, IsInstance = true });

                _propertyMetadata.Add("StaticProp1", new PropertyMetadata { IsPublic = true, IsNonVirtual = true, IsStatic = true });
                _propertyMetadata.Add("StaticProp2", new PropertyMetadata { IsProtected = true, IsNonVirtual = true, IsStatic = true });
                _propertyMetadata.Add("StaticProp3", new PropertyMetadata { IsPrivate = true, IsNonVirtual = true, IsStatic = true });
                _propertyMetadata.Add("StaticProp4", new PropertyMetadata { IsInternal = true, IsNonVirtual = true, IsStatic = true });

                _propertyMetadata.Add("AbstractProp1", new PropertyMetadata { IsPublic = true, IsAbstract = true, IsVirtual = true, IsInstance = true });
                _propertyMetadata.Add("AbstractProp2", new PropertyMetadata { IsProtected = true, IsAbstract = true, IsVirtual = true, IsInstance = true });
                _propertyMetadata.Add("AbstractProp3", new PropertyMetadata { IsInternal = true, IsAbstract = true, IsVirtual = true, IsInstance = true });

                _propertyMetadata.Add("VirtualProp1", new PropertyMetadata { IsPublic = true, IsVirtual = true, IsInstance = true });
                _propertyMetadata.Add("VirtualProp2", new PropertyMetadata { IsProtected = true, IsVirtual = true, IsInstance = true });
                _propertyMetadata.Add("VirtualProp3", new PropertyMetadata { IsInternal = true, IsVirtual = true, IsInstance = true });
            }

            [Theory]
            [InlineData((BindingOptions) 0, BindingOptions.DefaultScope | BindingOptions.DefaultAccessor | BindingOptions.DefaultVisibility)]
            [InlineData(BindingOptions.Default, BindingOptions.DefaultScope | BindingOptions.DefaultAccessor | BindingOptions.DefaultVisibility)]

            [InlineData(BindingOptions.All, BindingOptions.AllScope | BindingOptions.AllAccessor | BindingOptions.AllVisibility)]
            [InlineData(BindingOptions.AllScope, BindingOptions.AllScope | BindingOptions.DefaultAccessor | BindingOptions.DefaultVisibility)]
            [InlineData(BindingOptions.AllAccessor, BindingOptions.DefaultScope | BindingOptions.AllAccessor | BindingOptions.DefaultVisibility)]
            [InlineData(BindingOptions.AllVisibility, BindingOptions.DefaultScope | BindingOptions.DefaultAccessor | BindingOptions.AllVisibility)]

            // invalid combinations are commented out
            //[InlineData(BindingOptions.Static | BindingOptions.Abstract | BindingOptions.Public)]
            //[InlineData(BindingOptions.Static | BindingOptions.Abstract | BindingOptions.Protected)]
            //[InlineData(BindingOptions.Static | BindingOptions.Abstract | BindingOptions.Private)]
            //[InlineData(BindingOptions.Static | BindingOptions.Abstract | BindingOptions.Internal)]

            //[InlineData(BindingOptions.Static | BindingOptions.Virtual | BindingOptions.Public)]
            //[InlineData(BindingOptions.Static | BindingOptions.Virtual | BindingOptions.Protected)]
            //[InlineData(BindingOptions.Static | BindingOptions.Virtual | BindingOptions.Private)]
            //[InlineData(BindingOptions.Static | BindingOptions.Virtual | BindingOptions.Internal)]

            [InlineData(BindingOptions.Static | BindingOptions.NonVirtual | BindingOptions.Public)]
            [InlineData(BindingOptions.Static | BindingOptions.NonVirtual | BindingOptions.Protected)]
            [InlineData(BindingOptions.Static | BindingOptions.NonVirtual | BindingOptions.Private)]
            [InlineData(BindingOptions.Static | BindingOptions.NonVirtual | BindingOptions.Internal)]

            [InlineData(BindingOptions.Instance | BindingOptions.Abstract | BindingOptions.Public)]
            [InlineData(BindingOptions.Instance | BindingOptions.Abstract | BindingOptions.Protected)]
            //[InlineData(BindingOptions.Instance | BindingOptions.Abstract | BindingOptions.Private)]
            [InlineData(BindingOptions.Instance | BindingOptions.Abstract | BindingOptions.Internal)]

            [InlineData(BindingOptions.Instance | BindingOptions.Virtual | BindingOptions.Public)]
            [InlineData(BindingOptions.Instance | BindingOptions.Virtual | BindingOptions.Protected)]
            //[InlineData(BindingOptions.Instance | BindingOptions.Virtual | BindingOptions.Private)]
            [InlineData(BindingOptions.Instance | BindingOptions.Virtual | BindingOptions.Internal)]

            [InlineData(BindingOptions.Instance | BindingOptions.NonVirtual | BindingOptions.Public)]
            [InlineData(BindingOptions.Instance | BindingOptions.NonVirtual | BindingOptions.Protected)]
            [InlineData(BindingOptions.Instance | BindingOptions.NonVirtual | BindingOptions.Private)]
            [InlineData(BindingOptions.Instance | BindingOptions.NonVirtual | BindingOptions.Internal)]
            public void Should_Get_Expected_Properties(BindingOptions actualOptions, BindingOptions? expectedOptions = null)
            {
                var actual = GetBindingDummyPropertyNames(actualOptions);
                var expected = GetExpectedNames(expectedOptions ?? actualOptions);

                expected.Should().BeEquivalentTo(actual);
            }

            private static IReadOnlyList<string> GetBindingDummyPropertyNames(BindingOptions options)
            {
                var predicate = BindingOptionsHelper.BuildPropertyOrMethodBindingPredicate(options);

                var typeInfo = typeof(BindingDummy).GetTypeInfo();

                return
                  (from propInfo in typeInfo.GetPropertyInfo(true)
                   let methodInfo = propInfo.GetMethod
                   where predicate.Invoke(methodInfo)
                   select propInfo.Name
                  ).AsReadOnlyList();
            }

            private IReadOnlyList<string> GetExpectedNames(BindingOptions options)
            {
                var scope = PredicateBuilder.False<PropertyMetadata>();

                if (options.HasFlag(BindingOptions.Static))
                {
                    scope = scope.Or(item => item.IsStatic);
                }

                if (options.HasFlag(BindingOptions.Instance))
                {
                    scope = scope.Or(item => item.IsInstance);
                }

                var access = PredicateBuilder.False<PropertyMetadata>();

                if (options.HasFlag(BindingOptions.Abstract))
                {
                    access = access.Or(item => item.IsAbstract);
                }

                if (options.HasFlag(BindingOptions.Virtual))
                {
                    access = access.Or(item => item.IsVirtual);
                }

                if (options.HasFlag(BindingOptions.NonVirtual))
                {
                    access = access.Or(item => item.IsNonVirtual);
                }

                var visibility = PredicateBuilder.False<PropertyMetadata>();

                if (options.HasFlag(BindingOptions.Public))
                {
                    visibility = visibility.Or(item => item.IsPublic);
                }

                if (options.HasFlag(BindingOptions.Protected))
                {
                    visibility = visibility.Or(item => item.IsProtected);
                }

                if (options.HasFlag(BindingOptions.Private))
                {
                    visibility = visibility.Or(item => item.IsPrivate);
                }

                if (options.HasFlag(BindingOptions.Internal))
                {
                    visibility = visibility.Or(item => item.IsInternal);
                }

                var predicate = scope.And(access).And(visibility).Compile();

                return
                  (from kvp in _propertyMetadata
                   where predicate.Invoke(kvp.Value)
                   select kvp.Key
                  ).AsReadOnlyList();
            }
        }
    }
}