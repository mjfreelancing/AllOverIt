﻿using AllOverIt.Fixture;
using AllOverIt.Reflection;
using FluentAssertions;
using System.Reflection;

namespace AllOverIt.Tests.Reflection
{
    public partial class ReflectionCacheFixture : FixtureBase
    {
        private class DummyPropertyMethodBaseClass
        {
            public int Prop1 { get; set; }
            public string Prop2 { get; set; }
            public virtual double Prop3 { get; set; }

            // Used by GetMethodInfo() tests
            public void Method1()
            {
            }

            // Used by GetMethodInfo() tests
            private void Method2()
            {
            }
        }

        private class DummyPropertyMethodSuperClass : DummyPropertyMethodBaseClass
        {
            private readonly int _value;
            public override double Prop3 { get; set; }
            private long Prop4 { get; set; }
            public bool Prop5 { set { _ = value; } }    // automatic properties must have a getter
            public bool Prop6 { get; }

            public int Field1;

            public DummyPropertyMethodSuperClass()
            {
            }

            public DummyPropertyMethodSuperClass(int value)
            {
                _value = value;
            }

            // Used by GetMethodInfo() tests
            public void Method3()
            {
            }

            // Used by GetMethodInfo() tests
            private void Method4()
            {
            }

            // Used by GetMethodInfo() tests
            public int Method5()
            {
                return _value;
            }

            // Used by GetMethodInfo() tests
            public int Method6(int arg)
            {
                return arg;
            }
        }

