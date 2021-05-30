using AllOverIt.Fixture;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class AoiFormulaTokenProcessorContextFixture : AoiFixtureBase
    {
        private AoiFormulaTokenProcessorContext _context;

        [Fact]
        public void Should_Throw_When_Predicate_Null()
        {
            Invoking(() => _context = new AoiFormulaTokenProcessorContext(null, (p1, p2) => true))
                .Should()
                .Throw<ArgumentNullException>()
                .WithMessage(GetExpectedArgumentNullExceptionMessage("predicate"));
        }

        [Fact]
        public void Should_Throw_When_Processor_Null()
        {
            Invoking(() => _context = new AoiFormulaTokenProcessorContext((p1, p2) => true, null))
                .Should()
                .Throw<ArgumentNullException>()
                .WithMessage(GetExpectedArgumentNullExceptionMessage("processor"));
        }

        [Fact]
        public void Should_Set_Members()
        {
            var func1 = Create<Func<char, bool, bool>>();
            var func2 = Create<Func<char, bool, bool>>();

            _context = new AoiFormulaTokenProcessorContext(func1, func2);

            _context.Should().BeEquivalentTo(new
            {
                Predicate = func1,
                Processor = func2
            });
        }
    }
}
