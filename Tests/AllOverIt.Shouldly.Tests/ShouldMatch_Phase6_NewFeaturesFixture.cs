using AllOverIt.Shouldly.Extensions;
using Shouldly;

namespace AllOverIt.Shouldly.Tests;

public class ShouldMatch_Phase6_NewFeaturesFixture
{
    private sealed class Item
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    // ========================================================================================
    // ShouldNotMatch for sequences — no options overload
    // ========================================================================================

    [Fact]
    public void ShouldNotMatch_Sequence_Passes_When_Sequences_Differ()
    {
        var actual = new[] { new Item { Id = 1, Name = "A" } };

        actual.ShouldNotBeEquivalentTo(new[] { new { Id = 2, Name = "B" } });
    }

    [Fact]
    public void ShouldNotMatch_Sequence_Throws_When_Sequences_Match()
    {
        var actual = new[] { new Item { Id = 1, Name = "A" } };

        var ex = Should.Throw<ShouldAssertException>(() =>
        {
            actual.ShouldNotBeEquivalentTo(new[] { new { Id = 1, Name = "A" } });
        });

        ex.Message.ShouldContain("ShouldNotBeEquivalentTo failed");
    }

    [Fact]
    public void ShouldNotMatch_Sequence_Passes_When_Counts_Differ()
    {
        var actual = new[] { new Item { Id = 1, Name = "A" } };

        actual.ShouldNotBeEquivalentTo(new[] { new { Id = 1, Name = "A" }, new { Id = 2, Name = "B" } });
    }

    // ========================================================================================
    // ShouldNotMatch for sequences — Action<EquivalenceOptions> overload
    // ========================================================================================

    [Fact]
    public void ShouldNotMatch_Sequence_WithAction_Passes_When_StrictOrder_Differs()
    {
        var actual = new[]
        {
            new Item { Id = 1, Name = "A" },
            new Item { Id = 2, Name = "B" }
        };

        // Default strict ordering — elements in wrong order → does not match → ShouldNotMatch passes
        actual.ShouldNotBeEquivalentTo(new[] { new { Id = 2, Name = "B" }, new { Id = 1, Name = "A" } }, _ => { });
    }

    [Fact]
    public void ShouldNotMatch_Sequence_WithAction_AnyOrder_Throws_When_Sequences_Match()
    {
        var actual = new[]
        {
            new Item { Id = 1, Name = "A" },
            new Item { Id = 2, Name = "B" }
        };

        var ex = Should.Throw<ShouldAssertException>(() =>
        {
            actual.ShouldNotBeEquivalentTo(
                new[] { new { Id = 2, Name = "B" }, new { Id = 1, Name = "A" } },
                options => options.SequenceOrdering = SequenceOrdering.AnyOrder);
        });

        ex.Message.ShouldContain("ShouldNotBeEquivalentTo failed");
    }

    [Fact]
    public void ShouldNotMatch_Sequence_WithAction_AnyOrder_Passes_When_Sequences_Differ()
    {
        var actual = new[]
        {
            new Item { Id = 1, Name = "A" },
            new Item { Id = 2, Name = "B" }
        };

        actual.ShouldNotBeEquivalentTo(
            new[] { new { Id = 3, Name = "C" }, new { Id = 4, Name = "D" } },
            options => options.SequenceOrdering = SequenceOrdering.AnyOrder);
    }

    // ========================================================================================
    // ShouldNotMatch for sequences — EquivalenceOptions overload
    // ========================================================================================

    [Fact]
    public void ShouldNotMatch_Sequence_WithOptions_Passes_When_Sequences_Differ()
    {
        var actual = new[] { new Item { Id = 1, Name = "A" } };
        var options = new EquivalenceOptions();

        actual.ShouldNotBeEquivalentTo(new[] { new { Id = 2, Name = "A" } }, options);
    }

    [Fact]
    public void ShouldNotMatch_Sequence_WithOptions_Throws_When_Sequences_Match()
    {
        var actual = new[] { new Item { Id = 1, Name = "A" } };
        var options = new EquivalenceOptions();

        var ex = Should.Throw<ShouldAssertException>(() =>
        {
            actual.ShouldNotBeEquivalentTo(new[] { new { Id = 1, Name = "A" } }, options);
        });

        ex.Message.ShouldContain("ShouldNotBeEquivalentTo failed");
    }

    // ========================================================================================
    // Custom assertion messages — ShouldMatch scalar overloads
    // ========================================================================================

