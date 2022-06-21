using AllOverIt.Fixture;
using AllOverIt.Reflection;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AllOverIt.Tests.Reflection
{
    public partial class PropertyExpressionsFixture : FixtureBase
    {
        public class GetMethodInfo : PropertyExpressionsFixture
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

        public class GetMethodInfo_Named : PropertyExpressionsFixture
        {
            [Fact]
            public void Should_Not_Find_Method()
            {
                var actual = ReflectionHelper.GetMethodInfo<DummySuperClass>(Create<string>());

                actual.Should().BeNull();
            }

            [Fact]
            public void Should_Find_Method_With_No_Arguments()
            {
                var actual = ReflectionHelper.GetMethodInfo<DummySuperClass>("Method5");

                actual.Should().NotBeNull();

                // make sure the correct overload was chosen
                var expected = Create<int>();
                var dummy = new DummySuperClass(expected);

                var value = actual.Invoke(dummy, null);

                value.Should().Be(expected);
            }
        }

        public class GetMethodInfo_Named_And_Args : PropertyExpressionsFixture
        {
            [Fact]
            public void Should_Not_Find_Method()
            {
                var actual = ReflectionHelper.GetMethodInfo<DummySuperClass>(Create<string>(), Type.EmptyTypes);

                actual.Should().BeNull();
            }

            [Fact]
            public void Should_Find_Method_With_No_Arguments()
            {
                var actual = ReflectionHelper.GetMethodInfo<DummySuperClass>("Method5", Type.EmptyTypes);

                actual.Should().NotBeNull();

                // make sure the correct overload was chosen
                var expected = Create<int>();
                var dummy = new DummySuperClass(expected);

                var value = actual.Invoke(dummy, null);

                value.Should().Be(expected);
            }

            [Fact]
            public void Should_Find_Method_With_Specific_Arguments()
            {
                var actual = ReflectionHelper.GetMethodInfo<DummySuperClass>("Method6", new[] { typeof(int) });

                actual.Should().NotBeNull();

                // make sure the correct overload was chosen
                var expected = Create<int>();
                var dummy = new DummySuperClass();

                var value = actual.Invoke(dummy, new object[] { expected });

                value.Should().Be(expected);
            }
        }
    }
}
