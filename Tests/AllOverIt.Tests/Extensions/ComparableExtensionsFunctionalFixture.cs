using AllOverIt.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Tests.Extensions
{
    public class ComparableExtensionsFunctionalFixture : FixtureBase
    {
        private class Person : IComparable<Person>
        {
            public string FirstName { get; }
            public string Surname { get; }

            public Person(string firstName, string surname)
            {
                FirstName = firstName;
                Surname = surname;
            }

            public int CompareTo(Person other)
            {
                // compare by surname, then first name, case insensitively
                var surnameComparison = CompareName(Surname, other.Surname);

                return surnameComparison != 0
                  ? surnameComparison
                  : CompareName(FirstName, other.FirstName);
            }

            private static int CompareName(string lhs, string rhs)
            {
                return string.Compare(lhs, rhs, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        [Fact]
        public void Should_Return_Greater_Than_50()
        {
            var numbers = Enumerable.Range(1, 100).AsReadOnlyList();

            var actual = numbers.Where(number => number.GreaterThan(50)).AsReadOnlyList();

            var expected = Enumerable.Range(51, 50).AsReadOnlyList();

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Should_Be_Within_Range()
        {
            var numbers = Enumerable.Range(1, 100).AsReadOnlyList();

            Expression<Func<int, bool>> inRange = value => value.GreaterThanOrEqual(25) && value.LessThanOrEqual(75);

            var expected = Enumerable.Range(25, 51).AsReadOnlyList();

            var filtered = numbers.AsQueryable().Where(inRange).AsReadOnlyList();

            filtered.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Should_Compare_Using_Custom_Comparer()
        {
            var persons = new[]
            {
        new Person("Adam", "Baker"),
        new Person("Adam", "Murphy"),
        new Person("Paul", "Nielson"),
        new Person("Jon", "Nielson")
      };

            // custom comparer will consider them in the following order:
            //   Baker, Adam
            //   Murphy, Adam
            //   Nielson, Jon
            //   Nielson, Paul

            var comparisonPerson = new Person("Amy", "Myer");

            var actual = persons
                .Where(item => item.LessThan(comparisonPerson))
                .AsReadOnlyList();

            actual.Should().BeEquivalentTo(
              new Person("Adam", "Baker"),
              new Person("Adam", "Murphy")
            );
        }

        [Fact]
        public void Should_Compose_Comparable_Expressions()
        {
            var persons = new[]
            {
                new Person("Adam", "Baker"),
                new Person("Adam", "Murphy"),
                new Person("Paul", "Nielson"),
                new Person("Jon", "Nielson")
            };

            var comparisonPerson1 = new Person("Adam", "Murphy");
            var comparisonPerson2 = new Person("Jon", "Nielson");

            var actual = persons
                .Where(
                item => item.LessThan(comparisonPerson1) ||
                        item.GreaterThan(comparisonPerson2))
                .AsReadOnlyList();

            actual.Should().BeEquivalentTo(
              new Person("Adam", "Baker"),
              new Person("Paul", "Nielson")
            );
        }
    }
}