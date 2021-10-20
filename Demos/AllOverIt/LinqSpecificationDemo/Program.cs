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

            Console.WriteLine("Filtered Data - (Male >= 20 or Female < 20)");
            LogData(Repository.Persons.AsQueryable().Where(criteria));
            Console.WriteLine();

            // Testing inverse criteria by using the not (!) operator on the same criteria
            Console.WriteLine("Filtered Data - NOT (Male >= 20 or Female < 20)");
            LogData(Repository.Persons.AsQueryable().Where(!criteria));
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
