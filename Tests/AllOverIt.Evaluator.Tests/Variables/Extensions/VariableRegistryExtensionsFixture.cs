using AllOverIt.Evaluator.Variables;
using AllOverIt.Evaluator.Variables.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FakeItEasy;
using FluentAssertions;

namespace AllOverIt.Evaluator.Tests.Variables.Extensions
{
    public class VariableRegistryExtensionsFixture : FixtureBase
    {
        private readonly Fake<IVariableRegistry> _registryFake;
        private readonly IVariable _variable;

        public VariableRegistryExtensionsFixture()
        {
            _registryFake = new Fake<IVariableRegistry>();
            _variable = this.CreateStub<IVariable>();
        }

        public class Add_Single : VariableRegistryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Registry_Null()
            {
                Invoking(() => VariableRegistryExtensions.Add(null, _variable))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("registry");
            }

            [Fact]
            public void Should_Throw_When_Variable_Null()
            {
                Invoking(() => _registryFake.FakedObject.Add((IVariable) null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("variable");
            }

            [Fact]
            public void Should_Call_Registry_AddVariable()
            {
                _registryFake.FakedObject.Add(_variable);

                _registryFake
                  .CallsTo(fake => fake.AddVariable(_variable))
                  .MustHaveHappened(1, Times.Exactly);
            }
        }

        public class Add_Multiple : VariableRegistryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Registry_Null()
            {
                Invoking(() => VariableRegistryExtensions.Add(null, new[] { _variable }))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("registry");
            }

            [Fact]
            public void Should_Throw_When_Variables_Null()
            {
                Invoking(() => VariableRegistryExtensions.Add(_registryFake.FakedObject, (IVariable[]) null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("variables");
            }

            [Fact]
            public void Should_Throw_When_Variables_Empty()
            {
                Invoking(() => VariableRegistryExtensions.Add(_registryFake.FakedObject, Enumerable.Empty<IVariable>().ToArray()))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("variables");
            }

            [Fact]
            public void Should_Call_Registry_AddVariable()
            {
                var variables = Enumerable.Range(1, 5).Select(_ => this.CreateStub<IVariable>()).ToArray();

                VariableRegistryExtensions.Add(_registryFake.FakedObject, variables);

                foreach (var variable in variables)
                {
                    _registryFake
                        .CallsTo(fake => fake.AddVariable(variable))
                        .MustHaveHappened(1, Times.Exactly);
                }
            }
        }

        public class AddConstantVariable : VariableRegistryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Registry_Null()
            {
                Invoking(() => VariableRegistryExtensions.AddConstantVariable(null, Create<string>(), Create<double>()))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("registry");
            }

            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() => VariableRegistryExtensions.AddConstantVariable(_registryFake.FakedObject, null, default))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Add_ConstantVariable()
            {
                var name = Create<string>();
                var value = Create<double>();
                IVariable actual = null;

                _registryFake
                  .CallsTo(fake => fake.AddVariable(A<IVariable>.Ignored))
                  .Invokes(call => actual = call.Arguments.Get<IVariable>(0));

                VariableRegistryExtensions.AddConstantVariable(_registryFake.FakedObject, name, value);

                actual.Should().BeOfType<ConstantVariable>();

                actual.Name.Should().Be(name);
                actual.Value.Should().Be(value);
            }
        }

