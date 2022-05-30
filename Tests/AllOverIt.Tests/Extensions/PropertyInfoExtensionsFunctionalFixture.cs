using AllOverIt.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Tests.Extensions
{
    public class PropertyInfoExtensionsFunctionalFixture : FixtureBase
    {
        public enum Visibility
        {
            Public,
            Protected,
            Private,
            Internal
        }

        private class Person
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Part of the test")]
            private DateTime Timestamp1 { get; } // won't be included in the results

            internal DateTime Timestamp2 { get; } // won't be included in the results
            protected int Id { get; }
            public string FirstName { get; set; }
            public string Surname { get; set; }
            public int Age { get; set; }
            public string FullName => $"{FirstName} {Surname}";
        }

        [Fact]
        public void Should_Get_Default_Property_Value_from_Info()
        {
            var subject = Create<Person>();

            var valueLookup = subject.ToPropertyDictionary(true);

            // just to make sure the loop executes - the default binding used by ToPropertyDictionary() excludes private, protected and internal
            // and the GetPropertyInfo() method below does the same.
            var expectedValues = new[]{ nameof(Person.FirstName), nameof(Person.Surname), nameof(Person.Age), nameof(Person.FullName) };

            expectedValues.Should().BeEquivalentTo(valueLookup.Keys);

            foreach (var propertyName in valueLookup.Keys)
            {
                var actual = typeof(Person).GetPropertyInfo(propertyName).GetValue(subject);
                var expected = valueLookup[propertyName];

                expected.Should().Be(actual);
            }
        }

        [Theory]
        [InlineData("Timestamp1", Visibility.Private)]
        [InlineData("Timestamp2", Visibility.Internal)]
        [InlineData("Id", Visibility.Protected)]
        [InlineData("FirstName", Visibility.Public)]
        public void Should_Get_Expected_Visibility(string propertyName, Visibility visibility)
        {
            var propertyInfo = typeof(Person).GetPropertyInfo(propertyName);

            var actual = visibility switch
            {
                Visibility.Public => propertyInfo.IsPublic(),
                Visibility.Protected => propertyInfo.IsProtected(),
                Visibility.Private => propertyInfo.IsPrivate(),
                Visibility.Internal => propertyInfo.IsInternal(),
                _ => throw new ArgumentOutOfRangeException(nameof(visibility), visibility, null)
            };

            actual.Should().BeTrue();
        }
    }
}