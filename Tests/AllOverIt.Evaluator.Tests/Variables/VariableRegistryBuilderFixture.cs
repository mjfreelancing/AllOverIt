using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using Shouldly;

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

                actual.ShouldBeOfType<VariableRegistryBuilder>();
            }
        }

        public class AddConstantVariable : VariableRegistryBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = _variableRegistryBuilder.AddConstantVariable(stringValue, Create<double>());
                    }, "name");
            }

            [Fact]
            public void Should_Add_Variable()
            {
                var name = Create<string>();
                var value = Create<double>();

                _ = _variableRegistryBuilder.AddConstantVariable(name, value);

                var registry = _variableRegistryBuilder.Build();

                registry.GetVariable(name).ShouldBeOfType<ConstantVariable>();

                registry
                    .GetValue(name)
                    .ShouldBe(value);
            }
        }

        public class AddDelegateVariable_Resolver : VariableRegistryBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = _variableRegistryBuilder.AddDelegateVariable(stringValue, () => Create<double>());
                    }, "name");
            }

            [Fact]
            public void Should_Throw_When_Resolver_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = _variableRegistryBuilder.AddDelegateVariable(Create<string>(), (Func<double>)null);
                })
                .WithNamedMessageWhenNull("valueResolver");
            }

            [Fact]
            public void Should_Add_Variable()
            {
                var name = Create<string>();
                var value = Create<double>();

                _ = _variableRegistryBuilder.AddDelegateVariable(name, () => value);

                var registry = _variableRegistryBuilder.Build();

                registry.GetVariable(name).ShouldBeOfType<DelegateVariable>();

                registry
                    .GetValue(name)
                    .ShouldBe(value);
            }
        }

        public class AddDelegateVariable_Registry_Resolver : VariableRegistryBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = _variableRegistryBuilder.AddDelegateVariable(stringValue, registry => Create<double>());
                    }, "name");
            }

            [Fact]
            public void Should_Throw_When_Resolver_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = _variableRegistryBuilder.AddDelegateVariable(Create<string>(), (Func<IVariableRegistry, double>)null);
                })
                .WithNamedMessageWhenNull("valueResolver");
            }

            [Fact]
            public void Should_Add_Variable()
            {
                var name = Create<string>();
                var value = Create<double>();

                _ = _variableRegistryBuilder.AddDelegateVariable(name, registry => value);

                var registry = _variableRegistryBuilder.Build();

                registry.GetVariable(name).ShouldBeOfType<DelegateVariable>();

                registry
                    .GetValue(name)
                    .ShouldBe(value);
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

                actual.ShouldBeSameAs(expected);
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
            public void Should_Throw_When_Name_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = _variableRegistryBuilder.AddDelegateVariable(stringValue, _formulaCompilerResult);
                    }, "name");
            }

            [Fact]
            public void Should_Throw_When_Resolver_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = _variableRegistryBuilder.AddDelegateVariable(Create<string>(), (FormulaCompilerResult)null);
                })
                .WithNamedMessageWhenNull("formulaCompilerResultResolver");
            }

            [Fact]
            public void Should_Add_Variable()
            {
                var name = Create<string>();

                _ = _variableRegistryBuilder.AddDelegateVariable(name, _formulaCompilerResult);

                var registry = _variableRegistryBuilder.Build();

                registry.GetVariable(name).ShouldBeOfType<DelegateVariable>();

                registry
                    .GetValue(name)
                    .ShouldBe(_value);
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
            public void Should_Throw_When_Name_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = _variableRegistryBuilder.AddDelegateVariable(stringValue, registry => _formulaCompilerResult);
                    }, "name");
            }

            [Fact]
            public void Should_Throw_When_Resolver_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = _variableRegistryBuilder.AddDelegateVariable(Create<string>(), (Func<IVariableRegistry, FormulaCompilerResult>)null);
                })
                .WithNamedMessageWhenNull("formulaCompilerResultResolver");
            }

            [Fact]
            public void Should_Add_Variable()
            {
                var name = Create<string>();

                _ = _variableRegistryBuilder.AddDelegateVariable(name, registry => _formulaCompilerResult);

                var registry = _variableRegistryBuilder.Build();

                registry.GetVariable(name).ShouldBeOfType<DelegateVariable>();

                registry
                    .GetValue(name)
                    .ShouldBe(_value);
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

                actual.ShouldBeSameAs(expected);
            }
        }

        public class AddLazyVariable_Resolver : VariableRegistryBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = _variableRegistryBuilder.AddLazyVariable(stringValue, () => Create<double>(), Create<bool>());
                    }, "name");
            }

            [Fact]
            public void Should_Throw_When_Resolver_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = _variableRegistryBuilder.AddLazyVariable(Create<string>(), (Func<double>)null, Create<bool>());
                })
                .WithNamedMessageWhenNull("valueResolver");
            }

            [Fact]
            public void Should_Add_Variable()
            {
                var name = Create<string>();
                var value = Create<double>();

                _ = _variableRegistryBuilder.AddLazyVariable(name, () => value, Create<bool>());

                var registry = _variableRegistryBuilder.Build();

                registry.GetVariable(name).ShouldBeOfType<LazyVariable>();

                registry
                    .GetValue(name)
                    .ShouldBe(value);
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
            public void Should_Throw_When_Name_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = _variableRegistryBuilder.AddLazyVariable(stringValue, _formulaCompilerResult);
                    }, "name");
            }

            [Fact]
            public void Should_Throw_When_Resolver_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = _variableRegistryBuilder.AddLazyVariable(Create<string>(), (FormulaCompilerResult)null);
                })
                .WithNamedMessageWhenNull("formulaCompilerResultResolver");
            }

            [Fact]
            public void Should_Add_Variable()
            {
                var name = Create<string>();

                _ = _variableRegistryBuilder.AddLazyVariable(name, _formulaCompilerResult);

                var registry = _variableRegistryBuilder.Build();

                registry.GetVariable(name).ShouldBeOfType<LazyVariable>();

                registry
                    .GetValue(name)
                    .ShouldBe(_value);
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
            public void Should_Throw_When_Name_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = _variableRegistryBuilder.AddLazyVariable(stringValue, registry => _formulaCompilerResult);
                    }, "name");
            }

            [Fact]
            public void Should_Throw_When_Resolver_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = _variableRegistryBuilder.AddLazyVariable(Create<string>(), (Func<IVariableRegistry, FormulaCompilerResult>)null);
                })
                .WithNamedMessageWhenNull("formulaCompilerResultResolver");
            }

            [Fact]
            public void Should_Add_Variable()
            {
                var name = Create<string>();

                _ = _variableRegistryBuilder.AddLazyVariable(name, registry => _formulaCompilerResult);

                var registry = _variableRegistryBuilder.Build();

                registry.GetVariable(name).ShouldBeOfType<LazyVariable>();

                registry
                    .GetValue(name)
                    .ShouldBe(_value);
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

                actual.ShouldBeSameAs(expected);
            }
        }

        public class AddMutableVariable : VariableRegistryBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = _variableRegistryBuilder.AddMutableVariable(stringValue);
                    }, "name");
            }

            [Fact]
            public void Should_Add_Variable()
            {
                var name = Create<string>();
                var value = Create<double>();

                _ = _variableRegistryBuilder.AddMutableVariable(name, value);

                var registry = _variableRegistryBuilder.Build();

                registry.GetVariable(name).ShouldBeOfType<MutableVariable>();

                registry
                    .GetValue(name)
                    .ShouldBe(value);
            }
        }

        public class Build : VariableRegistryBuilderFixture
        {
            [Fact]
            public void Should_Build_Functional_1()
            {
                _variableRegistryBuilder.Build().ShouldBeOfType<VariableRegistry>();
            }

            [Fact]
            public void Should_Build_Functional_2()
            {
                _variableRegistryBuilder.AddConstantVariable(Create<string>(), Create<double>());

                var registry = _variableRegistryBuilder.Build();

                registry.Count().ShouldBe(1);
            }

            [Fact]
            public void Should_Build_Functional_3()
            {
                var formulaCompiler = new FormulaCompiler();

                _variableRegistryBuilder.AddConstantVariable("a", Create<double>());
                _variableRegistryBuilder.AddDelegateVariable("b", registry => formulaCompiler.Compile("a + 1", registry));

                var registry = _variableRegistryBuilder.Build();

                registry.Count().ShouldBe(2);
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

                registry.Count().ShouldBe(4);
            }

            [Fact]
            public void Should_Throw_When_Cannot_Build_Functional_1()
            {
                var formulaCompiler = new FormulaCompiler();

                _variableRegistryBuilder.AddConstantVariable("a", Create<double>());
                _variableRegistryBuilder.AddDelegateVariable("b", registry => formulaCompiler.Compile("a + 1", registry));
                _variableRegistryBuilder.AddDelegateVariable("d", registry => formulaCompiler.Compile("c - b", registry));

                Should.Throw<VariableRegistryBuilderException>(() =>
                {
                    _ = _variableRegistryBuilder.Build();
                })
                    .WithMessage("Cannot build the variable registry due to missing variable references.");

                _variableRegistryBuilder.GetUnregisteredVariableNames()
                    .ShouldBeEquivalentTo(new[] { "c" });
            }

            [Fact]
            public void Should_Throw_When_Cannot_Build_Functional_2()
            {
                var formulaCompiler = new FormulaCompiler();

                _variableRegistryBuilder.AddConstantVariable("a", Create<double>());
                _variableRegistryBuilder.AddDelegateVariable("b", registry => formulaCompiler.Compile("a + e * f", registry));
                _variableRegistryBuilder.AddDelegateVariable("d", registry => formulaCompiler.Compile("c - b + g", registry));       // b is registered but cannot be resolved - won't be in the list of unregistered variables

                Should.Throw<VariableRegistryBuilderException>(() =>
                    {
                        _ = _variableRegistryBuilder.Build();
                    })
                    .WithMessage("Cannot build the variable registry due to missing variable references.");

                _variableRegistryBuilder.GetUnregisteredVariableNames()
                    .ShouldBeEquivalentTo(new[] { "e", "f", "c", "g" });
            }
        }

        public class TryBuild : VariableRegistryBuilderFixture
        {
            [Fact]
            public void Should_Build_Functional_1()
            {
                var suucess = _variableRegistryBuilder.TryBuild(out var variableRegistry);

                suucess.ShouldBeTrue();
                variableRegistry.ShouldBeOfType<VariableRegistry>();
            }

            [Fact]
            public void Should_Build_Functional_2()
            {
                _variableRegistryBuilder.AddConstantVariable(Create<string>(), Create<double>());

                _ = _variableRegistryBuilder.TryBuild(out var variableRegistry);

                variableRegistry.Count().ShouldBe(1);
            }

            [Fact]
            public void Should_Build_Functional_3()
            {
                var formulaCompiler = new FormulaCompiler();

                _variableRegistryBuilder.AddConstantVariable("a", Create<double>());
                _variableRegistryBuilder.AddDelegateVariable("b", registry => formulaCompiler.Compile("a + 1", registry));

                _ = _variableRegistryBuilder.TryBuild(out var variableRegistry);

                variableRegistry.Count().ShouldBe(2);
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

                variableRegistry.Count().ShouldBe(4);
            }

            [Fact]
            public void Should_Fail_When_Cannot_Build_Functional_1()
            {
                var formulaCompiler = new FormulaCompiler();

                _variableRegistryBuilder.AddConstantVariable("a", Create<double>());
                _variableRegistryBuilder.AddDelegateVariable("b", registry => formulaCompiler.Compile("a + 1", registry));
                _variableRegistryBuilder.AddDelegateVariable("d", registry => formulaCompiler.Compile("c - b", registry));

                _variableRegistryBuilder.TryBuild(out _).ShouldBeFalse();

                _variableRegistryBuilder.GetUnregisteredVariableNames()
                    .ShouldBeEquivalentTo(new[] { "c" });
            }

            [Fact]
            public void Should_Throw_When_Cannot_Build_Functional_2()
            {
                var formulaCompiler = new FormulaCompiler();

                _variableRegistryBuilder.AddConstantVariable("a", Create<double>());
                _variableRegistryBuilder.AddDelegateVariable("b", registry => formulaCompiler.Compile("a + e * f", registry));
                _variableRegistryBuilder.AddDelegateVariable("d", registry => formulaCompiler.Compile("c - b + g", registry));       // b is registered but cannot be resolved - won't be in the list of unregistered variables

                _variableRegistryBuilder.TryBuild(out _).ShouldBeFalse();

                _variableRegistryBuilder.GetUnregisteredVariableNames()
                    .ShouldBeEquivalentTo(new[] { "e", "f", "c", "g" });
            }
        }

        public class GetUnregisteredVariableNames : VariableRegistryBuilderFixture
        {
            [Fact]
            public void Should_Return_Empty_Names()
            {
                _variableRegistryBuilder.GetUnregisteredVariableNames().ShouldBeEmpty();
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
                    .ShouldBeEquivalentTo(new[] { "e", "f", "c", "g" });

                _variableRegistryBuilder.AddConstantVariable("f", Create<double>());
                _variableRegistryBuilder.AddConstantVariable("e", Create<double>());

                _variableRegistryBuilder.GetUnregisteredVariableNames()
                    .ShouldBeEquivalentTo(new[] { "c", "g" });

                _variableRegistryBuilder.AddConstantVariable("c", Create<double>());
                _variableRegistryBuilder.AddConstantVariable("g", Create<double>());

                _variableRegistryBuilder.GetUnregisteredVariableNames()
                    .ShouldBeEmpty();
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
                    .ShouldBeEquivalentTo(new[] { "j", "k", "f", "g", "h" });

                _variableRegistryBuilder.AddConstantVariable("g", Create<double>());
                _variableRegistryBuilder.AddConstantVariable("h", Create<double>());

                _variableRegistryBuilder.GetUnregisteredVariableNames()
                    .ShouldBeEquivalentTo(new[] { "j", "k", "f" });

                _variableRegistryBuilder.AddDelegateVariable("m", registry => formulaCompiler.Compile("n + p", registry));

                _variableRegistryBuilder.GetUnregisteredVariableNames()
                    .ShouldBeEquivalentTo(new[] { "j", "k", "f", "n", "p" });

                _variableRegistryBuilder.AddConstantVariable("j", Create<double>());
                _variableRegistryBuilder.AddConstantVariable("k", Create<double>());
                _variableRegistryBuilder.AddConstantVariable("p", Create<double>());
                _variableRegistryBuilder.AddConstantVariable("f", Create<double>());
                _variableRegistryBuilder.AddConstantVariable("n", Create<double>());

                _variableRegistryBuilder.GetUnregisteredVariableNames()
                    .ShouldBeEmpty();
            }
        }
    }
}
