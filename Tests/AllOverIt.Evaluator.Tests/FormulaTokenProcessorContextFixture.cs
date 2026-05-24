using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Shouldly.Extensions;
using Shouldly;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaTokenProcessorContextFixture : FixtureBase
    {
        private FormulaTokenProcessorContext _context;

        [Fact]
        public void Should_Throw_When_Predicate_Null()
        {
            Should.Throw<ArgumentNullException>(() => _context = new FormulaTokenProcessorContext(null, (p1, p2) => true))
                .WithNamedMessageWhenNull("predicate");
        }

        [Fact]
        public void Should_Throw_When_Processor_Null()
        {
            Should.Throw<ArgumentNullException>(() => _context = new FormulaTokenProcessorContext((p1, p2) => true, null))
                .WithNamedMessageWhenNull("processor");
        }

        [Fact]
        public void Should_Set_Members()
        {
            var func1 = Create<Func<char, bool, bool>>();
            var func2 = Create<Func<char, bool, bool>>();

            _context = new FormulaTokenProcessorContext(func1, func2);

            var expected = new
            {
                Predicate = func1,
                Processor = func2
            };

            expected.ShouldBeEquivalentTo(_context);
        }
    }
}
