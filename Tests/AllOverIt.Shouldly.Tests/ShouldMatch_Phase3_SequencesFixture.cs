using AllOverIt.Shouldly.Extensions;
using Shouldly;

namespace AllOverIt.Shouldly.Tests;

public class ShouldMatch_Phase3_SequencesFixture
{
    private sealed class DummyModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [Fact]
    public void Should_Match_Sequence_In_Strict_Order()
    {
        var actual = new[]
        {
            new DummyModel { Id = 1, Name = "A" },
            new DummyModel { Id = 2, Name = "B" }
        };

        actual.ShouldBeEquivalentTo(new[]
        {
            new { Id = 1, Name = "A" },
            new { Id = 2, Name = "B" }
        });
    }

    [Fact]
    public void Should_Throw_When_Sequence_Order_Differs()
    {
        var actual = new[]
        {
            new DummyModel { Id = 1, Name = "A" },
            new DummyModel { Id = 2, Name = "B" }
        };

        var ex = Should.Throw<ShouldAssertException>(() =>
        {
            actual.ShouldBeEquivalentTo(new[]
            {
                new { Id = 2, Name = "B" },
                new { Id = 1, Name = "A" }
            });
        });

        ex.Message.ShouldContain("$[0].Id");
    }

    [Fact]
    public void Should_Throw_When_Sequence_Count_Differs()
    {
        var actual = new[]
        {
            new DummyModel { Id = 1, Name = "A" }
        };

        var ex = Should.Throw<ShouldAssertException>(() =>
        {
            actual.ShouldBeEquivalentTo(new[]
            {
                new { Id = 1, Name = "A" },
                new { Id = 2, Name = "B" }
            });
        });

        ex.Message.ShouldContain("sequence count mismatch");
    }

}




