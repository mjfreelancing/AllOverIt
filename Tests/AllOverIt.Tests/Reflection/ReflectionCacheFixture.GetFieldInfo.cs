using AllOverIt.Fixture;
using AllOverIt.Shouldly;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Reflection;
using System.Linq;
using System.Reflection;
using AllOverIt.Shouldly.Extensions;

namespace AllOverIt.Tests.Reflection
{
    public partial class ReflectionCacheFixture : FixtureBase
    {
        private class DummyFieldBaseClass
        {
            public string Field2;
            private double Field5;
        }

        private class DummyFieldSuperClass : DummyFieldBaseClass
        {
            public int Field1;
            public bool Field3;
            private long Field4;

            public DummyFieldSuperClass()
            {
            }

            public DummyFieldSuperClass(long value)
            {
                Field4 = value;
            }
        }

        public class GetFieldInfo_Typed_FieldName : ReflectionCacheFixture
        {
            [Fact]
            public void Should_Throw_When_Field_Name_Null()
            {
                Invoking(() =>
                {
                    _ = ReflectionCache.GetFieldInfo<DummyFieldSuperClass>(null);
                })
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("fieldName");
            }

            [Fact]
            public void Should_Throw_When_Field_Name_Empty()
            {
                Invoking(() =>
                {
                    _ = ReflectionCache.GetFieldInfo<DummyFieldSuperClass>(string.Empty);
                })
                    .ShouldThrow<ArgumentException>()
                    .WithNamedMessageWhenEmpty("fieldName");
            }

            [Fact]
            public void Should_Throw_When_Field_Name_Whitespace()
            {
                Invoking(() =>
                {
                    _ = ReflectionCache.GetFieldInfo<DummyFieldSuperClass>("   ");
                })
                    .ShouldThrow<ArgumentException>()
                    .WithNamedMessageWhenEmpty("fieldName");
            }

