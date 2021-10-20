using AllOverIt.Patterns.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqSpecificationDemo
{
    internal enum Sex
    {
        Male,
        Female
    }

    internal class Person
    {
        public int Age { get; }
        public Sex Sex { get; }
        public string Initials { get; }

        public Person(int age, Sex sex, string initials)
        {
            Age = age;
            Sex = sex;
            Initials = initials;
        }
    }

    internal static class Repository
    {
        public static readonly IEnumerable<Person> Persons = new[]
        {
            new Person(20, Sex.Male, "WE"),
            new Person(18, Sex.Male, "AB"),
            new Person(16, Sex.Female, "GH"),
            new Person(20, Sex.Male, "DE"),
            new Person(24, Sex.Male, "DM"),
            new Person(25, Sex.Female, "CS"),
            new Person(15, Sex.Male, "ML"),
            new Person(20, Sex.Female, "ER"),
            new Person(39, Sex.Female, "NP"),
            new Person(30, Sex.Male, "WY"),
            new Person(18, Sex.Female, "ZS"),
            new Person(20, Sex.Female, "CD"),
            new Person(22, Sex.Male, "HK")
        };
    }

    internal sealed class IsOfSex : LinqSpecification<Person>
    {
        private readonly Sex _sex;

        public IsOfSex(Sex sex)
        {
            _sex = sex;
        }

        public override Expression<Func<Person, bool>> AsExpression()
        {
            return person => person.Sex == _sex;
        }
    }

    // LinqSpecification<T> supports IEnumerable and IQueryable. For IEnumerable only (better performance) use Specification<T>
    internal sealed class IsOfMinimumAge : LinqSpecification<Person>
    {
        private readonly int _age;

        public IsOfMinimumAge(int age)
        {
            _age = age;
        }

        public override Expression<Func<Person, bool>> AsExpression()
        {
            return person => person.Age >= _age;
        }
    }

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

            Console.WriteLine("Filtered Data - (Male >= 20 or Female < 20) - As Enumerable");
            LogData(Repository.Persons.Where(criteria));
            Console.WriteLine();

            Console.WriteLine("Filtered Data - (Male >= 20 or Female < 20) - As Queryable");
            LogData(Repository.Persons.AsQueryable().Where(criteria));
            Console.WriteLine();

            // Testing inverse criteria by using the not (!) operator
            Console.WriteLine("Filtered Data - NOT (Male >= 20 or Female < 20) - As Enumerable");
            LogData(Repository.Persons.Where(!criteria));
            Console.WriteLine();

            Console.WriteLine("Filtered Data - NOT (Male >= 20 or Female < 20) - As Queryable");
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
