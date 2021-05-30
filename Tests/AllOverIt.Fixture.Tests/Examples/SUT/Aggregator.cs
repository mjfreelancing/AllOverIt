using System;
using System.Linq;

namespace AllOverIt.Fixture.Tests.Examples.SUT
{
    public class Aggregator : IAggregator
    {
        private readonly ICalculator _calculator;

        public Aggregator(ICalculator calculator)
        {
            _calculator = calculator ?? throw new ArgumentNullException();
        }

        public double Summate(params double[] values)
        {
            return values.Aggregate(0.0d, (prev, next) => _calculator.Add(prev, next));
        }
    }
}