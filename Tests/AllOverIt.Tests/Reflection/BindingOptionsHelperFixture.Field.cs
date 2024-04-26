using AllOverIt.Expressions;
using AllOverIt.Extensions;
using AllOverIt.Reflection;
using FluentAssertions;
using System.Reflection;

namespace AllOverIt.Tests.Reflection
{
    public partial class BindingOptionsHelperFixture
    {
        public class BuildFieldInfoBindingPredicate : BindingOptionsHelperFixture
        {
            private abstract class BindingDummy
            {
                public bool Field1;
                protected bool Field2;
                private bool Field3;
                internal bool Field4;

                public static bool StaticField1;
                protected static bool StaticField2;
                private static bool StaticField3;
                internal static bool StaticField4;
            }

            private class FieldMetadata
            {
                public bool IsStatic { get; set; }
                public bool IsInstance { get; set; }
                public bool IsNonVirtual { get; set; }
                public bool IsPublic { get; set; }
                public bool IsProtected { get; set; }
                public bool IsPrivate { get; set; }
                public bool IsInternal { get; set; }
            }

            private readonly IDictionary<string, FieldMetadata> _fieldMetadata;

            public BuildFieldInfoBindingPredicate()
            {
                _fieldMetadata = new Dictionary<string, FieldMetadata>();

                _fieldMetadata.Add("Field1", new FieldMetadata { IsPublic = true, IsNonVirtual = true, IsInstance = true });
                _fieldMetadata.Add("Field2", new FieldMetadata { IsProtected = true, IsNonVirtual = true, IsInstance = true });
                _fieldMetadata.Add("Field3", new FieldMetadata { IsPrivate = true, IsNonVirtual = true, IsInstance = true });
                _fieldMetadata.Add("Field4", new FieldMetadata { IsInternal = true, IsNonVirtual = true, IsInstance = true });

                _fieldMetadata.Add("StaticField1", new FieldMetadata { IsPublic = true, IsNonVirtual = true, IsStatic = true });
                _fieldMetadata.Add("StaticField2", new FieldMetadata { IsProtected = true, IsNonVirtual = true, IsStatic = true });
                _fieldMetadata.Add("StaticField3", new FieldMetadata { IsPrivate = true, IsNonVirtual = true, IsStatic = true });
                _fieldMetadata.Add("StaticField4", new FieldMetadata { IsInternal = true, IsNonVirtual = true, IsStatic = true });
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

            //[InlineData(BindingOptions.Instance | BindingOptions.Abstract | BindingOptions.Public)]
            //[InlineData(BindingOptions.Instance | BindingOptions.Abstract | BindingOptions.Protected)]
            //[InlineData(BindingOptions.Instance | BindingOptions.Abstract | BindingOptions.Private)]
            //[InlineData(BindingOptions.Instance | BindingOptions.Abstract | BindingOptions.Internal)]

            //[InlineData(BindingOptions.Instance | BindingOptions.Virtual | BindingOptions.Public)]
            //[InlineData(BindingOptions.Instance | BindingOptions.Virtual | BindingOptions.Protected)]
            //[InlineData(BindingOptions.Instance | BindingOptions.Virtual | BindingOptions.Private)]
            //[InlineData(BindingOptions.Instance | BindingOptions.Virtual | BindingOptions.Internal)]

            [InlineData(BindingOptions.Instance | BindingOptions.NonVirtual | BindingOptions.Public)]
            [InlineData(BindingOptions.Instance | BindingOptions.NonVirtual | BindingOptions.Protected)]
            [InlineData(BindingOptions.Instance | BindingOptions.NonVirtual | BindingOptions.Private)]
            [InlineData(BindingOptions.Instance | BindingOptions.NonVirtual | BindingOptions.Internal)]
            public void Should_Get_Expected_Properties(BindingOptions actualOptions, BindingOptions? expectedOptions = null)
            {
                var actual = GetBindingDummyFieldNames(actualOptions);
                var expected = GetExpectedNames(expectedOptions ?? actualOptions);

                expected.Should().BeEquivalentTo(actual);
            }

            private static IReadOnlyList<string> GetBindingDummyFieldNames(BindingOptions options)
            {
                var predicate = BindingOptionsHelper.BuildFieldInfoBindingPredicate(options);

                var typeInfo = typeof(BindingDummy).GetTypeInfo();

                return
                  (from propInfo in typeInfo.GetFieldInfo(true)
                   where predicate.Invoke(propInfo)
                   select propInfo.Name
                  ).AsReadOnlyList();
            }

            private IReadOnlyList<string> GetExpectedNames(BindingOptions options)
            {
                var scope = PredicateBuilder.False<FieldMetadata>();

                if (options.HasFlag(BindingOptions.Static))
                {
                    scope = scope.Or(item => item.IsStatic);
                }

                if (options.HasFlag(BindingOptions.Instance))
                {
                    scope = scope.Or(item => item.IsInstance);
                }

                var access = PredicateBuilder.False<FieldMetadata>();

                //if (options.HasFlag(BindingOptions.Abstract))
                //{
                //    access = access.Or(item => item.IsAbstract);
                //}

                //if (options.HasFlag(BindingOptions.Virtual))
                //{
                //    access = access.Or(item => item.IsVirtual);
                //}

                if (options.HasFlag(BindingOptions.NonVirtual))
                {
                    access = access.Or(item => item.IsNonVirtual);
                }

                var visibility = PredicateBuilder.False<FieldMetadata>();

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
                  (from kvp in _fieldMetadata
                   where predicate.Invoke(kvp.Value)
                   select kvp.Key
                  ).AsReadOnlyList();
            }
        }
    }
}