namespace CustomOperation.Math
{
    public sealed class CustomMath
    {
        public static double CustomMin(double value1, double value2)
        {
            // doesn't take epsilon into account - but this is just for demo purposes
            return value1 < value2
              ? value1
              : value2;
        }

        public static int GreatestCommonDenominator(int value1, int value2)
        {
            //return value2 == 0 
            //  ? value1 
            //  : GreatestCommonDenominator(value2, value1 % value2);

            // using a non-recursive approach to avoid stack overflows
            while (true)
            {
                if (value2 == 0)
                {
                    return value1;
                }

                var value3 = value1;
                value1 = value2;
                value2 = value3 % value2;
            }
        }
    }
}