        public class AddMutableVariable : VariableRegistryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Registry_Null()
            {
                Invoking(() => VariableRegistryExtensions.AddMutableVariable(null, Create<string>()))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("registry");
            }

            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() => VariableRegistryExtensions.AddMutableVariable(_registryFake.FakedObject, null, default))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Add_MutableVariable()
            {
                var name = Create<string>();
                var value = Create<double>();
                IVariable actual = null;

                _registryFake
                  .CallsTo(fake => fake.AddVariable(A<IVariable>.Ignored))
                  .Invokes(call => actual = call.Arguments.Get<IVariable>(0));

                VariableRegistryExtensions.AddMutableVariable(_registryFake.FakedObject, name, value);

                actual.Should().BeOfType<MutableVariable>();

                actual.Name.Should().Be(name);
                actual.Value.Should().Be(value);
            }
        }

        public class AddDelegateVariable_Func : VariableRegistryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Registry_Null()
            {
                Invoking(() => VariableRegistryExtensions.AddDelegateVariable(null, Create<string>(), () => Create<double>()))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("registry");
            }

            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() => VariableRegistryExtensions.AddDelegateVariable(_registryFake.FakedObject, null, () => Create<double>()))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Resolver_Null()
            {
                Invoking(() => VariableRegistryExtensions.AddDelegateVariable(_registryFake.FakedObject, Create<string>(), (Func<double>) null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("valueResolver");
            }

            [Fact]
            public void Should_Add_DelegateVariable()
            {
                var name = Create<string>();
                var value = Create<double>();

                IVariable actual = null;

                _registryFake
                  .CallsTo(fake => fake.AddVariable(A<IVariable>.Ignored))
                  .Invokes(call => actual = call.Arguments.Get<IVariable>(0));

                VariableRegistryExtensions.AddDelegateVariable(_registryFake.FakedObject, name, () => value);

                actual.Should().BeOfType<DelegateVariable>();

                actual.Name.Should().Be(name);
                actual.Value.Should().Be(value);
            }
        }

        public class AddDelegateVariable_FormulaCompilerResult : VariableRegistryExtensionsFixture
        {
            private readonly FormulaCompilerResult _formulaCompilerResult = new FormulaCompilerResult(null, () => 0.0d, []);

            [Fact]
            public void Should_Throw_When_Registry_Null()
            {
                Invoking(() => VariableRegistryExtensions.AddDelegateVariable(null, Create<string>(), _formulaCompilerResult))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("registry");
            }

            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() => VariableRegistryExtensions.AddDelegateVariable(_registryFake.FakedObject, null, _formulaCompilerResult))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_FormulaCompilerResult_Null()
            {
                Invoking(() => VariableRegistryExtensions.AddDelegateVariable(_registryFake.FakedObject, Create<string>(), (FormulaCompilerResult) null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("formulaCompilerResult");
            }

            [Fact]
            public void Should_Add_DelegateVariable()
            {
                var name = Create<string>();
                var value = Create<double>();

                var compilerResult = new FormulaCompilerResult(_registryFake.FakedObject, () => value, Array.Empty<string>())
;
                IVariable actual = null;

                _registryFake
                  .CallsTo(fake => fake.AddVariable(A<IVariable>.Ignored))
                  .Invokes(call => actual = call.Arguments.Get<IVariable>(0));

                VariableRegistryExtensions.AddDelegateVariable(_registryFake.FakedObject, name, compilerResult);

                actual.Should().BeOfType<DelegateVariable>();

                actual.Name.Should().Be(name);
                actual.Value.Should().Be(value);
            }
        }

        public class AddLazyVariable_Func : VariableRegistryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Registry_Null()
            {
                Invoking(() => VariableRegistryExtensions.AddLazyVariable(null, Create<string>(), () => Create<double>()))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("registry");
            }

            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() => VariableRegistryExtensions.AddLazyVariable(_registryFake.FakedObject, null, () => Create<double>()))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Resolver_Null()
            {
                Invoking(() => VariableRegistryExtensions.AddLazyVariable(_registryFake.FakedObject, Create<string>(), (Func<double>) null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("valueResolver");
            }

            [Fact]
            public void Should_Add_LazyVariable()
            {
                var name = Create<string>();
                var value = Create<double>();

                IVariable actual = null;

                _registryFake
                  .CallsTo(fake => fake.AddVariable(A<IVariable>.Ignored))
                  .Invokes(call => actual = call.Arguments.Get<IVariable>(0));

                VariableRegistryExtensions.AddLazyVariable(_registryFake.FakedObject, name, () => value);

                actual.Should().BeOfType<LazyVariable>();

                actual.Name.Should().Be(name);
                actual.Value.Should().Be(value);
            }
        }

        public class AddLazyVariable_FormulaCompilerResult : VariableRegistryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Registry_Null()
            {
                Invoking(() => VariableRegistryExtensions.AddLazyVariable(null, Create<string>(), (FormulaCompilerResult) null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("registry");
            }

            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() => VariableRegistryExtensions.AddLazyVariable(_registryFake.FakedObject, null, (FormulaCompilerResult) null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Add_LazyVariable()
            {
                var name = Create<string>();
                var value = Create<double>();

                var compilerResult = new FormulaCompilerResult(_registryFake.FakedObject, () => value, Array.Empty<string>())
;
                IVariable actual = null;

                _registryFake
                  .CallsTo(fake => fake.AddVariable(A<IVariable>.Ignored))
                  .Invokes(call => actual = call.Arguments.Get<IVariable>(0));

                VariableRegistryExtensions.AddLazyVariable(_registryFake.FakedObject, name, compilerResult);

                actual.Should().BeOfType<LazyVariable>();

                actual.Name.Should().Be(name);
                actual.Value.Should().Be(value);
            }
        }
    }
}
