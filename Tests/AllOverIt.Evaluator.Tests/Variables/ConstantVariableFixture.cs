using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using AllOverIt.Shouldly.Extensions;
namespace AllOverIt.Evaluator.Tests.Variables
{
    public class ConstantVariableFixture : FixtureBase
    {
        private readonly string _name;
        private readonly double _value;
        private ConstantVariable _variable;

        public ConstantVariableFixture()
        {
            _name = Create<string>();
            _value = Create<double>();
            _variable = new ConstantVariable(_name, _value);
        }

        public class Constructor : ConstantVariableFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue => _variable = new ConstantVariable(stringValue),
                    "name");
            }

            [Fact]
            public void Should_Set_Members()
            {
                var expected = new
                {
                    Name = _name,
                    Value = _value,
                    VariableRegistry = default(IVariableRegistry),
                    ReferencedVariables = default(IEnumerable<string>)
                };

                expected.ShouldBeEquivalentTo(_variable, opts => opts.ExcludeMember("ReferencedVariables"));
            }

            [Fact]
            public void Should_Set_Default_Value()
            {
                var name = Create<string>();

                _variable = new ConstantVariable(name);

                var expected = new
                {
                    Name = name,
                    Value = default(double),
                    VariableRegistry = default(IVariableRegistry),
                    ReferencedVariables = default(IEnumerable<string>)
                };

                expected.ShouldBeEquivalentTo(_variable, opts => opts.ExcludeMember("ReferencedVariables"));
            }
        }
    }
}




