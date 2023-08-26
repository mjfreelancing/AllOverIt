using AllOverIt.Evaluator.Variables;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Variables
{
    public class VariableFactoryFixture : FixtureBase
    {
        private string _name;
        private readonly double _value;
        private readonly VariableFactory _variableFactory;

        public VariableFactoryFixture()
        {
            _name = Create<string>();
            _value = Create<double>();
            _variableFactory = new VariableFactory();
        }

        public class CreateVariableRegistry : VariableFactoryFixture
        {
            [Fact]
            public void Should_Create_Variable_Registry()
            {
                var registry = _variableFactory.CreateVariableRegistry();

                registry.Should().BeOfType<VariableRegistry>();
            }
        }

        public class CreateMutableVariable : VariableFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() => _variableFactory.CreateMutableVariable(null, _value))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() => _variableFactory.CreateMutableVariable(string.Empty, _value))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() => _variableFactory.CreateMutableVariable(" ", _value))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Create_MutableVariable()
            {
                var variable = _variableFactory.CreateMutableVariable(_name, _value);

                variable.Should().BeOfType<MutableVariable>();
            }

            [Fact]
            public void Should_Set_Variable_Members()
            {
                var variable = _variableFactory.CreateMutableVariable(_name, _value);

                var expected = new
                {
                    Name = _name,
                    Value = _value,
                    ReferencedVariables = Enumerable.Empty<string>()
                };

                expected.Should().BeEquivalentTo(variable);
            }
        }

        public class CreateConstantVariable : VariableFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() => _variableFactory.CreateConstantVariable(null, _value))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() => _variableFactory.CreateConstantVariable(string.Empty, _value))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() => _variableFactory.CreateConstantVariable(" ", _value))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Create_ConstantVariable()
            {
                var variable = _variableFactory.CreateConstantVariable(_name, _value);

                variable.Should().BeOfType<ConstantVariable>();
            }

            [Fact]
            public void Should_Set_Variable_Members()
            {
                var variable = _variableFactory.CreateConstantVariable(_name, _value);

                var expected = new
                {
                    Name = _name,
                    Value = _value,
                    ReferencedVariables = Enumerable.Empty<string>()
                };

                expected.Should().BeEquivalentTo(variable);
            }
        }

        public class CreateDelegateVariable_Func : VariableFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() => _variableFactory.CreateDelegateVariable(null, () => _value))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() => _variableFactory.CreateDelegateVariable(string.Empty, () => _value))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() => _variableFactory.CreateDelegateVariable(" ", () => _value))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Create_Func_Variable()
            {
                var variable = _variableFactory.CreateDelegateVariable(_name, () => _value);

                var expected = new
                {
                    Name = _name,
                    Value = _value,
                    ReferencedVariables = Enumerable.Empty<string>()
                };

                expected.Should().BeEquivalentTo(variable);
            }
        }

        public class CreateDelegateVariable_FormulaCompilerResult : VariableFactoryFixture
        {
            private FormulaCompilerResult _formulaCompilerResult;

            public CreateDelegateVariable_FormulaCompilerResult()
            {
                var formulaCompiler = new FormulaCompiler();
                _formulaCompilerResult = formulaCompiler.Compile($"{_value}");
            }

            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() => _variableFactory.CreateDelegateVariable(null, _formulaCompilerResult))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() => _variableFactory.CreateDelegateVariable(string.Empty, _formulaCompilerResult))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() => _variableFactory.CreateDelegateVariable(" ", _formulaCompilerResult))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Create_Func_Variable()
            {
                var variable = _variableFactory.CreateDelegateVariable(_name, _formulaCompilerResult);

                var expected = new
                {
                    Name = _name,
                    Value = _value,
                    ReferencedVariables = Enumerable.Empty<string>()
                };

                expected.Should().BeEquivalentTo(variable);
            }
        }

        public class CreateLazyVariable_Func : VariableFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() => _variableFactory.CreateLazyVariable(null, () => _value, Create<bool>()))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() => _variableFactory.CreateLazyVariable(string.Empty, () => _value, Create<bool>()))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() => _variableFactory.CreateLazyVariable(" ", () => _value, Create<bool>()))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Create_Lazy_Variable()
            {
                var variable = _variableFactory.CreateLazyVariable(_name, () => _value, Create<bool>());

                var expected = new
                {
                    Name = _name,
                    Value = _value,
                    ReferencedVariables = Enumerable.Empty<string>()
                };

                expected.Should().BeEquivalentTo(variable);
            }
        }

        public class CreateLazyVariable_FormulaCompilerResult : VariableFactoryFixture
        {
            private FormulaCompilerResult _formulaCompilerResult;

            public CreateLazyVariable_FormulaCompilerResult()
            {
                var formulaCompiler = new FormulaCompiler();
                _formulaCompilerResult = formulaCompiler.Compile($"{_value}");
            }

            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() => _variableFactory.CreateLazyVariable(null, _formulaCompilerResult, Create<bool>()))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() => _variableFactory.CreateLazyVariable(string.Empty, _formulaCompilerResult, Create<bool>()))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() => _variableFactory.CreateLazyVariable(" ", _formulaCompilerResult, Create<bool>()))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Create_Lazy_Variable()
            {
                var variable = _variableFactory.CreateLazyVariable(_name, _formulaCompilerResult, Create<bool>());

                var expected = new
                {
                    Name = _name,
                    Value = _value,
                    ReferencedVariables = Enumerable.Empty<string>()
                };

                expected.Should().BeEquivalentTo(variable);
            }
        }

        public class CreateAggregateVariable_Funcs : VariableFactoryFixture
        {
            private readonly IEnumerable<double> _values;
            private readonly List<Func<double>> _funcs;
            private readonly IVariable _variable;

            public CreateAggregateVariable_Funcs()
            {
                _name = Create<string>();
                _values = CreateMany<double>();

                _funcs = new List<Func<double>>();

                foreach (var value in _values)
                {
                    _funcs.Add(() => value);
                }

                _variable = _variableFactory.CreateAggregateVariable(_name, _funcs.ToArray());
            }

            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() => _variableFactory.CreateAggregateVariable(null, () => 0.0d))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() => _variableFactory.CreateAggregateVariable(string.Empty, () => 0.0d))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() => _variableFactory.CreateAggregateVariable("  ", () => 0.0d))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Return_Delegate_Variable()
            {
                _variable.Should().BeOfType<DelegateVariable>();
            }

            [Fact]
            public void Should_Have_Name()
            {
                _variable.Name.Should().Be(_name);
            }

            [Fact]
            public void Should_Aggregate_Values()
            {
                var expected = _values.Sum();
                var actual = _variable.Value;

                actual.Should().Be(expected);
            }
        }

        public class CreateAggregateVariable_Names : VariableFactoryFixture
        {
            private readonly int _count = 10;
            private readonly Fake<IVariableRegistry> _registryFake;
            private readonly IEnumerable<string> _names;
            private readonly IEnumerable<double> _values;
            private IVariable _variable;

            public CreateAggregateVariable_Names()
            {
                var variableDictionary = new Dictionary<string, IVariable>();
                _names = CreateMany<string>(_count);
                _values = CreateMany<double>(_count);

                var variables = Enumerable
                    .Range(1, _count)
                    .Select(_ => new Fake<IVariable>())
                    .AsReadOnlyCollection();

                for (var idx = 0; idx < _count; idx++)
                {
                    variables
                        .ElementAt(idx)
                        .CallsTo(fake => fake.Value)
                        .Returns(_values.ElementAt(idx));
                }

                _registryFake = new Fake<IVariableRegistry>();

                for (var idx = 0; idx < _count; idx++)
                {
                    var name = _names.ElementAt(idx);

                    variableDictionary.Add(name, variables.ElementAt(idx).FakedObject);

                    _registryFake
                        .CallsTo(fake => fake.GetValue(name))
                        .Returns(_values.ElementAt(idx));
                }

                _registryFake
                  .CallsTo(fake => ((IEnumerable<KeyValuePair<string, IVariable>>) fake).GetEnumerator())
                  .Returns(variableDictionary.GetEnumerator());

                _variable = _variableFactory.CreateAggregateVariable(_name, _registryFake.FakedObject);
            }

            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() => _variableFactory.CreateAggregateVariable(null, this.CreateStub<IVariableRegistry>(), new[] { "" }))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() => _variableFactory.CreateAggregateVariable(string.Empty, this.CreateStub<IVariableRegistry>(), new[] { "" }))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() => _variableFactory.CreateAggregateVariable("  ", this.CreateStub<IVariableRegistry>(), new[] { "" }))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_VariableRegistry_Null()
            {
                Invoking(() => _variableFactory.CreateAggregateVariable(Create<string>(), null, new[] { "" }))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("variableRegistry");
            }

            [Fact]
            public void Should_Return_Delegate_Variable()
            {
                _variable.Should().BeOfType<DelegateVariable>();
            }

            [Fact]
            public void Should_Have_Name()
            {
                _variable.Name.Should().Be(_name);
            }

            [Fact]
            public void Should_Aggregate_All_Values()
            {
                var expected = _values.Sum();
                var actual = _variable.Value;

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Aggregate_Named_Values()
            {
                var names = _names.Skip(2).Take(3).Skip(2).Take(1).ToList();
                var values = _values.Skip(2).Take(3).Skip(2).Take(1);

                var expected = values.Sum();

                _variable = _variableFactory.CreateAggregateVariable(_name, _registryFake.FakedObject, names);

                var actual = _variable.Value;

                actual.Should().Be(expected);
            }
        }
    }
}