            [Fact]
            public void Should_Get_Field_In_Super()
            {
                var actual = ReflectionCache.GetFieldInfo<DummyFieldSuperClass>("Field4");

                var expected = new
                {
                    Name = "Field4",
                    FieldType = typeof(long)
                };

                new { actual.Name, actual.FieldType }.ShouldBeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_Field_In_Base()
            {
                var actual = ReflectionCache.GetFieldInfo<DummyFieldSuperClass>("Field2");

                var expected = new
                {
                    Name = "Field2",
                    FieldType = typeof(string)
                };

                new { actual.Name, actual.FieldType }.ShouldBeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Not_Find_Field()
            {
                var actual = (object) ReflectionCache.GetFieldInfo<DummyFieldSuperClass>("PropXYZ");

                actual.ShouldBeNull();
            }
        }

        public class GetFieldInfo_Type_FieldName : ReflectionCacheFixture
        {
            [Fact]
            public void Should_Throw_When_Type_Null()
            {
                Invoking(() =>
                {
                    _ = ReflectionCache.GetFieldInfo((Type) null, Create<string>());
                })
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("type");
            }

            [Fact]
            public void Should_Throw_When_Field_Name_Null()
            {
                Invoking(() =>
                {
                    _ = ReflectionCache.GetFieldInfo(typeof(DummyFieldSuperClass), null);
                })
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("fieldName");
            }

            [Fact]
            public void Should_Throw_When_Field_Name_Empty()
            {
                Invoking(() =>
                {
                    _ = ReflectionCache.GetFieldInfo(typeof(DummyFieldSuperClass), string.Empty);
                })
                    .ShouldThrow<ArgumentException>()
                    .WithNamedMessageWhenEmpty("fieldName");
            }

            [Fact]
            public void Should_Throw_When_Field_Name_Whitespace()
            {
                Invoking(() =>
                {
                    _ = ReflectionCache.GetFieldInfo(typeof(DummyFieldSuperClass), "   ");
                })
                    .ShouldThrow<ArgumentException>()
                    .WithNamedMessageWhenEmpty("fieldName");
            }

            [Fact]
            public void Should_Get_Field_In_Super()
            {
                var actual = ReflectionCache.GetFieldInfo(typeof(DummyFieldSuperClass), "Field4");

                var expected = new
                {
                    Name = "Field4",
                    FieldType = typeof(long)
                };

                new { actual.Name, actual.FieldType }.ShouldBeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_Field_In_Base()
            {
                var actual = ReflectionCache.GetFieldInfo(typeof(DummyFieldSuperClass), "Field2");

                var expected = new
                {
                    Name = "Field2",
                    FieldType = typeof(string)
                };

                new { actual.Name, actual.FieldType }.ShouldBeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Not_Find_Field()
            {
                var actual = (object) ReflectionCache.GetFieldInfo(typeof(DummyFieldSuperClass), "PropXYZ");

                actual.ShouldBeNull();
            }
        }

        public class GetFieldInfo_TypeInfo_FieldName : ReflectionCacheFixture
        {
            [Fact]
            public void Should_Throw_When_TypeInfo_Null()
            {
                Invoking(() =>
                {
                    _ = ReflectionCache.GetFieldInfo((TypeInfo) null, Create<string>());
                })
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("typeInfo");
            }

            [Fact]
            public void Should_Throw_When_Field_Name_Null()
            {
                Invoking(() =>
                {
                    var typeInfo = typeof(DummyFieldSuperClass).GetTypeInfo();

                    _ = ReflectionCache.GetFieldInfo(typeInfo, null);
                })
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("fieldName");
            }

            [Fact]
            public void Should_Throw_When_Field_Name_Empty()
            {
                Invoking(() =>
                {
                    var typeInfo = typeof(DummyFieldSuperClass).GetTypeInfo();

                    _ = ReflectionCache.GetFieldInfo(typeInfo, string.Empty);
                })
                    .ShouldThrow<ArgumentException>()
                    .WithNamedMessageWhenEmpty("fieldName");
            }

            [Fact]
            public void Should_Throw_When_Field_Name_Whitespace()
            {
                Invoking(() =>
                {
                    var typeInfo = typeof(DummyFieldSuperClass).GetTypeInfo();

                    _ = ReflectionCache.GetFieldInfo(typeInfo, "   ");
                })
                    .ShouldThrow<ArgumentException>()
                    .WithNamedMessageWhenEmpty("fieldName");
            }

            [Fact]
            public void Should_Get_Field_In_Super()
            {
                var typeInfo = typeof(DummyFieldSuperClass).GetTypeInfo();
                var actual = ReflectionCache.GetFieldInfo(typeInfo, "Field3");

                var expected = new
                {
                    Name = "Field3",
                    FieldType = typeof(bool)
                };

                new { actual.Name, actual.FieldType }.ShouldBeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_Field_In_Base()
            {
                var typeInfo = typeof(DummyFieldSuperClass).GetTypeInfo();
                var actual = ReflectionCache.GetFieldInfo(typeInfo, "Field1");

                var expected = new
                {
                    Name = "Field1",
                    FieldType = typeof(int)
                };

                new { actual.Name, actual.FieldType }.ShouldBeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Not_Find_Field()
            {
                var typeInfo = typeof(DummyFieldSuperClass).GetTypeInfo();
                var actual = (object) ReflectionCache.GetFieldInfo(typeInfo, "PropXYZ");

                actual.ShouldBeNull();
            }
        }

        public class GetFieldInfo_Typed_Bindings : ReflectionCacheFixture
        {
            [Fact]
            public void Should_Use_Default_Binding_Not_Declared_Only()
            {
                var actual = ReflectionCache.GetFieldInfo<DummyFieldSuperClass>()
                    .Select(item => new { item.Name, item.FieldType });

                var expected = new[]
                {
                    new
                    {
                        Name = "Field1",
                        FieldType = typeof(int)
                    },
                    new
                    {
                        Name = "Field2",
                        FieldType = typeof(string)
                    },
                    new
                    {
                        Name = "Field3",
                        FieldType = typeof(bool)
                    }
                };

                actual.ShouldBeEquivalentTo(expected, options => options.SequenceOrdering = SequenceOrdering.AnyOrder);
            }

            [Fact]
            public void Should_Use_Default_Binding_Declared_Only()
            {
                var actual = ReflectionCache.GetFieldInfo<DummyFieldSuperClass>(BindingOptions.Default, true)
                    .Select(item => new { item.Name, item.FieldType });

                var expected = new[]
                {
                     new
                    {
                        Name = "Field1",
                        FieldType = typeof(int)
                    },
                    new
                    {
                        Name = "Field3",
                        FieldType = typeof(bool)
                    }
                };

                actual.ShouldBeEquivalentTo(expected, options => options.SequenceOrdering = SequenceOrdering.AnyOrder);
            }

            [Fact]
            public void Should_Include_Private_Field()
            {
                var binding = BindingOptions.DefaultScope | BindingOptions.Private | BindingOptions.DefaultAccessor | BindingOptions.DefaultVisibility;

                var actual = ReflectionCache.GetFieldInfo<DummyFieldSuperClass>(binding, false);

                actual.Single(item => item.Name == "Field5").ShouldNotBeNull();
            }
        }

        public class GetFieldInfo_Type_Bindings : ReflectionCacheFixture
        {
            [Fact]
            public void Should_Throw_When_Type_Null()
            {
                Invoking(() =>
                {
                    _ = ReflectionCache.GetFieldInfo((Type) null, BindingOptions.Default);
                })
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("type");
            }

            [Fact]
            public void Should_Use_Default_Binding_Not_Declared_Only()
            {
                var actual = ReflectionCache.GetFieldInfo(typeof(DummyFieldSuperClass))
                    .Select(item => new { item.Name, item.FieldType });

                var expected = new[]
                {
                    new
                    {
                        Name = "Field1",
                        FieldType = typeof(int)
                    },
                    new
                    {
                        Name = "Field2",
                        FieldType = typeof(string)
                    },
                    new
                    {
                        Name = "Field3",
                        FieldType = typeof(bool)
                    }
                };

                actual.ShouldBeEquivalentTo(expected, options => options.SequenceOrdering = SequenceOrdering.AnyOrder);
            }

            [Fact]
            public void Should_Use_Default_Binding_Declared_Only()
            {
                var actual = ReflectionCache.GetFieldInfo(typeof(DummyFieldSuperClass), BindingOptions.Default, true)
                    .Select(item => new { item.Name, item.FieldType });

                var expected = new[]
                {
                     new
                    {
                        Name = "Field1",
                        FieldType = typeof(int)
                    },
                    new
                    {
                        Name = "Field3",
                        FieldType = typeof(bool)
                    }
                };

                actual.ShouldBeEquivalentTo(expected, options => options.SequenceOrdering = SequenceOrdering.AnyOrder);
            }

            [Fact]
            public void Should_Include_Private_Field()
            {
                var binding = BindingOptions.DefaultScope | BindingOptions.Private | BindingOptions.DefaultAccessor | BindingOptions.DefaultVisibility;

                var actual = ReflectionCache.GetFieldInfo(typeof(DummyFieldSuperClass), binding, false);

                actual.Single(item => item.Name == "Field5").ShouldNotBeNull();
            }
        }

        public class GetFieldInfo_TypeInfo : ReflectionCacheFixture
        {
            [Fact]
            public void Should_Throw_When_TypeInfo_Null()
            {
                Invoking(() =>
                {
                    _ = ReflectionCache.GetFieldInfo((TypeInfo) null);
                })
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("typeInfo");
            }

            [Fact]
            public void Should_Get_Declared_Only()
            {
                var typeInfo = typeof(DummyFieldSuperClass).GetTypeInfo();
                var actual = ReflectionCache.GetFieldInfo(typeInfo, true)
                    .Select(item => new { item.Name, item.FieldType });

                var expected = new[]
                {
                    new
                    {
                        Name = "Field1",
                        FieldType = typeof(int)
                    },
                    new
                    {
                        Name = "Field3",
                        FieldType = typeof(bool)
                    },
                    new
                    {
                        Name = "Field4",
                        FieldType = typeof(long)
                    }
                };

                actual.ShouldBeEquivalentTo(expected, options => options.SequenceOrdering = SequenceOrdering.AnyOrder);
            }

            [Fact]
            public void Should_Get_All_Properties()
            {
                var typeInfo = typeof(DummyFieldSuperClass).GetTypeInfo();
                var actual = ReflectionCache.GetFieldInfo(typeInfo, false)
                    .Select(item => new { item.Name, item.FieldType });

                var expected = new[]
                {
                    new
                    {
                        Name = "Field1",
                        FieldType = typeof(int)
                    },
                    new
                    {
                        Name = "Field2",
                        FieldType = typeof(string)
                    },
                    new
                    {
                        Name = "Field3",
                        FieldType = typeof(bool)
                    },
                    new
                    {
                        Name = "Field4",
                        FieldType = typeof(long)
                    },
                    new
                    {
                        Name = "Field5",
                        FieldType = typeof(double)
                    }
                };

                actual.ShouldBeEquivalentTo(expected, options => options.SequenceOrdering = SequenceOrdering.AnyOrder);
            }
        }
    }
}












