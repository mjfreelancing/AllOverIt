using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Specification.Types;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Tests.Patterns.Specification.Types
{
    public class ContainsFixture : SpecificationFixtureBase
    {
        private readonly Contains _specification;
        private readonly string _value;

        public ContainsFixture()
        {
            _value = Create<string>();
            _specification = new Contains(_value);
        }

        public class Constructor : ContainsFixture
        {
            [Fact]
            public void Should_Throw_When_Value_Null()
            {
                Invoking(() =>
                    {
                        _ = new Contains(null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("value");
            }

            [Fact]
            public void Should_Not_Throw_When_Value_Empty()
            {
                Invoking(() =>
                    {
                        _ = new Contains(string.Empty);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("value");
            }

            [Fact]
            public void Should_Not_Throw_When_Value_Whitespace()
            {
                Invoking(() =>
                    {
                        _ = new Contains("  ");
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("value");
            }
        }

        public class DoIsSatisfiedBy : ContainsFixture
        {
            [Fact]
            public void Should_Return_True()
            {
                var actual = _specification.IsSatisfiedBy($"123{_value}123");

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_False()
            {
                var actual = _specification.IsSatisfiedBy(Create<string>());

                actual.Should().BeFalse();
            }
        }
    }
}