    [Fact]
    public void ShouldMatch_Scalar_Passes_With_CustomMessage_When_Objects_Match()
    {
        var actual = new Item { Id = 1, Name = "A" };

        // No exception — custom message never appears
        actual.ShouldBeEquivalentTo(new { Id = 1, Name = "A" }, "This message should not appear");
    }

    [Fact]
    public void ShouldMatch_Scalar_CustomMessage_Prepended_On_Failure()
    {
        var actual = new Item { Id = 1, Name = "A" };
        const string customMessage = "Step 5 failed";

        var ex = Should.Throw<ShouldAssertException>(() =>
        {
            actual.ShouldBeEquivalentTo(new { Id = 2, Name = "A" }, customMessage);
        });

        ex.Message.ShouldStartWith(customMessage);
        ex.Message.ShouldContain("ShouldBeEquivalentTo failed");
    }

    [Fact]
    public void ShouldMatch_Scalar_NullCustomMessage_NoPrefix()
    {
        var actual = new Item { Id = 1, Name = "A" };

        string nullMessage = null;

        var ex = Should.Throw<ShouldAssertException>(() =>
        {
            actual.ShouldBeEquivalentTo(new { Id = 2, Name = "A" }, nullMessage);
        });

        ex.Message.ShouldStartWith("ShouldBeEquivalentTo failed");
    }

    [Fact]
    public void ShouldMatch_Scalar_WithAction_CustomMessage_Prepended_On_Failure()
    {
        var actual = new Item { Id = 1, Name = "A" };
        const string customMessage = "Scenario check";

        var ex = Should.Throw<ShouldAssertException>(() =>
        {
            actual.ShouldBeEquivalentTo(new { Id = 2, Name = "A" }, _ => { }, customMessage);
        });

        ex.Message.ShouldStartWith(customMessage);
        ex.Message.ShouldContain("ShouldBeEquivalentTo failed");
    }

    [Fact]
    public void ShouldMatch_Scalar_WithOptions_CustomMessage_Prepended_On_Failure()
    {
        var actual = new Item { Id = 1, Name = "A" };
        const string customMessage = "Verify order";
        var options = new EquivalenceOptions();

        var ex = Should.Throw<ShouldAssertException>(() =>
        {
            actual.ShouldBeEquivalentTo(new { Id = 2, Name = "A" }, options, customMessage);
        });

        ex.Message.ShouldStartWith(customMessage);
        ex.Message.ShouldContain("ShouldBeEquivalentTo failed");
    }

    // ========================================================================================
    // Custom assertion messages — ShouldMatch sequence overloads
    // ========================================================================================

    [Fact]
    public void ShouldMatch_Sequence_Passes_With_CustomMessage_When_Sequences_Match()
    {
        var actual = new[] { new Item { Id = 1, Name = "A" } };

        // No exception — custom message never appears
        actual.ShouldBeEquivalentTo(new[] { new { Id = 1, Name = "A" } }, "This message should not appear");
    }

    [Fact]
    public void ShouldMatch_Sequence_CustomMessage_Prepended_On_Failure()
    {
        var actual = new[] { new Item { Id = 1, Name = "A" } };
        const string customMessage = "Array step failed";

        var ex = Should.Throw<ShouldAssertException>(() =>
        {
            actual.ShouldBeEquivalentTo(new[] { new { Id = 2, Name = "A" } }, customMessage);
        });

        ex.Message.ShouldStartWith(customMessage);
        ex.Message.ShouldContain("ShouldBeEquivalentTo failed");
    }

    [Fact]
    public void ShouldMatch_Sequence_WithAction_CustomMessage_Prepended_On_Failure()
    {
        var actual = new[] { new Item { Id = 1, Name = "A" } };
        const string customMessage = "Collection mismatch";

        var ex = Should.Throw<ShouldAssertException>(() =>
        {
            actual.ShouldBeEquivalentTo(new[] { new { Id = 2, Name = "A" } }, _ => { }, customMessage);
        });

        ex.Message.ShouldStartWith(customMessage);
        ex.Message.ShouldContain("ShouldBeEquivalentTo failed");
    }

    [Fact]
    public void ShouldMatch_Sequence_WithOptions_CustomMessage_Prepended_On_Failure()
    {
        var actual = new[] { new Item { Id = 1, Name = "A" } };
        const string customMessage = "Sequence check";
        var options = new EquivalenceOptions();

        var ex = Should.Throw<ShouldAssertException>(() =>
        {
            actual.ShouldBeEquivalentTo(new[] { new { Id = 2, Name = "A" } }, options, customMessage);
        });

        ex.Message.ShouldStartWith(customMessage);
        ex.Message.ShouldContain("ShouldBeEquivalentTo failed");
    }

