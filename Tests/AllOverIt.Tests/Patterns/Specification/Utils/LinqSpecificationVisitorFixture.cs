using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Specification;
using AllOverIt.Patterns.Specification.Utils;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AllOverIt.Tests.Patterns.Specification.Utils
{
    public class LinqSpecificationVisitorFixture : FixtureBase
    {
        private class TypeDummy
        {
            public int Value1 { get; set; }
            public double Value2 { get; set; }
            public string Value3 { get; set; }
            public DateTime Value4 { get; set; }
            public bool Value5 { get; set; }
        }

        private readonly LinqSpecificationVisitor _visitor = new();

        public class AsQueryString : LinqSpecificationVisitorFixture
        {
            [Fact]
            public void Should_Throw_When_Specification_Null()
            {
                Invoking(() =>
                {
                    ILinqSpecification<TypeDummy> specification = null;

                    _ = _visitor.AsQueryString(specification);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("specification");
            }

            [Fact]
            public void Should_Output_EqualTo()
            {
                var value = Create<int>();

                var specification = LinqSpecification<TypeDummy>.Create(item => item.Value1 == value);

                var actual = _visitor.AsQueryString(specification);

                actual.Should().Be($"(Value1 == {value})");
            }

            [Fact]
            public void Should_Output_NotEqualTo()
            {
                var value = Create<int>();

                var specification = LinqSpecification<TypeDummy>.Create(item => item.Value1 != value);

                var actual = _visitor.AsQueryString(specification);

                actual.Should().Be($"(Value1 != {value})");
            }

            [Fact]
            public void Should_Output_GreaterThan()
            {
                var value = Create<int>();

                var specification = LinqSpecification<TypeDummy>.Create(item => item.Value1 > value);

                var actual = _visitor.AsQueryString(specification);

                actual.Should().Be($"(Value1 > {value})");
            }

            [Fact]
            public void Should_Output_GreaterThanEqualTo()
            {
                var value = Create<int>();

                var specification = LinqSpecification<TypeDummy>.Create(item => item.Value1 >= value);

                var actual = _visitor.AsQueryString(specification);

                actual.Should().Be($"(Value1 >= {value})");
            }

            [Fact]
            public void Should_Output_LessThan()
            {
                var value = Create<int>();

                var specification = LinqSpecification<TypeDummy>.Create(item => item.Value1 < value);

                var actual = _visitor.AsQueryString(specification);

                actual.Should().Be($"(Value1 < {value})");
            }

            [Fact]
            public void Should_Output_LessThanEqualTo()
            {
                var value = Create<int>();

                var specification = LinqSpecification<TypeDummy>.Create(item => item.Value1 <= value);

                var actual = _visitor.AsQueryString(specification);

                actual.Should().Be($"(Value1 <= {value})");
            }

            [Fact]
            public void Should_Output_Combine_Two_Operations()
            {
                var value1 = Create<int>();
                var value2 = Create<int>();

                var specification = LinqSpecification<TypeDummy>.Create(item => item.Value1 <= value1 && item.Value2 >= value2);

                var actual = _visitor.AsQueryString(specification);

                actual.Should().Be($"((Value1 <= {value1}) AND (Value2 >= {value2}))");
            }

            [Fact]
            public void Should_Output_Single_Quote_String_Value()
            {
                var value = Create<string>();

                var specification = LinqSpecification<TypeDummy>.Create(item => item.Value3 == value);

                var actual = _visitor.AsQueryString(specification);

                actual.Should().Be($"(Value3 == '{value}')");
            }

            [Fact]
            public void Should_Output_Bool_Comparison()
            {
                var value = Create<bool>();

                // Has to be a comparison, can't just use 'item => item.Value5' as this only retrieves the property name
                var specification = LinqSpecification<TypeDummy>.Create(item => item.Value5 == value);

                var actual = _visitor.AsQueryString(specification);

                actual.Should().Be($"(Value5 == {value})");
            }

            [Fact]
            public void Should_Output_Contains()
            {
                var values = CreateMany<int>().ToList();

                var specification = LinqSpecification<TypeDummy>.Create(item => values.Contains(item.Value1));

                var actual = _visitor.AsQueryString(specification);

                actual.Should().Be($"({string.Join(", ", values)}).Contains(Value1)");
            }

            [Fact]
            public void Should_Output_Chained_Calls()
            {
                var value1 = Create<string>();
                var value2 = Create<string>();

                var specification = LinqSpecification<TypeDummy>.Create(item => value1.ToLower().StartsWith(value2));

                var actual = _visitor.AsQueryString(specification);

                actual.Should().Be($"'{value1}'.ToLower().StartsWith('{value2}')");
            }

            [Fact]
            public void Should_Output_Compare()
            {
                var value1 = Create<string>();
                var value2 = Create<string>();

                var specification = LinqSpecification<TypeDummy>.Create(item => string.Compare(value1, value2) == 0);

                var actual = _visitor.AsQueryString(specification);

                actual.Should().Be($"(Compare('{value1}', '{value2}') == 0)");
            }

            [Fact]
            public void Should_Output_Constant()
            {
                var specification = LinqSpecification<TypeDummy>.Create(item => item.Value1 == 99);

                var actual = _visitor.AsQueryString(specification);

                actual.Should().Be("(Value1 == 99)");
            }

            [Fact]
            public void Should_Output_DateTime_Comparison()
            {
                var value = Create<DateTime>().ToUniversalTime();

                // Has to be a comparison, can't just use 'item => item.Value5' as this only retrieves the property name
                var specification = LinqSpecification<TypeDummy>.Create(item => item.Value4 == value);

                var actual = _visitor.AsQueryString(specification);

                actual.Should().Be($"(Value4 == '{value:yyyy-MM-ddTHH:mm:ss.fffZ}')");
            }

            [Fact]
            public void Should_Output_Not_Two_Operations()
            {
                var value1 = Create<int>();
                var value2 = Create<int>();

                var specification = LinqSpecification<TypeDummy>.Create(item => !(item.Value1 <= value1 && item.Value2 >= value2));

                var actual = _visitor.AsQueryString(specification);

                actual.Should().Be($"NOT (((Value1 <= {value1}) AND (Value2 >= {value2})))");
            }

            [Fact]
            public void Should_Output_Three_Operations_With_A_Not()
            {
                var value1 = Create<int>();
                var value2 = Create<int>();
                var value3 = Create<string>();

                var specification = LinqSpecification<TypeDummy>.Create(item => item.Value3 == value3 && !(item.Value1 <= value1 && item.Value2 >= value2));

                var actual = _visitor.AsQueryString(specification);

                actual.Should().Be($"((Value3 == '{value3}') AND NOT (((Value1 <= {value1}) AND (Value2 >= {value2}))))");
            }

            [Fact]
            public void Should_Output_Combine_Three_Operations()
            {
                var value1 = Create<int>();
                var value2 = Create<int>();
                var value3 = Create<string>();

                var specification = LinqSpecification<TypeDummy>.Create(item => item.Value1 <= value1 && item.Value2 >= value2 || item.Value3 != value3);

                var actual = _visitor.AsQueryString(specification);

                actual.Should().Be($"(((Value1 <= {value1}) AND (Value2 >= {value2})) OR (Value3 != '{value3}'))");
            }
        }
    }
}