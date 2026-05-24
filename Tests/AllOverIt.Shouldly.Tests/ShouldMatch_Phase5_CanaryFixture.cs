using AllOverIt.Shouldly.Extensions;
using Shouldly;

namespace AllOverIt.Shouldly.Tests;

public class ShouldMatch_Phase5_CanaryFixture
{
    private sealed class Breadcrumb
    {
        public string CallerName { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime TimestampUtc { get; set; }
    }

    private sealed class ValidationError
    {
        public string PropertyName { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public object AttemptedValue { get; set; }
        public string PlaceholderOnlyInActual { get; set; }
    }

    [Fact]
    public void Canary_ObservableProxy_Anonymous_Shape_Match()
    {
        var model = new
        {
            FirstName = "Jane",
            LastName = "Doe",
            Age = 22,
            PropertyDummy = new { Value = "abc" }
        };

        model.ShouldBeEquivalentTo(new
        {
            FirstName = "Jane",
            LastName = "Doe",
            Age = 22,
            PropertyDummy = new { Value = "abc" }
        });
    }

    [Fact]
    public void Canary_Breadcrumb_Excluding_Timestamps()
    {
        var expected = new Breadcrumb
        {
            CallerName = "Demo.Caller",
            Message = "Call Site",
            Timestamp = DateTime.MinValue,
            TimestampUtc = DateTime.MinValue
        };

        var actual = new Breadcrumb
        {
            CallerName = "Demo.Caller",
            Message = "Call Site",
            Timestamp = DateTime.UtcNow,
            TimestampUtc = DateTime.UtcNow
        };

        actual.ShouldBeEquivalentTo(expected, options =>
        {
            options.ExcludeMember($"$.{nameof(Breadcrumb.Timestamp)}");
            options.ExcludeMember($"$.{nameof(Breadcrumb.TimestampUtc)}");
        });
    }

    [Fact]
    public void Canary_Validation_ExcludingMissingMembers()
    {
        var actual = new[]
        {
            new ValidationError
            {
                PropertyName = "Value1",
                ErrorCode = "Duplicate",
                ErrorMessage = "Value must be unique",
                AttemptedValue = new[] { "-1", "-2" },
                PlaceholderOnlyInActual = "extra"
            }
        };

        var expected = new[]
        {
            new
            {
                PropertyName = "Value1",
                ErrorCode = "Duplicate",
                ErrorMessage = "Value must be unique",
                AttemptedValue = new[] { "-1", "-2" }
            }
        };

        actual.ShouldBeEquivalentTo(expected, options =>
        {
            options.ExcludeMissingMembers = true;
        });
    }

    [Fact]
    public void Canary_NestedDictionary_With_Tolerances()
    {
        var actual = new
        {
            Prop11 = new Dictionary<string, object>
            {
                ["Prop5"] = 1.00001f,
                ["Prop6"] = 2.00001d,
                ["Prop7"] = 3.00001m
            }
        };

        var expected = new
        {
            Prop11 = new Dictionary<string, object>
            {
                ["Prop5"] = 1.0f,
                ["Prop6"] = 2.0d,
                ["Prop7"] = 3.0m
            }
        };

        actual.ShouldBeEquivalentTo(expected, options =>
        {
            options.FloatTolerance = 0.001f;
            options.DoubleTolerance = 0.001d;
            options.DecimalTolerance = 0.001m;
        });
    }

    [Fact]
    public void Canary_StrictOrdering_For_Event_Sequences()
    {
        var actual = new[] { false, true, false };
        var expected = new[] { false, true, false };

        actual.ShouldBeEquivalentTo(expected, options =>
        {
            options.SequenceOrdering = SequenceOrdering.Strict;
        });

        var ex = Should.Throw<ShouldAssertException>(() =>
        {
            actual.ShouldBeEquivalentTo(new[] { true, false, false }, options =>
            {
                options.SequenceOrdering = SequenceOrdering.Strict;
            });
        });

        ex.Message.ShouldContain("$[0]");
    }
}





