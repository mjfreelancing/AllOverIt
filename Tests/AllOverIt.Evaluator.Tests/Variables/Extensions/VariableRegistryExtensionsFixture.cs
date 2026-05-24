using AllOverIt.Evaluator.Variables;
using AllOverIt.Evaluator.Variables.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FakeItEasy;
using Shouldly;

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
                Should.Throw<ArgumentNullException>(() => VariableRegistryExtensions.Add(null, _variable))
                    .WithNamedMessageWhenNull("registry");
            }

            [Fact]
            public void Should_Throw_When_Variable_Null()
            {
                Should.Throw<ArgumentNullException>(() => _registryFake.FakedObject.Add((IVariable) null))
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
                Should.Throw<ArgumentNullException>(() => VariableRegistryExtensions.Add(null, new[] { _variable }))
                    .WithNamedMessageWhenNull("registry");
            }

            [Fact]
            public void Should_Throw_When_Variables_Null()
            {
                Should.Throw<ArgumentNullException>(() => VariableRegistryExtensions.Add(_registryFake.FakedObject, (IVariable[]) null))
                    .WithNamedMessageWhenNull("variables");
            }

            [Fact]
            public void Should_Throw_When_Variables_Empty()
            {
                Should.Throw<ArgumentException>(() => VariableRegistryExtensions.Add(_registryFake.FakedObject, Enumerable.Empty<IVariable>().ToArray()))
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
                Should.Throw<ArgumentNullException>(() => VariableRegistryExtensions.AddConstantVariable(null, Create<string>(), Create<double>()))
                    .WithNamedMessageWhenNull("registry");
            }

            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Should.Throw<ArgumentNullException>(() => VariableRegistryExtensions.AddConstantVariable(_registryFake.FakedObject, null, default))
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

                actual.ShouldBeOfType<ConstantVariable>();

                actual.Name.ShouldBe(name);
                actual.Value.ShouldBe(value);
            }
        }

        public class AddMutableVariable : VariableRegistryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Registry_Null()
            {
                Should.Throw<ArgumentNullException>(() => VariableRegistryExtensions.AddMutableVariable(null, Create<string>()))
                    .WithNamedMessageWhenNull("registry");
            }

            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Should.Throw<ArgumentNullException>(() => VariableRegistryExtensions.AddMutableVariable(_registryFake.FakedObject, null, default))
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

                actual.ShouldBeOfType<MutableVariable>();

                actual.Name.ShouldBe(name);
                actual.Value.ShouldBe(value);
            }
        }

        public class AddDelegateVariable_Func : VariableRegistryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Registry_Null()
            {
                Should.Throw<ArgumentNullException>(() => VariableRegistryExtensions.AddDelegateVariable(null, Create<string>(), () => Create<double>()))
                    .WithNamedMessageWhenNull("registry");
            }

            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Should.Throw<ArgumentNullException>(() => VariableRegistryExtensions.AddDelegateVariable(_registryFake.FakedObject, null, () => Create<double>()))
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Resolver_Null()
            {
                Should.Throw<ArgumentNullException>(() => VariableRegistryExtensions.AddDelegateVariable(_registryFake.FakedObject, Create<string>(), (Func<double>) null))
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

                actual.ShouldBeOfType<DelegateVariable>();

                actual.Name.ShouldBe(name);
                actual.Value.ShouldBe(value);
            }
        }

        public class AddDelegateVariable_FormulaCompilerResult : VariableRegistryExtensionsFixture
        {
            private readonly FormulaCompilerResult _formulaCompilerResult = new FormulaCompilerResult(null, () => 0.0d, []);

            [Fact]
            public void Should_Throw_When_Registry_Null()
            {
                Should.Throw<ArgumentNullException>(() => VariableRegistryExtensions.AddDelegateVariable(null, Create<string>(), _formulaCompilerResult))
                    .WithNamedMessageWhenNull("registry");
            }

            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Should.Throw<ArgumentNullException>(() => VariableRegistryExtensions.AddDelegateVariable(_registryFake.FakedObject, null, _formulaCompilerResult))
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_FormulaCompilerResult_Null()
            {
                Should.Throw<ArgumentNullException>(() => VariableRegistryExtensions.AddDelegateVariable(_registryFake.FakedObject, Create<string>(), (FormulaCompilerResult) null))
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

                actual.ShouldBeOfType<DelegateVariable>();

                actual.Name.ShouldBe(name);
                actual.Value.ShouldBe(value);
            }
        }

        public class AddLazyVariable_Func : VariableRegistryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Registry_Null()
            {
                Should.Throw<ArgumentNullException>(() => VariableRegistryExtensions.AddLazyVariable(null, Create<string>(), () => Create<double>()))
                    .WithNamedMessageWhenNull("registry");
            }

            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Should.Throw<ArgumentNullException>(() => VariableRegistryExtensions.AddLazyVariable(_registryFake.FakedObject, null, () => Create<double>()))
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Resolver_Null()
            {
                Should.Throw<ArgumentNullException>(() => VariableRegistryExtensions.AddLazyVariable(_registryFake.FakedObject, Create<string>(), (Func<double>) null))
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

                actual.ShouldBeOfType<LazyVariable>();

                actual.Name.ShouldBe(name);
                actual.Value.ShouldBe(value);
            }
        }

        public class AddLazyVariable_FormulaCompilerResult : VariableRegistryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Registry_Null()
            {
                Should.Throw<ArgumentNullException>(() => VariableRegistryExtensions.AddLazyVariable(null, Create<string>(), (FormulaCompilerResult) null))
                    .WithNamedMessageWhenNull("registry");
            }

            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Should.Throw<ArgumentNullException>(() => VariableRegistryExtensions.AddLazyVariable(_registryFake.FakedObject, null, (FormulaCompilerResult) null))
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

                actual.ShouldBeOfType<LazyVariable>();

                actual.Name.ShouldBe(name);
                actual.Value.ShouldBe(value);
            }
        }
    }
}
