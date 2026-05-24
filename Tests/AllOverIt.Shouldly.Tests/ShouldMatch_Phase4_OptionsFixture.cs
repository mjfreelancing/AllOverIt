using AllOverIt.Shouldly.Extensions;
using Shouldly;

namespace AllOverIt.Shouldly.Tests;

public class ShouldMatch_Phase4_OptionsFixture
{
    private readonly struct CaseInsensitiveCode
    {
        public CaseInsensitiveCode(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override bool Equals(object obj)
        {
            return obj is CaseInsensitiveCode other &&
                   string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
        }
    }

    private sealed class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public float Score { get; set; }
        internal string InternalCode { get; set; }
    }

    [Fact]
    public void Should_Exclude_Member_By_Path()
    {
        var actual = new Person { FirstName = "A", LastName = "B", Age = 30 };
        var expected = new Person { FirstName = "A", LastName = "C", Age = 30 };

        actual.ShouldBeEquivalentTo(expected, options =>
        {
            options.ExcludeMember($"$.{nameof(Person.LastName)}");
        });
    }

    [Fact]
    public void Should_Exclude_Missing_Members_When_Enabled()
    {
        var actual = new
        {
            FirstName = "A"
        };

        actual.ShouldBeEquivalentTo(new
        {
            FirstName = "A",
            LastName = "B"
        }, options =>
        {
            options.ExcludeMissingMembers = true;
        });
    }

    [Fact]
    public void Should_Throw_On_Missing_Members_When_Exclusion_Disabled()
    {
        var actual = new
        {
            FirstName = "A"
        };

        var ex = Should.Throw<ShouldAssertException>(() =>
        {
            actual.ShouldBeEquivalentTo(new
            {
                FirstName = "A",
                LastName = "B"
            }, options =>
            {
                options.ExcludeMissingMembers = false;
            });
        });

        ex.Message.ShouldContain("missing member");
    }

    [Fact]
    public void Should_Match_AnyOrder_When_Configured()
    {
        var actual = new[]
        {
            new Person { FirstName = "A", LastName = "A", Age = 1 },
            new Person { FirstName = "B", LastName = "B", Age = 2 }
        };

        actual.ShouldBeEquivalentTo(new[]
        {
            new { FirstName = "B", LastName = "B", Age = 2 },
            new { FirstName = "A", LastName = "A", Age = 1 }
        }, options =>
        {
            options.SequenceOrdering = SequenceOrdering.AnyOrder;
        });
    }

    [Fact]
    public void Should_Use_Numeric_Tolerance_For_Float()
    {
        var actual = new Person { FirstName = "A", LastName = "B", Age = 1, Score = 10.0004f };
        var expected = new Person { FirstName = "A", LastName = "B", Age = 1, Score = 10.0000f };

        actual.ShouldBeEquivalentTo(expected, options =>
        {
            options.FloatTolerance = 0.001f;
        });
    }

    [Fact]
    public void Should_Use_Custom_Type_Comparer()
    {
        var actual = new Person { FirstName = "a", LastName = "b", Age = 1 };
        var expected = new Person { FirstName = "A", LastName = "B", Age = 1 };

        actual.ShouldBeEquivalentTo(expected, options =>
        {
            options.UseComparer<string>((a, b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase));
        });
    }

    [Fact]
    public void Should_Include_Internal_Members_When_Enabled()
    {
        var actual = new Person { FirstName = "A", LastName = "B", Age = 1, InternalCode = "x" };
        var expected = new Person { FirstName = "A", LastName = "B", Age = 1, InternalCode = "x" };

        actual.ShouldBeEquivalentTo(expected, options =>
        {
            options.IncludeNonPublicMembers = true;
        });
    }

    [Fact]
    public void Should_Fail_On_Internal_Member_Mismatch_When_Internal_Comparison_Enabled()
    {
        var actual = new Person { FirstName = "A", LastName = "B", Age = 1, InternalCode = "x" };
        var expected = new Person { FirstName = "A", LastName = "B", Age = 1, InternalCode = "y" };

        var ex = Should.Throw<ShouldAssertException>(() =>
        {
            actual.ShouldBeEquivalentTo(expected, options =>
            {
                options.IncludeNonPublicMembers = true;
            });
        });

        ex.Message.ShouldContain("InternalCode");
    }

    [Fact]
    public void Should_Allow_Registering_Additional_Leaf_Types()
    {
        var actual = new { Code = new CaseInsensitiveCode("abc") };
        var expected = new { Code = new CaseInsensitiveCode("ABC") };

        actual.ShouldBeEquivalentTo(expected, options =>
        {
            options.TreatAsLeaf<CaseInsensitiveCode>();
        });
    }
}





