﻿using AllOverIt.Fixture;
using AllOverIt.Reflection;
using FluentAssertions;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace AllOverIt.Tests.Reflection
{
    public class ReflectionHelperFixture : FixtureBase
    {
        private class DummyBaseClass
        {
            public int Prop1 { get; set; }
            public string Prop2 { get; set; }
            public virtual double Prop3 { get; set; }

            public void Method1()
            {
            }

            private void Method2()
            {
            }
        }

        private class DummySuperClass : DummyBaseClass
        {
            public override double Prop3 { get; set; }
            private long Prop4 { get; set; }

#pragma warning disable 649
            // Unassigned field
            public int Field1;
#pragma warning restore 649

            public void Method3()
            {
            }

            private void Method4()
            {
            }
        }

        private class DummyMemberInfo : MemberInfo
        {
            public override object[] GetCustomAttributes(bool inherit)
            {
                throw new NotImplementedException();
            }

            public override object[] GetCustomAttributes(Type attributeType, bool inherit)
            {
                throw new NotImplementedException();
            }

            public override bool IsDefined(Type attributeType, bool inherit)
            {
                throw new NotImplementedException();
            }

#pragma warning disable 8632
            // Annotation for nullable reference types should only be used in code within a #nullable annotations context
            // ReSharper disable once UnassignedGetOnlyAutoProperty
            public override Type? DeclaringType { get; }
#pragma warning restore 8632

            // ReSharper disable once UnassignedGetOnlyAutoProperty
            public override MemberTypes MemberType { get; }

            public override string Name { get; }

#pragma warning disable 8632
            // Annotation for nullable reference types should only be used in code within a #nullable annotations context
            // ReSharper disable once UnassignedGetOnlyAutoProperty
            public override Type? ReflectedType { get; }
#pragma warning restore 8632

        }

        public class GetPropertyInfo_Property : ReflectionHelperFixture
        {
            [Fact]
            public void Should_Get_Property_In_Super()
            {
                var actual = (object)ReflectionHelper.GetPropertyInfo<DummySuperClass>("Prop3");

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
                var actual = (object)ReflectionHelper.GetPropertyInfo<DummySuperClass>("Prop1");

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
                var actual = (object)ReflectionHelper.GetPropertyInfo<DummySuperClass>("PropXYZ");

                actual.Should().BeNull();
            }
        }

        public class GetPropertyInfo_Bindings : ReflectionHelperFixture
        {
            [Fact]
            public void Should_Use_Default_Binding_Not_Declared_Only()
            {
                var actual = ReflectionHelper.GetPropertyInfo<DummySuperClass>();

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
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Use_Default_Binding_Declared_Only()
            {
                var actual = ReflectionHelper.GetPropertyInfo<DummySuperClass>(BindingOptions.Default, true);

                var expected = new[]
                {
                    new
                    {
                        Name = "Prop3",
                        PropertyType = typeof(double)
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Include_Private_Property()
            {
                var binding = BindingOptions.DefaultScope | BindingOptions.Private | BindingOptions.DefaultAccessor | BindingOptions.DefaultVisibility;

                var actual = ReflectionHelper.GetPropertyInfo<DummySuperClass>(binding, false);

                actual.Single(item => item.Name == "Prop4").Should().NotBeNull();
            }
        }

        public class GetMethodInfo : ReflectionHelperFixture
        {
            private readonly string[] _knownMethods = new[] { "Method1", "Method2", "Method3", "Method4" };

            // GetMethod() returns methods of object as well as property get/set methods, so these tests filter down to expected (non-property) methods in the dummy classes

            [Fact]
            public void Should_Use_Default_Binding_Not_Declared_Only()
            {
                var actual = ReflectionHelper.GetMethodInfo<DummySuperClass>()
                  .Where(item => _knownMethods.Contains(item.Name))
                  .Select(item => new
                  {
                      item.Name,
                      item.DeclaringType
                  });

                var expected = new[]
                {
                    new
                    {
                        Name = "Method1",
                        DeclaringType = typeof(DummyBaseClass)
                    },
                    new
                    {
                        Name = "Method3",
                        DeclaringType = typeof(DummySuperClass)
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Use_Default_Binding_Declared_Only()
            {
                var actual = ReflectionHelper.GetMethodInfo<DummySuperClass>(BindingOptions.Default, true)
                  .Where(item => _knownMethods.Contains(item.Name))
                  .Select(item => new
                  {
                      item.Name,
                      item.DeclaringType
                  });

                var expected = new[]
                {
                    new
                    {
                        Name = "Method3",
                        DeclaringType = typeof(DummySuperClass)
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_All_Base_Methods_Only()
            {
                var actual = ReflectionHelper.GetMethodInfo<DummyBaseClass>(BindingOptions.All, true)
                  .Where(item => _knownMethods.Contains(item.Name))
                  .Select(item => new
                  {
                      item.Name,
                      item.DeclaringType
                  });

                var expected = new[]
                {
                    new
                    {
                        Name = "Method1",
                        DeclaringType = typeof(DummyBaseClass)
                    },
                    new
                    {
                        Name = "Method2",
                        DeclaringType = typeof(DummyBaseClass)
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_All_Super_Methods_Only()
            {
                var actual = ReflectionHelper.GetMethodInfo<DummySuperClass>(BindingOptions.All, true)
                  .Where(item => _knownMethods.Contains(item.Name))
                  .Select(item => new
                  {
                      item.Name,
                      item.DeclaringType
                  });

                var expected = new[]
                {
                    new
                    {
                        Name = "Method3",
                        DeclaringType = typeof(DummySuperClass)
                    },
                    new
                    {
                        Name = "Method4",
                        DeclaringType = typeof(DummySuperClass)
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_Private_Methods_Only()
            {
                var actual = ReflectionHelper.GetMethodInfo<DummySuperClass>(BindingOptions.Private, false)   // default scope and visibility is implied
                  .Where(item => _knownMethods.Contains(item.Name))
                  .Select(item => new
                  {
                      item.Name,
                      item.DeclaringType
                  });

                var expected = new[]
                {
                    new
                    {
                        Name = "Method2",
                        DeclaringType = typeof(DummyBaseClass)
                    },
                    new
                    {
                        Name = "Method4",
                        DeclaringType = typeof(DummySuperClass)
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }
        }
    }
}
