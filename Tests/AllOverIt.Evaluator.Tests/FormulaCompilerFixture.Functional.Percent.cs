using AllOverIt.Evaluator.Extensions;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Evaluator.Variables.Extensions;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalPercent : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Calculate_Percentage_Of_Aggregated_Values()
        {
            // using two different aggregate variable approaches - testing:
            // Percent = 100 * (sum of numbers1) / (sum of numbers1 and numbers2)

            var numbers1 = CreateMany<double>(20);
            var numbers2 = CreateMany<double>(20);

            var numerator = numbers1.Sum();
            var denominator = numerator + numbers2.Sum();

            var expected = 100 * numerator / denominator;

            var factory = new VariableFactory();

            var registry = factory.CreateVariableRegistry();

            var names1 = Enumerable.Range(1, 20).Select(idx => $"x{idx}").ToList();
            var names2 = Enumerable.Range(1, 20).Select(idx => $"y{idx}").ToList();

            for (var i = 0; i < 20; i++)
            {
                var name1 = names1.ElementAt(i);
                registry.AddVariable(new ConstantVariable(name1, numbers1.ElementAt(i)));

                var name2 = names2.ElementAt(i);
                registry.AddVariable(new ConstantVariable(name2, numbers2.ElementAt(i)));
            }

            var sum1 = factory.CreateAggregateVariable("sum1", registry, names1);   // will sum x1 -> x20
            var sum2 = factory.CreateAggregateVariable("sum2", registry);           // will sum x1 -> x20 AND y1 -> y20 (no names, so all variables)

            // Need to use a different registry for sum1, sum2 because if it is in the same as the x, y variables
            // we'll end up with a recursive overflow due to 'sum2' aggregating all variables (including sum1 and sum2).
            // This also demonstrates the above registry can be used independantly (it's only required by the aggregate variable).
            // If the demo was simply calculating (x1 -> x20) / (y1 -> y20) a second registry would not be required.
            var sumRegistry = factory.CreateVariableRegistry();
            sumRegistry.Add(sum1, sum2);

            var compiler = new FormulaCompiler();

            // when sum1 and sum2 are resolved their delegates pull variable values from 'registry'
            var actual = compiler.GetResult("PERC(sum1, sum2)", sumRegistry);

            actual.Should().BeApproximately(expected, 1E-3);
        }
    }
}
