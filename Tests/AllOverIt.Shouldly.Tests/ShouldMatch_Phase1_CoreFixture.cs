using AllOverIt.Shouldly.Extensions;
using Shouldly;

namespace AllOverIt.Shouldly.Tests;

public class ShouldMatch_Phase1_CoreFixture
{
    private sealed class DummyModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }

    [Fact]
    public void Should_Match_Concrete_Model_By_Public_Properties()
    {
        var expected = new DummyModel
        {
            FirstName = "Jane",
            LastName = "Doe",
            Age = 42
        };

        var actual = new DummyModel
        {
            FirstName = "Jane",
            LastName = "Doe",
            Age = 42
        };

        actual.ShouldBeEquivalentTo(expected);
    }

    [Fact]
    public void Should_Match_Anonymous_Expected_Shape()
    {
        var actual = new DummyModel
        {
            FirstName = "John",
            LastName = "Smith",
            Age = 30
        };

        actual.ShouldBeEquivalentTo(new
        {
            FirstName = "John",
            LastName = "Smith",
            Age = 30
        });
    }

    [Fact]
    public void Should_Throw_When_Expected_Member_Is_Missing_On_Actual()
    {
        var actual = new
        {
            FirstName = "John"
        };

        var ex = Should.Throw<ShouldAssertException>(() =>
        {
            actual.ShouldBeEquivalentTo(new
            {
                FirstName = "John",
                LastName = "Smith"
            });
        });

        ex.Message.ShouldContain("missing member");
        ex.Message.ShouldContain("$.LastName");
    }

    [Fact]
    public void Should_Throw_When_Member_Value_Differs()
    {
        var actual = new DummyModel
        {
            FirstName = "John",
            LastName = "Smith",
            Age = 31
        };

        var ex = Should.Throw<ShouldAssertException>(() =>
        {
            actual.ShouldBeEquivalentTo(new
            {
                FirstName = "John",
                LastName = "Smith",
                Age = 30
            });
        });

        ex.Message.ShouldContain("value mismatch");
        ex.Message.ShouldContain("$.Age");
    }

    [Fact]
    public void Should_Throw_When_Actual_Is_Null_And_Expected_Is_Not_Null()
    {
        DummyModel actual = null;

        var ex = Should.Throw<ShouldAssertException>(() =>
        {
            actual.ShouldBeEquivalentTo(new { FirstName = "John" });
        });

        ex.Message.ShouldContain("actual is null");
    }

    [Fact]
    public void Should_Succeed_When_Both_Actual_And_Expected_Are_Null()
    {
        DummyModel actual = null;
        DummyModel expected = null;

        actual.ShouldBeEquivalentTo(expected);
    }
}