    // ========================================================================================
    // Custom assertion messages — ShouldNotMatch scalar overloads
    // ========================================================================================

    [Fact]
    public void ShouldNotMatch_Scalar_Passes_With_CustomMessage_When_Objects_Differ()
    {
        var actual = new Item { Id = 1, Name = "A" };

        // No exception — custom message never appears
        actual.ShouldNotBeEquivalentTo(new { Id = 2, Name = "A" }, "This message should not appear");
    }

    [Fact]
    public void ShouldNotMatch_Scalar_CustomMessage_Prepended_On_Failure()
    {
        var actual = new Item { Id = 1, Name = "A" };
        const string customMessage = "Items should differ";

        var ex = Should.Throw<ShouldAssertException>(() =>
        {
            actual.ShouldNotBeEquivalentTo(new { Id = 1, Name = "A" }, customMessage);
        });

        ex.Message.ShouldStartWith(customMessage);
        ex.Message.ShouldContain("ShouldNotBeEquivalentTo failed");
    }

    [Fact]
    public void ShouldNotMatch_Scalar_WithAction_CustomMessage_Prepended_On_Failure()
    {
        var actual = new Item { Id = 1, Name = "A" };
        const string customMessage = "Expected difference";

        var ex = Should.Throw<ShouldAssertException>(() =>
        {
            actual.ShouldNotBeEquivalentTo(new { Id = 1, Name = "A" }, _ => { }, customMessage);
        });

        ex.Message.ShouldStartWith(customMessage);
        ex.Message.ShouldContain("ShouldNotBeEquivalentTo failed");
    }

    [Fact]
    public void ShouldNotMatch_Scalar_WithOptions_CustomMessage_Prepended_On_Failure()
    {
        var actual = new Item { Id = 1, Name = "A" };
        const string customMessage = "Pre-configured check";
        var options = new EquivalenceOptions();

        var ex = Should.Throw<ShouldAssertException>(() =>
        {
            actual.ShouldNotBeEquivalentTo(new { Id = 1, Name = "A" }, options, customMessage);
        });

        ex.Message.ShouldStartWith(customMessage);
        ex.Message.ShouldContain("ShouldNotBeEquivalentTo failed");
    }

    // ========================================================================================
    // Custom assertion messages — ShouldNotMatch sequence overloads
    // ========================================================================================

    [Fact]
    public void ShouldNotMatch_Sequence_Passes_With_CustomMessage_When_Sequences_Differ()
    {
        var actual = new[] { new Item { Id = 1, Name = "A" } };

        // No exception — custom message never appears
        actual.ShouldNotBeEquivalentTo(new[] { new { Id = 2, Name = "B" } }, "This message should not appear");
    }

    [Fact]
    public void ShouldNotMatch_Sequence_CustomMessage_Prepended_On_Failure()
    {
        var actual = new[] { new Item { Id = 1, Name = "A" } };
        const string customMessage = "Sequence should differ";

        var ex = Should.Throw<ShouldAssertException>(() =>
        {
            actual.ShouldNotBeEquivalentTo(new[] { new { Id = 1, Name = "A" } }, customMessage);
        });

        ex.Message.ShouldStartWith(customMessage);
        ex.Message.ShouldContain("ShouldNotBeEquivalentTo failed");
    }

    [Fact]
    public void ShouldNotMatch_Sequence_WithAction_CustomMessage_Prepended_On_Failure()
    {
        var actual = new[] { new Item { Id = 1, Name = "A" } };
        const string customMessage = "Verify sequences differ";

        var ex = Should.Throw<ShouldAssertException>(() =>
        {
            actual.ShouldNotBeEquivalentTo(new[] { new { Id = 1, Name = "A" } }, _ => { }, customMessage);
        });

        ex.Message.ShouldStartWith(customMessage);
        ex.Message.ShouldContain("ShouldNotBeEquivalentTo failed");
    }

    [Fact]
    public void ShouldNotMatch_Sequence_WithOptions_CustomMessage_Prepended_On_Failure()
    {
        var actual = new[] { new Item { Id = 1, Name = "A" } };
        const string customMessage = "Options check";
        var options = new EquivalenceOptions();

        var ex = Should.Throw<ShouldAssertException>(() =>
        {
            actual.ShouldNotBeEquivalentTo(new[] { new { Id = 1, Name = "A" } }, options, customMessage);
        });

        ex.Message.ShouldStartWith(customMessage);
        ex.Message.ShouldContain("ShouldNotBeEquivalentTo failed");
    }
}
