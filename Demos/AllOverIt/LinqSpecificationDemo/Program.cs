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
            var isFemale = new IsOfSex(Sex.Female);
            var minimumAge = new IsOfMinimumAge(20);

            var criteria = isMale && minimumAge ||
                           isFemale && !minimumAge;

            Console.WriteLine("Source Data:");
            LogData(Repository.Persons);
            Console.WriteLine();

            Console.WriteLine("Filtered Data as IQueryable<Person> - (Male >= 20 or Female < 20)");
            LogData(Repository.Persons.AsQueryable().Where(criteria));
            Console.WriteLine();

            // Testing inverse criteria by using the not (!) operator on the same criteria
            Console.WriteLine("Filtered Data as IQueryable<Person> - NOT (Male >= 20 or Female < 20)");
            LogData(Repository.Persons.AsQueryable().Where(!criteria));
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
