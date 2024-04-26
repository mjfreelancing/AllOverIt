namespace AllOverIt.Fixture.Tests.Examples.SUT
{
    public class Calculator : ICalculator
    {
        public double Add(double lhs, double rhs)
        {
            return lhs + rhs;
        }

        public double Divide(double numerator, double denominator)
        {
            if (Math.Abs(denominator) < double.Epsilon)
            {
                throw new DivideByZeroException();
            }

            return numerator / denominator;
        }
    }
}