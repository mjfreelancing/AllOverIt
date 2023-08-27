using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Variables
{
    public class VariableRegistryBuilderFixture : FixtureBase
    {
        private readonly VariableRegistryBuilder _variableRegistryBuilder = new();

        public class Create : VariableRegistryBuilderFixture
        {
            [Fact]
            public void Should_Create_VariableRegistryBuilder()
            {
                var actual = VariableRegistryBuilder.Create();

                actual.Should().BeOfType<VariableRegistryBuilder>();
            }
        }

        public class AddConstantVariable : VariableRegistryBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddConstantVariable(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddConstantVariable(string.Empty);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddConstantVariable("  ");
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Add_Variable()
            {
                var name = Create<string>();
                var value = Create<double>();

                _ = _variableRegistryBuilder.AddConstantVariable(name, value);

                var registry = _variableRegistryBuilder.Build();

                registry.GetVariable(name).Should().BeOfType<ConstantVariable>();

                registry
                    .GetValue(name)
                    .Should()
                    .Be(value);
            }
        }

        public class AddConstantVariable_Registry_Resolver : VariableRegistryBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddConstantVariable(null, registry => Create<double>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddConstantVariable(string.Empty, registry => Create<double>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddConstantVariable("  ", registry => Create<double>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Resolver_Null()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddConstantVariable(Create<string>(), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("valueResolver");
            }

            [Fact]
            public void Should_Add_Variable()
            {
                var name = Create<string>();
                var value = Create<double>();

                _ = _variableRegistryBuilder.AddConstantVariable(name, registry => value);

                var registry = _variableRegistryBuilder.Build();

                registry.GetVariable(name).Should().BeOfType<ConstantVariable>();

                registry
                    .GetValue(name)
                    .Should()
                    .Be(value);
            }

            [Fact]
            public void Should_Provide_Registry()
            {
                var name = Create<string>();
                IVariableRegistry actual = null;

                _ = _variableRegistryBuilder.AddConstantVariable(name, registry =>
                {
                    actual = registry;

                    return Create<double>();
                });

                var expected = _variableRegistryBuilder.Build();

                _ = expected.GetValue(name);

                actual.Should().BeSameAs(expected);
            }
        }

        public class AddDelegateVariable_Resolver : VariableRegistryBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddDelegateVariable(null, () => Create<double>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddDelegateVariable(string.Empty, () => Create<double>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddDelegateVariable("  ", () => Create<double>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Resolver_Null()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddDelegateVariable(Create<string>(), (Func<double>)null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("valueResolver");
            }

            [Fact]
            public void Should_Add_Variable()
            {
                var name = Create<string>();
                var value = Create<double>();

                _ = _variableRegistryBuilder.AddDelegateVariable(name, () => value);

                var registry = _variableRegistryBuilder.Build();

                registry.GetVariable(name).Should().BeOfType<DelegateVariable>();

                registry
                    .GetValue(name)
                    .Should()
                    .Be(value);
            }
        }

        public class AddDelegateVariable_Registry_Resolver : VariableRegistryBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddDelegateVariable(null, registry => Create<double>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddDelegateVariable(string.Empty, registry => Create<double>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddDelegateVariable("  ", registry => Create<double>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Resolver_Null()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddDelegateVariable(Create<string>(), (Func<IVariableRegistry, double>) null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("valueResolver");
            }

            [Fact]
            public void Should_Add_Variable()
            {
                var name = Create<string>();
                var value = Create<double>();

                _ = _variableRegistryBuilder.AddDelegateVariable(name, registry => value);

                var registry = _variableRegistryBuilder.Build();

                registry.GetVariable(name).Should().BeOfType<DelegateVariable>();

                registry
                    .GetValue(name)
                    .Should()
                    .Be(value);
            }

            [Fact]
            public void Should_Provide_Registry()
            {
                var name = Create<string>();
                IVariableRegistry actual = null;

                _ = _variableRegistryBuilder.AddDelegateVariable(name, registry =>
                {
                    actual = registry;

                    return Create<double>();
                });

                var expected = _variableRegistryBuilder.Build();

                _ = expected.GetValue(name);

                actual.Should().BeSameAs(expected);
            }
        }

        public class AddDelegateVariable_FormulaCompilerResult : VariableRegistryBuilderFixture
        {
            private readonly double _value;
            private readonly FormulaCompilerResult _formulaCompilerResult;

            public AddDelegateVariable_FormulaCompilerResult()
            {
                var formulaCompiler = new FormulaCompiler();
                _formulaCompilerResult = formulaCompiler.Compile($"{_value}");
            }

            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddDelegateVariable(null, _formulaCompilerResult);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddDelegateVariable(string.Empty, _formulaCompilerResult);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddDelegateVariable("  ", _formulaCompilerResult);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Resolver_Null()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddDelegateVariable(Create<string>(), (FormulaCompilerResult) null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("formulaCompilerResultResolver");
            }

            [Fact]
            public void Should_Add_Variable()
            {
                var name = Create<string>();

                _ = _variableRegistryBuilder.AddDelegateVariable(name, _formulaCompilerResult);

                var registry = _variableRegistryBuilder.Build();

                registry.GetVariable(name).Should().BeOfType<DelegateVariable>();

                registry
                    .GetValue(name)
                    .Should()
                    .Be(_value);
            }
        }

        public class AddDelegateVariable_Registry_FormulaCompilerResult : VariableRegistryBuilderFixture
        {
            private readonly double _value;
            private readonly FormulaCompilerResult _formulaCompilerResult;

            public AddDelegateVariable_Registry_FormulaCompilerResult()
            {
                var formulaCompiler = new FormulaCompiler();
                _formulaCompilerResult = formulaCompiler.Compile($"{_value}");
            }

            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddDelegateVariable(null, registry => _formulaCompilerResult);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddDelegateVariable(string.Empty, registry => _formulaCompilerResult);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddDelegateVariable("  ", registry => _formulaCompilerResult);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Resolver_Null()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddDelegateVariable(Create<string>(), (Func<IVariableRegistry, FormulaCompilerResult>) null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("formulaCompilerResultResolver");
            }

            [Fact]
            public void Should_Add_Variable()
            {
                var name = Create<string>();

                _ = _variableRegistryBuilder.AddDelegateVariable(name, registry => _formulaCompilerResult);

                var registry = _variableRegistryBuilder.Build();

                registry.GetVariable(name).Should().BeOfType<DelegateVariable>();

                registry
                    .GetValue(name)
                    .Should()
                    .Be(_value);
            }

            [Fact]
            public void Should_Provide_Registry()
            {
                var name = Create<string>();
                IVariableRegistry actual = null;

                _ = _variableRegistryBuilder.AddDelegateVariable(name, registry =>
                {
                    actual = registry;

                    return _formulaCompilerResult;
                });

                var expected = _variableRegistryBuilder.Build();

                _ = expected.GetValue(name);

                actual.Should().BeSameAs(expected);
            }
        }

        public class AddLazyVariable_Resolver : VariableRegistryBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddLazyVariable(null, () => Create<double>(), Create<bool>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddLazyVariable(string.Empty, () => Create<double>(), Create<bool>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddLazyVariable("  ", () => Create<double>(), Create<bool>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Resolver_Null()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddLazyVariable(Create<string>(), (Func<double>) null, Create<bool>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("valueResolver");
            }

            [Fact]
            public void Should_Add_Variable()
            {
                var name = Create<string>();
                var value = Create<double>();

                _ = _variableRegistryBuilder.AddLazyVariable(name, () => value, Create<bool>());

                var registry = _variableRegistryBuilder.Build();

                registry.GetVariable(name).Should().BeOfType<LazyVariable>();

                registry
                    .GetValue(name)
                    .Should()
                    .Be(value);
            }
        }

        public class AddLazyVariable_Registry_Resolver : VariableRegistryBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddLazyVariable(null, registry => Create<double>(), Create<bool>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddLazyVariable(string.Empty, registry => Create<double>(), Create<bool>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddLazyVariable("  ", registry => Create<double>(), Create<bool>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Resolver_Null()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddLazyVariable(Create<string>(), (Func<IVariableRegistry, double>) null, Create<bool>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("valueResolver");
            }

            [Fact]
            public void Should_Add_Variable()
            {
                var name = Create<string>();
                var value = Create<double>();

                _ = _variableRegistryBuilder.AddLazyVariable(name, registry => value, Create<bool>());

                var registry = _variableRegistryBuilder.Build();

                registry.GetVariable(name).Should().BeOfType<LazyVariable>();

                registry
                    .GetValue(name)
                    .Should()
                    .Be(value);
            }

            [Fact]
            public void Should_Provide_Registry()
            {
                var name = Create<string>();
                IVariableRegistry actual = null;

                _ = _variableRegistryBuilder.AddLazyVariable(name, registry =>
                {
                    actual = registry;

                    return Create<double>();
                }, Create<bool>());

                var expected = _variableRegistryBuilder.Build();

                _ = expected.GetValue(name);

                actual.Should().BeSameAs(expected);
            }
        }

        public class AddLazyVariable_FormulaCompilerResult : VariableRegistryBuilderFixture
        {
            private readonly double _value;
            private readonly FormulaCompilerResult _formulaCompilerResult;

            public AddLazyVariable_FormulaCompilerResult()
            {
                var formulaCompiler = new FormulaCompiler();
                _formulaCompilerResult = formulaCompiler.Compile($"{_value}");
            }

            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddLazyVariable(null, _formulaCompilerResult);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddLazyVariable(string.Empty, _formulaCompilerResult);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddLazyVariable("  ", _formulaCompilerResult);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Resolver_Null()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddLazyVariable(Create<string>(), (FormulaCompilerResult) null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("formulaCompilerResultResolver");
            }

            [Fact]
            public void Should_Add_Variable()
            {
                var name = Create<string>();

                _ = _variableRegistryBuilder.AddLazyVariable(name, _formulaCompilerResult);

                var registry = _variableRegistryBuilder.Build();

                registry.GetVariable(name).Should().BeOfType<LazyVariable>();

                registry
                    .GetValue(name)
                    .Should()
                    .Be(_value);
            }
        }

        public class AddLazyVariable_Registry_FormulaCompilerResult : VariableRegistryBuilderFixture
        {
            private readonly double _value;
            private readonly FormulaCompilerResult _formulaCompilerResult;

            public AddLazyVariable_Registry_FormulaCompilerResult()
            {
                var formulaCompiler = new FormulaCompiler();
                _formulaCompilerResult = formulaCompiler.Compile($"{_value}");
            }

            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddLazyVariable(null, registry => _formulaCompilerResult);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddLazyVariable(string.Empty, registry => _formulaCompilerResult);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddLazyVariable("  ", registry => _formulaCompilerResult);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Resolver_Null()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddLazyVariable(Create<string>(), (Func<IVariableRegistry, FormulaCompilerResult>) null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("formulaCompilerResultResolver");
            }

            [Fact]
            public void Should_Add_Variable()
            {
                var name = Create<string>();

                _ = _variableRegistryBuilder.AddLazyVariable(name, registry => _formulaCompilerResult);

                var registry = _variableRegistryBuilder.Build();

                registry.GetVariable(name).Should().BeOfType<LazyVariable>();

                registry
                    .GetValue(name)
                    .Should()
                    .Be(_value);
            }

            [Fact]
            public void Should_Provide_Registry()
            {
                var name = Create<string>();
                IVariableRegistry actual = null;

                _ = _variableRegistryBuilder.AddLazyVariable(name, registry =>
                {
                    actual = registry;

                    return _formulaCompilerResult;
                });

                var expected = _variableRegistryBuilder.Build();

                _ = expected.GetValue(name);

                actual.Should().BeSameAs(expected);
            }
        }

        public class AddMutableVariable : VariableRegistryBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddMutableVariable(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddMutableVariable(string.Empty);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddMutableVariable("  ");
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Add_Variable()
            {
                var name = Create<string>();
                var value = Create<double>();

                _ = _variableRegistryBuilder.AddMutableVariable(name, value);

                var registry = _variableRegistryBuilder.Build();

                registry.GetVariable(name).Should().BeOfType<MutableVariable>();

                registry
                    .GetValue(name)
                    .Should()
                    .Be(value);
            }
        }

        public class AddMutableVariable_Registry_Resolver : VariableRegistryBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddMutableVariable(null, registry => Create<double>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddMutableVariable(string.Empty, registry => Create<double>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddMutableVariable("  ", registry => Create<double>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Resolver_Null()
            {
                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.AddMutableVariable(Create<string>(), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("valueResolver");
            }

            [Fact]
            public void Should_Add_Variable()
            {
                var name = Create<string>();
                var value = Create<double>();

                _ = _variableRegistryBuilder.AddMutableVariable(name, registry => value);

                var registry = _variableRegistryBuilder.Build();

                registry.GetVariable(name).Should().BeOfType<MutableVariable>();

                registry
                    .GetValue(name)
                    .Should()
                    .Be(value);
            }

            [Fact]
            public void Should_Provide_Registry()
            {
                var name = Create<string>();
                IVariableRegistry actual = null;

                _ = _variableRegistryBuilder.AddMutableVariable(name, registry =>
                {
                    actual = registry;

                    return Create<double>();
                });

                var expected = _variableRegistryBuilder.Build();

                _ = expected.GetValue(name);

                actual.Should().BeSameAs(expected);
            }
        }










    }
}
