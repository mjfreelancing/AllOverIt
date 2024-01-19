using AllOverIt.Evaluator.Exceptions;
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
                    _ = _variableRegistryBuilder.AddConstantVariable(null, Create<double>());
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
                    _ = _variableRegistryBuilder.AddConstantVariable(string.Empty, Create<double>());
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
                    _ = _variableRegistryBuilder.AddConstantVariable("  ", Create<double>());
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

        public class Build : VariableRegistryBuilderFixture
        {
            [Fact]
            public void Should_Build_Functional_1()
            {
                _variableRegistryBuilder.Build().Should().BeOfType<VariableRegistry>();
            }

            [Fact]
            public void Should_Build_Functional_2()
            {
                _variableRegistryBuilder.AddConstantVariable(Create<string>(), Create<double>());

                var registry = _variableRegistryBuilder.Build();

                registry.Should().HaveCount(1);
            }

            [Fact]
            public void Should_Build_Functional_3()
            {
                var formulaCompiler = new FormulaCompiler();

                _variableRegistryBuilder.AddConstantVariable("a", Create<double>());
                _variableRegistryBuilder.AddDelegateVariable("b", registry => formulaCompiler.Compile("a + 1", registry));

                var registry = _variableRegistryBuilder.Build();

                registry.Should().HaveCount(2);
            }

            [Fact]
            public void Should_Build_Functional_4()
            {
                var formulaCompiler = new FormulaCompiler();

                _variableRegistryBuilder.AddConstantVariable("a", Create<double>());
                _variableRegistryBuilder.AddDelegateVariable("b", registry => formulaCompiler.Compile("a + 1", registry));
                _variableRegistryBuilder.AddDelegateVariable("d", registry => formulaCompiler.Compile("c - b", registry));
                _variableRegistryBuilder.AddDelegateVariable("c", registry => formulaCompiler.Compile("a * b", registry));

                var registry = _variableRegistryBuilder.Build();

                registry.Should().HaveCount(4);
            }

            [Fact]
            public void Should_Throw_When_Cannot_Build_Functional_1()
            {
                var formulaCompiler = new FormulaCompiler();

                _variableRegistryBuilder.AddConstantVariable("a", Create<double>());
                _variableRegistryBuilder.AddDelegateVariable("b", registry => formulaCompiler.Compile("a + 1", registry));
                _variableRegistryBuilder.AddDelegateVariable("d", registry => formulaCompiler.Compile("c - b", registry));

                Invoking(() =>
                {
                    _ = _variableRegistryBuilder.Build();
                })
                    .Should()
                    .Throw<VariableRegistryBuilderException>()
                    .WithMessage("Cannot build the variable registry due to missing variable references.");

                _variableRegistryBuilder.GetUnregisteredVariableNames()
                    .Should()
                    .BeEquivalentTo(new[] { "c" });
            }

            [Fact]
            public void Should_Throw_When_Cannot_Build_Functional_2()
            {
                var formulaCompiler = new FormulaCompiler();

                _variableRegistryBuilder.AddConstantVariable("a", Create<double>());
                _variableRegistryBuilder.AddDelegateVariable("b", registry => formulaCompiler.Compile("a + e * f", registry));
                _variableRegistryBuilder.AddDelegateVariable("d", registry => formulaCompiler.Compile("c - b + g", registry));       // b is registered but cannot be resolved - won't be in the list of unregistered variables

                Invoking(() =>
                    {
                        _ = _variableRegistryBuilder.Build();
                    })
                    .Should()
                    .Throw<VariableRegistryBuilderException>()
                    .WithMessage("Cannot build the variable registry due to missing variable references.");

                _variableRegistryBuilder.GetUnregisteredVariableNames()
                    .Should()
                    .BeEquivalentTo(new[] { "e", "f", "c", "g" });
            }
        }

        public class TryBuild : VariableRegistryBuilderFixture
        {
            [Fact]
            public void Should_Build_Functional_1()
            {
                var suucess = _variableRegistryBuilder.TryBuild(out var variableRegistry);

                suucess.Should().BeTrue();
                variableRegistry.Should().BeOfType<VariableRegistry>();
            }

            [Fact]
            public void Should_Build_Functional_2()
            {
                _variableRegistryBuilder.AddConstantVariable(Create<string>(), Create<double>());

                _ = _variableRegistryBuilder.TryBuild(out var variableRegistry);

                variableRegistry.Should().HaveCount(1);
            }

            [Fact]
            public void Should_Build_Functional_3()
            {
                var formulaCompiler = new FormulaCompiler();

                _variableRegistryBuilder.AddConstantVariable("a", Create<double>());
                _variableRegistryBuilder.AddDelegateVariable("b", registry => formulaCompiler.Compile("a + 1", registry));

                _ = _variableRegistryBuilder.TryBuild(out var variableRegistry);

                variableRegistry.Should().HaveCount(2);
            }

            [Fact]
            public void Should_Build_Functional_4()
            {
                var formulaCompiler = new FormulaCompiler();

                _variableRegistryBuilder.AddConstantVariable("a", Create<double>());
                _variableRegistryBuilder.AddDelegateVariable("b", registry => formulaCompiler.Compile("a + 1", registry));
                _variableRegistryBuilder.AddDelegateVariable("d", registry => formulaCompiler.Compile("c - b", registry));
                _variableRegistryBuilder.AddDelegateVariable("c", registry => formulaCompiler.Compile("a * b", registry));

                _ = _variableRegistryBuilder.TryBuild(out var variableRegistry);

                variableRegistry.Should().HaveCount(4);
            }

            [Fact]
            public void Should_Fail_When_Cannot_Build_Functional_1()
            {
                var formulaCompiler = new FormulaCompiler();

                _variableRegistryBuilder.AddConstantVariable("a", Create<double>());
                _variableRegistryBuilder.AddDelegateVariable("b", registry => formulaCompiler.Compile("a + 1", registry));
                _variableRegistryBuilder.AddDelegateVariable("d", registry => formulaCompiler.Compile("c - b", registry));

                _variableRegistryBuilder.TryBuild(out _).Should().BeFalse();

                _variableRegistryBuilder.GetUnregisteredVariableNames()
                    .Should()
                    .BeEquivalentTo(new[] { "c" });
            }

            [Fact]
            public void Should_Throw_When_Cannot_Build_Functional_2()
            {
                var formulaCompiler = new FormulaCompiler();

                _variableRegistryBuilder.AddConstantVariable("a", Create<double>());
                _variableRegistryBuilder.AddDelegateVariable("b", registry => formulaCompiler.Compile("a + e * f", registry));
                _variableRegistryBuilder.AddDelegateVariable("d", registry => formulaCompiler.Compile("c - b + g", registry));       // b is registered but cannot be resolved - won't be in the list of unregistered variables

                _variableRegistryBuilder.TryBuild(out _).Should().BeFalse();

                _variableRegistryBuilder.GetUnregisteredVariableNames()
                    .Should()
                    .BeEquivalentTo(new[] { "e", "f", "c", "g" });
            }
        }

        public class GetUnregisteredVariableNames : VariableRegistryBuilderFixture
        {
            [Fact]
            public void Should_Return_Empty_Names()
            {
                _variableRegistryBuilder.GetUnregisteredVariableNames().Should().BeEmpty();
            }

            [Fact]
            public void Should_Return_Expected_Names_Functional_1()
            {
                var formulaCompiler = new FormulaCompiler();

                _variableRegistryBuilder.AddConstantVariable("a", Create<double>());
                _variableRegistryBuilder.AddDelegateVariable("b", registry => formulaCompiler.Compile("a + e * f", registry));
                _variableRegistryBuilder.AddDelegateVariable("d", registry => formulaCompiler.Compile("c - b + g", registry));

                // Above, b is pending registration since e and f cannot be resolved - only e, f, c, g will be reported as unregistered
                // despite b not yet being present in the variable registry.

                _variableRegistryBuilder.GetUnregisteredVariableNames()
                    .Should()
                    .BeEquivalentTo(new[] { "e", "f", "c", "g" });

                _variableRegistryBuilder.AddConstantVariable("f", Create<double>());
                _variableRegistryBuilder.AddConstantVariable("e", Create<double>());

                _variableRegistryBuilder.GetUnregisteredVariableNames()
                    .Should()
                    .BeEquivalentTo(new[] { "c", "g" });

                _variableRegistryBuilder.AddConstantVariable("c", Create<double>());
                _variableRegistryBuilder.AddConstantVariable("g", Create<double>());

                _variableRegistryBuilder.GetUnregisteredVariableNames()
                    .Should()
                    .BeEmpty();
            }

            [Fact]
            public void Should_Return_Expected_Names_Functional_2()
            {
                var formulaCompiler = new FormulaCompiler();

                _variableRegistryBuilder.AddDelegateVariable("a", registry => formulaCompiler.Compile("j + k", registry));
                _variableRegistryBuilder.AddDelegateVariable("b", registry => formulaCompiler.Compile("a + e * f", registry));
                _variableRegistryBuilder.AddDelegateVariable("e", registry => formulaCompiler.Compile("g + h", registry));

                // Above, b is pending registration since e and f cannot be resolved - only e, f, c, g will be reported as unregistered
                // despite b not yet being present in the variable registry.

                _variableRegistryBuilder.GetUnregisteredVariableNames()
                    .Should()
                    .BeEquivalentTo(new[] { "j", "k", "f", "g", "h" });

                _variableRegistryBuilder.AddConstantVariable("g", Create<double>());
                _variableRegistryBuilder.AddConstantVariable("h", Create<double>());

                _variableRegistryBuilder.GetUnregisteredVariableNames()
                    .Should()
                    .BeEquivalentTo(new[] { "j", "k", "f" });

                _variableRegistryBuilder.AddDelegateVariable("m", registry => formulaCompiler.Compile("n + p", registry));

                _variableRegistryBuilder.GetUnregisteredVariableNames()
                    .Should()
                    .BeEquivalentTo(new[] { "j", "k", "f", "n", "p" });

                _variableRegistryBuilder.AddConstantVariable("j", Create<double>());
                _variableRegistryBuilder.AddConstantVariable("k", Create<double>());
                _variableRegistryBuilder.AddConstantVariable("p", Create<double>());
                _variableRegistryBuilder.AddConstantVariable("f", Create<double>());
                _variableRegistryBuilder.AddConstantVariable("n", Create<double>());

                _variableRegistryBuilder.GetUnregisteredVariableNames()
                    .Should()
                    .BeEmpty();
            }
        }
    }
}
