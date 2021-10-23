using AllOverIt.Extensions;
using AllOverIt.Patterns.Specification;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SpecificationDemo
{
    public sealed class IsEven : LinqSpecification<int>
    {
        public override Expression<Func<int, bool>> AsExpression()
        {
            return value => value % 2 == 0;
        }
    }

    class Program
    {
        static void Main()
        {
            #region Define specifications

            var multipleOfTwo = new IsMultipleOf(2);
            var multipleOfThree = new IsMultipleOf(3);
            var multipleOfSeven = new IsMultipleOf(7);

            var twoOrThreeSpecification = multipleOfTwo || multipleOfThree;                     // Same as: multipleOfTwo.Or(multipleOfThree);
            var twoAndThreeSpecification = multipleOfTwo && multipleOfThree;                    // Same as: multipleOfTwo.And(multipleOfThree);
            var complexSpecification = (multipleOfTwo && multipleOfThree) || multipleOfSeven;   // Same as: twoAndThreeSpecification.Or(multipleOfSeven);

            #endregion

            ShowIndividualResults(twoOrThreeSpecification, twoAndThreeSpecification);
            ShowDivisbleAndTest(twoAndThreeSpecification);
            ShowDivisbleOrTest(twoOrThreeSpecification);
            ShowComplexTest(complexSpecification);
            ShowNotComplexTest(complexSpecification);

            Console.WriteLine("");
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        #region Show Simple Specification

        private static void ShowIndividualResults(ISpecification<int> twoOrThreeSpecification, ISpecification<int> twoAndThreeSpecification)
        {
            // test all numbers 1..10 individually
            foreach (var i in Enumerable.Range(1, 10))
            {
                Console.WriteLine($"{i} is divisible by 2 or 3 {twoOrThreeSpecification.IsSatisfiedBy(i)}");
                Console.WriteLine($"{i} is divisible by 2 and 3 = {twoAndThreeSpecification.IsSatisfiedBy(i)}");
                Console.WriteLine("");
            }
        }

        #endregion

        private static void ShowDivisbleAndTest(ISpecification<int> twoAndThreeSpecification)
        {
            Console.WriteLine("");
            Console.WriteLine("Press any key for the next test (list only those divisible by 2 and 3)...");
            Console.ReadKey();
            Console.WriteLine("");

            // find all matches where a value is divisible by 2 and 3
            var twoAndThreeResults = Enumerable.Range(1, 12).Where(twoAndThreeSpecification);

            foreach (var i in twoAndThreeResults)
            {
                Console.WriteLine($"{i} is divisible by 2 and 3");
            }
        }

        private static void ShowDivisbleOrTest(ISpecification<int> twoOrThreeSpecification)
        {
            Console.WriteLine("");
            Console.WriteLine("Press any key for the next test (list only those divisible by 2 or 3)...");
            Console.ReadKey();
            Console.WriteLine("");

            // find all matches where a value is divisible by 2 or 3
            var twoOrThreeResults = Enumerable.Range(1, 12).Where(twoOrThreeSpecification);

            foreach (var i in twoOrThreeResults)
            {
                Console.WriteLine($"{i} is divisible by 2 or 3");
            }
        }

        #region Show Complex Specification

        private static void ShowComplexTest(ISpecification<int> complexSpecification)
        {
            Console.WriteLine("");
            Console.WriteLine("Press any key for the next test (list only those divisible by ((2 and 3) or 7))...");
            Console.ReadKey();
            Console.WriteLine("");

            var results = Enumerable.Range(1, 21).Where(complexSpecification);

            foreach (var i in results)
            {
                Console.WriteLine($"{i} is divisible by ((2 and 3) or 7)");
            }
        }

        #endregion

        private static void ShowNotComplexTest(ISpecification<int> complexSpecification)
        {
            Console.WriteLine("");
            Console.WriteLine("Press any key for the next test (list only those NOT divisible by ((2 and 3) or 7))...");
            Console.ReadKey();
            Console.WriteLine("");

            var notSpecification = complexSpecification.Not();
            var results = Enumerable.Range(1, 21).Where(notSpecification);

            foreach (var i in results)
            {
                Console.WriteLine($"{i} is NOT divisible by ((2 and 3) or 7)");
            }
        }
    }
}
