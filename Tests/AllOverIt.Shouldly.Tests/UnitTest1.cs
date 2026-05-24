using Shouldly;

namespace AllOverIt.Shouldly.Tests;

/*
ShouldMatch Implementation Gate Plan
====================================

This fixture is the execution plan and acceptance matrix for the ShouldMatch
implementation. The implementation must progress in phases and each phase must
be green before moving to the next phase.

Phase 0 - Harness
1. Project references and package wiring.
2. Smoke test proving test project can call Shouldly and AllOverIt.Shouldly.

Phase 1 - Core object matching
1. Match concrete object against concrete object by member name/value.
2. Match concrete object against anonymous expected shape.
3. Fail on missing expected member in actual.
4. Support null member values and null roots.

Phase 2 - Recursive and dictionary matching
1. Recursive nested object comparison.
2. Dictionary key/value comparison with recursive value matching.
3. Deterministic path-based failure output for nested mismatches.

Phase 3 - Sequence matching
1. IEnumerable overload support.
2. Strict ordering mode.
3. Any-order mode.

Phase 4 - Options needed for migration parity
1. Excluded members by path.
2. Exclude missing members behavior.
3. Include internal members behavior.
4. Custom comparers and numeric tolerance behavior.

Phase 5 - Canary migration validation
1. Representative tests mirroring current FluentAssertions BeEquivalentTo usage.
2. Confirm behavioral coverage before bulk migration.

Rules
1. Do not skip phases.
2. Do not broaden scope mid-phase.
3. Add tests first for each phase, then implement.
4. Keep all tests green before proceeding.
*/
public class ShouldMatchPlanFixture
{
    [Fact]
    public void Phase0_Smoke_Should_Load_Shouldly_And_Test_Harness()
    {
        _ = Should.Throw<InvalidOperationException>(() =>
        {
            throw new InvalidOperationException("smoke");
        });
    }
}