        public class GetPropertyInfo_Typed_PropertyName : ReflectionCacheFixture
        {
            [Fact]
            public void Should_Get_Property_In_Super()
            {
                var actual = (object) ReflectionCache.GetPropertyInfo<DummyPropertyMethodSuperClass>("Prop3");

                var expected = new
                {
                    Name = "Prop3",
                    PropertyType = typeof(double)
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_Property_In_Base()
            {
                var actual = (object) ReflectionCache.GetPropertyInfo<DummyPropertyMethodSuperClass>("Prop1");

                var expected = new
                {
                    Name = "Prop1",
                    PropertyType = typeof(int)
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Not_Find_Property()
            {
                var actual = (object) ReflectionCache.GetPropertyInfo<DummyPropertyMethodSuperClass>("PropXYZ");

                actual.Should().BeNull();
            }
        }

        public class GetPropertyInfo_Type_PropertyName : ReflectionCacheFixture
        {
            [Fact]
            public void Should_Get_Property_In_Super()
            {
                var actual = (object) ReflectionCache.GetPropertyInfo(typeof(DummyPropertyMethodSuperClass), "Prop3");

                var expected = new
                {
                    Name = "Prop3",
                    PropertyType = typeof(double)
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_Property_In_Base()
            {
                var actual = (object) ReflectionCache.GetPropertyInfo(typeof(DummyPropertyMethodSuperClass), "Prop1");

                var expected = new
                {
                    Name = "Prop1",
                    PropertyType = typeof(int)
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Not_Find_Property()
            {
                var actual = (object) ReflectionCache.GetPropertyInfo(typeof(DummyPropertyMethodSuperClass), "PropXYZ");

                actual.Should().BeNull();
            }
        }

        public class GetPropertyInfo_TypeInfo_PropertyName : ReflectionCacheFixture
        {
            [Fact]
            public void Should_Get_Property_In_Super()
            {
                var typeInfo = typeof(DummyPropertyMethodSuperClass).GetTypeInfo();
                var actual = (object) ReflectionCache.GetPropertyInfo(typeInfo, "Prop3");

                var expected = new
                {
                    Name = "Prop3",
                    PropertyType = typeof(double)
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_Property_In_Base()
            {
                var typeInfo = typeof(DummyPropertyMethodSuperClass).GetTypeInfo();
                var actual = (object) ReflectionCache.GetPropertyInfo(typeInfo, "Prop1");

                var expected = new
                {
                    Name = "Prop1",
                    PropertyType = typeof(int)
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Not_Find_Property()
            {
                var typeInfo = typeof(DummyPropertyMethodSuperClass).GetTypeInfo();
                var actual = (object) ReflectionCache.GetPropertyInfo(typeInfo, "PropXYZ");

                actual.Should().BeNull();
            }
        }

        public class GetPropertyInfo_Typed_Bindings : ReflectionCacheFixture
        {
            [Fact]
            public void Should_Use_Default_Binding_Not_Declared_Only()
            {
                var actual = ReflectionCache.GetPropertyInfo<DummyPropertyMethodSuperClass>();

                var expected = new[]
                {
                    new
                    {
                        Name = "Prop1",
                        PropertyType = typeof(int)
                    },
                    new
                    {
                        Name = "Prop2",
                        PropertyType = typeof(string)
                    },
                    new
                    {
                        Name = "Prop3",
                        PropertyType = typeof(double)
                    },
                    new
                    {
                        Name = nameof(DummyPropertyMethodSuperClass.Prop6),
                        PropertyType = typeof(bool)
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Use_Default_Binding_Declared_Only()
            {
                var actual = ReflectionCache.GetPropertyInfo<DummyPropertyMethodSuperClass>(BindingOptions.Default, true);

                var expected = new[]
                {
                    new
                    {
                        Name = nameof(DummyPropertyMethodSuperClass.Prop3),
                        PropertyType = typeof(double)
                    },
                    new
                    {
                        Name = nameof(DummyPropertyMethodSuperClass.Prop6),
                        PropertyType = typeof(bool)
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Include_Private_Property()
            {
                var binding = BindingOptions.DefaultScope | BindingOptions.Private | BindingOptions.DefaultAccessor | BindingOptions.DefaultVisibility;

                var actual = ReflectionCache.GetPropertyInfo<DummyPropertyMethodSuperClass>(binding, false);

                actual.Single(item => item.Name == "Prop4").Should().NotBeNull();
            }
        }

        public class GetPropertyInfo_Type_Bindings : ReflectionCacheFixture
        {
            [Fact]
            public void Should_Use_Default_Binding_Not_Declared_Only()
            {
                var actual = ReflectionCache.GetPropertyInfo(typeof(DummyPropertyMethodSuperClass));

                var expected = new[]
                {
                    new
                    {
                        Name = "Prop1",
                        PropertyType = typeof(int)
                    },
                    new
                    {
                        Name = "Prop2",
                        PropertyType = typeof(string)
                    },
                    new
                    {
                        Name = "Prop3",
                        PropertyType = typeof(double)
                    },
                    new
                    {
                        Name = nameof(DummyPropertyMethodSuperClass.Prop6),
                        PropertyType = typeof(bool)
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Use_Default_Binding_Declared_Only()
            {
                var actual = ReflectionCache.GetPropertyInfo(typeof(DummyPropertyMethodSuperClass), BindingOptions.Default, true);

                var expected = new[]
                {
                    new
                    {
                        Name = nameof(DummyPropertyMethodSuperClass.Prop3),
                        PropertyType = typeof(double)
                    },
                    new
                    {
                        Name = nameof(DummyPropertyMethodSuperClass.Prop6),
                        PropertyType = typeof(bool)
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Include_Private_Property()
            {
                var binding = BindingOptions.DefaultScope | BindingOptions.Private | BindingOptions.DefaultAccessor | BindingOptions.DefaultVisibility;

                var actual = ReflectionCache.GetPropertyInfo(typeof(DummyPropertyMethodSuperClass), binding, false);

                actual.Single(item => item.Name == "Prop4").Should().NotBeNull();
            }
        }

        public class GetPropertyInfo_TypeInfo : ReflectionCacheFixture
        {
            [Fact]
            public void Should_Get_Declared_Only()
            {
                var typeInfo = typeof(DummyPropertyMethodSuperClass).GetTypeInfo();
                var actual = ReflectionCache.GetPropertyInfo(typeInfo, true);

                var expected = new[]
                {
                    new
                    {
                        Name = nameof(DummyPropertyMethodSuperClass.Prop3),
                        PropertyType = typeof(double)
                    },
                    new
                    {
                        Name = "Prop4",                     // This is private
                        PropertyType = typeof(long)
                    },
                    new
                    {
                        Name = nameof(DummyPropertyMethodSuperClass.Prop5),
                        PropertyType = typeof(bool)
                    },
                    new
                    {
                        Name = nameof(DummyPropertyMethodSuperClass.Prop6),
                        PropertyType = typeof(bool)
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_All_Properties()
            {
                var typeInfo = typeof(DummyPropertyMethodSuperClass).GetTypeInfo();
                var actual = ReflectionCache.GetPropertyInfo(typeInfo, false);

                var expected = new[]
                {
                    new
                    {
                        Name = nameof(DummyPropertyMethodBaseClass.Prop1),
                        PropertyType = typeof(int)
                    },
                    new
                    {
                        Name = nameof(DummyPropertyMethodBaseClass.Prop2),
                        PropertyType = typeof(string)
                    },
                    new
                    {
                        Name = nameof(DummyPropertyMethodSuperClass.Prop3),
                        PropertyType = typeof(double)
                    },
                    new
                    {
                        Name = "Prop4",                     // This is private
                        PropertyType = typeof(long)
                    },
                    new
                    {
                        Name = nameof(DummyPropertyMethodSuperClass.Prop5),
                        PropertyType = typeof(bool)
                    },
                    new
                    {
                        Name = nameof(DummyPropertyMethodSuperClass.Prop6),
                        PropertyType = typeof(bool)
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }
        }
    }
}
