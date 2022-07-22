using AllOverIt.Fixture;
using AllOverIt.Reflection;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AllOverIt.Tests.Reflection
{
    public partial class ReflectionCacheFixture : FixtureBase
    {
        public class GetMethodInfo_Typed : ReflectionCacheFixture
        {
            private readonly string[] _knownMethods = new[] { "Method1", "Method2", "Method3", "Method4" };

            // GetMethod() returns methods of object as well as property get/set methods, so these tests filter down to expected (non-property) methods in the dummy classes

            [Fact]
            public void Should_Use_Default_Binding_Not_Declared_Only()
            {
                var actual = ReflectionCache.GetMethodInfo<DummyPropertyMethodSuperClass>()
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
                        DeclaringType = typeof(DummyPropertyMethodBaseClass)
                    },
                    new
                    {
                        Name = "Method3",
                        DeclaringType = typeof(DummyPropertyMethodSuperClass)
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Use_Default_Binding_Declared_Only()
            {
                var actual = ReflectionCache.GetMethodInfo<DummyPropertyMethodSuperClass>(BindingOptions.Default, true)
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
                        DeclaringType = typeof(DummyPropertyMethodSuperClass)
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_All_Base_Methods_Only()
            {
                var actual = ReflectionCache.GetMethodInfo<DummyPropertyMethodBaseClass>(BindingOptions.All, true)
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
                        DeclaringType = typeof(DummyPropertyMethodBaseClass)
                    },
                    new
                    {
                        Name = "Method2",
                        DeclaringType = typeof(DummyPropertyMethodBaseClass)
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_All_Super_Methods_Only()
            {
                var actual = ReflectionCache.GetMethodInfo<DummyPropertyMethodSuperClass>(BindingOptions.All, true)
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
                        DeclaringType = typeof(DummyPropertyMethodSuperClass)
                    },
                    new
                    {
                        Name = "Method4",
                        DeclaringType = typeof(DummyPropertyMethodSuperClass)
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_Private_Methods_Only()
            {
                var actual = ReflectionCache.GetMethodInfo<DummyPropertyMethodSuperClass>(BindingOptions.Private, false)   // default scope and visibility is implied
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
                        DeclaringType = typeof(DummyPropertyMethodBaseClass)
                    },
                    new
                    {
                        Name = "Method4",
                        DeclaringType = typeof(DummyPropertyMethodSuperClass)
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }
        }
        
        public class GetMethodInfo_Type : ReflectionCacheFixture
        {
            private readonly string[] _knownMethods = new[] { "Method1", "Method2", "Method3", "Method4" };

            // GetMethod() returns methods of object as well as property get/set methods, so these tests filter down to expected (non-property) methods in the dummy classes

            [Fact]
            public void Should_Use_Default_Binding_Not_Declared_Only()
            {
                var actual = ReflectionCache.GetMethodInfo(typeof(DummyPropertyMethodSuperClass))
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
                        DeclaringType = typeof(DummyPropertyMethodBaseClass)
                    },
                    new
                    {
                        Name = "Method3",
                        DeclaringType = typeof(DummyPropertyMethodSuperClass)
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Use_Default_Binding_Declared_Only()
            {
                var actual = ReflectionCache.GetMethodInfo(typeof(DummyPropertyMethodSuperClass), BindingOptions.Default, true)
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
                        DeclaringType = typeof(DummyPropertyMethodSuperClass)
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_All_Base_Methods_Only()
            {
                var actual = ReflectionCache.GetMethodInfo(typeof(DummyPropertyMethodBaseClass), BindingOptions.All, true)
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
                        DeclaringType = typeof(DummyPropertyMethodBaseClass)
                    },
                    new
                    {
                        Name = "Method2",
                        DeclaringType = typeof(DummyPropertyMethodBaseClass)
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_All_Super_Methods_Only()
            {
                var actual = ReflectionCache.GetMethodInfo(typeof(DummyPropertyMethodSuperClass), BindingOptions.All, true)
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
                        DeclaringType = typeof(DummyPropertyMethodSuperClass)
                    },
                    new
                    {
                        Name = "Method4",
                        DeclaringType = typeof(DummyPropertyMethodSuperClass)
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_Private_Methods_Only()
            {
                var actual = ReflectionCache.GetMethodInfo(typeof(DummyPropertyMethodSuperClass), BindingOptions.Private, false)   // default scope and visibility is implied
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
                        DeclaringType = typeof(DummyPropertyMethodBaseClass)
                    },
                    new
                    {
                        Name = "Method4",
                        DeclaringType = typeof(DummyPropertyMethodSuperClass)
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }
        }

        public class GetMethodInfo_Typed_Name : ReflectionCacheFixture
        {
            [Fact]
            public void Should_Not_Find_Method()
            {
                var actual = ReflectionCache.GetMethodInfo<DummyPropertyMethodSuperClass>(Create<string>());

                actual.Should().BeNull();
            }

            [Fact]
            public void Should_Find_Method_With_No_Arguments()
            {
                var actual = ReflectionCache.GetMethodInfo<DummyPropertyMethodSuperClass>("Method5");

                actual.Should().NotBeNull();

                // make sure the correct overload was chosen
                var expected = Create<int>();
                var dummy = new DummyPropertyMethodSuperClass(expected);

                var value = actual.Invoke(dummy, null);

                value.Should().Be(expected);
            }
        }

        public class GetMethodInfo_Type_Name : ReflectionCacheFixture
        {
            [Fact]
            public void Should_Not_Find_Method()
            {
                var actual = ReflectionCache.GetMethodInfo(typeof(DummyPropertyMethodSuperClass), Create<string>());

                actual.Should().BeNull();
            }

            [Fact]
            public void Should_Find_Method_With_No_Arguments()
            {
                var actual = ReflectionCache.GetMethodInfo(typeof(DummyPropertyMethodSuperClass), "Method5");

                actual.Should().NotBeNull();

                // make sure the correct overload was chosen
                var expected = Create<int>();
                var dummy = new DummyPropertyMethodSuperClass(expected);

                var value = actual.Invoke(dummy, null);

                value.Should().Be(expected);
            }
        }

        public class GetMethodInfo_Typed_Name_And_Args : ReflectionCacheFixture
        {
            [Fact]
            public void Should_Not_Find_Method()
            {
                var actual = ReflectionCache.GetMethodInfo<DummyPropertyMethodSuperClass>(Create<string>(), Type.EmptyTypes);

                actual.Should().BeNull();
            }

            [Fact]
            public void Should_Find_Method_With_No_Arguments()
            {
                var actual = ReflectionCache.GetMethodInfo<DummyPropertyMethodSuperClass>("Method5", Type.EmptyTypes);

                actual.Should().NotBeNull();

                // make sure the correct overload was chosen
                var expected = Create<int>();
                var dummy = new DummyPropertyMethodSuperClass(expected);

                var value = actual.Invoke(dummy, null);

                value.Should().Be(expected);
            }

            [Fact]
            public void Should_Find_Method_With_Specific_Arguments()
            {
                var actual = ReflectionCache.GetMethodInfo<DummyPropertyMethodSuperClass>("Method6", new[] { typeof(int) });

                actual.Should().NotBeNull();

                // make sure the correct overload was chosen
                var expected = Create<int>();
                var dummy = new DummyPropertyMethodSuperClass();

                var value = actual.Invoke(dummy, new object[] { expected });

                value.Should().Be(expected);
            }
        }

        public class GetMethodInfo_Type_Name_And_Args : ReflectionCacheFixture
        {
            [Fact]
            public void Should_Not_Find_Method()
            {
                var actual = ReflectionCache.GetMethodInfo(typeof(DummyPropertyMethodSuperClass), Create<string>(), Type.EmptyTypes);

                actual.Should().BeNull();
            }

            [Fact]
            public void Should_Find_Method_With_No_Arguments()
            {
                var actual = ReflectionCache.GetMethodInfo(typeof(DummyPropertyMethodSuperClass), "Method5", Type.EmptyTypes);

                actual.Should().NotBeNull();

                // make sure the correct overload was chosen
                var expected = Create<int>();
                var dummy = new DummyPropertyMethodSuperClass(expected);

                var value = actual.Invoke(dummy, null);

                value.Should().Be(expected);
            }

            [Fact]
            public void Should_Find_Method_With_Specific_Arguments()
            {
                var actual = ReflectionCache.GetMethodInfo(typeof(DummyPropertyMethodSuperClass), "Method6", new[] { typeof(int) });

                actual.Should().NotBeNull();

                // make sure the correct overload was chosen
                var expected = Create<int>();
                var dummy = new DummyPropertyMethodSuperClass();

                var value = actual.Invoke(dummy, new object[] { expected });

                value.Should().Be(expected);
            }
        }
    }
}
