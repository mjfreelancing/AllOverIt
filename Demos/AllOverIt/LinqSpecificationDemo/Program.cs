using AllOverIt.Patterns.Specification;
using LinqSpecificationDemo.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqSpecificationDemo
{
    class Program
    {
        static void Main()
        {
            var isMale = new IsOfSex(Sex.Male);

            // Alternative to creating a concrete Specification class and using as:
            // var isFemale = new IsOfSex(Sex.Female);
            // Note: Must cast to LinqSpecification<Person> when using this factory method if using operator && or ||
            var isFemale = LinqSpecification<Person>.Create(candidate => candidate.Sex == Sex.Female) as LinqSpecification<Person>;

            var minimumAge = new IsOfMinimumAge(20);

            // Same as isMale.And(minimumAge).Or(isFemale.And(minimumAge.Not()));
            var criteria = isMale && minimumAge ||
                           isFemale && !minimumAge;

            Console.WriteLine("Source Data:");
            LogData(Repository.Persons);
            Console.WriteLine();

            Console.WriteLine("Filtered Data as IQueryable<Person> - (Male >= 20 or Female < 20)");
            LogData(Queryable.Where(Repository.Persons.AsQueryable(), criteria));
            Console.WriteLine();

            // Testing inverse criteria by using the not (!) operator on the same criteria
            Console.WriteLine("Filtered Data as IQueryable<Person> - NOT (Male >= 20 or Female < 20)");
            LogData(Queryable.Where(Repository.Persons.AsQueryable(), !criteria));
            Console.WriteLine();

            Console.WriteLine("====== Do the same tests using an explicit conversion for non-IQueryable based filtering ======");
            Console.WriteLine();

            Console.WriteLine("Filtered Data as Func<Person, bool> - (Male >= 20 or Female < 20)");
            LogData(Repository.Persons.Where((Func<Person, bool>) criteria));
            Console.WriteLine();

            // Testing inverse criteria by using the not (!) operator on the same criteria
            Console.WriteLine("Filtered Data as Func<Person, bool> - NOT (Male >= 20 or Female < 20)");
            LogData(Repository.Persons.Where((Func<Person, bool>) !criteria));
            Console.WriteLine();

            Console.WriteLine("");
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static void LogData(IEnumerable<Person> persons)
        {
            foreach (var person in persons)
            {
                Console.WriteLine($"  {person.Initials} - {person.Sex} - {person.Age}");
            }
        }
    }
}
