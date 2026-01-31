namespace AllOverIt.Patterns.Specification
{
    // Note: tested indirectly via SpecificationFixture

    // ═══════════════════════════════════════════════════════════════════════════════════════════════
    // IMPORTANT: Understanding 'operator true' and 'operator false'
    // ═══════════════════════════════════════════════════════════════════════════════════════════════
    //
    // These operators enable the use of && and || syntax for composing specifications while preventing
    // short-circuit evaluation during composition. This is a well-established C# pattern for building
    // composable query objects (similar to LINQ expression trees).
    //
    // ═══════════════════════════════════════════════════════════════════════════════════════════════
    // How the mechanism works
    // ═══════════════════════════════════════════════════════════════════════════════════════════════
    //
    // When C# encounters: spec1 || spec2
    //
    // Step 1: C# checks if 'operator true(spec1)' is defined
    // Step 2: If defined, C# calls it to determine if short-circuiting should occur
    // Step 3: Since we return 'false', short-circuiting is DISABLED
    // Step 4: C# then evaluates: spec1 | spec2 (calling our 'operator |')
    // Step 5: This creates a new OrSpecification<TType> object (NOT a boolean!)
    //
    // The same logic applies for && using 'operator false' and 'operator &'.
    //
    // ═══════════════════════════════════════════════════════════════════════════════════════════════
    // Why returning 'false' is essential
    // ═══════════════════════════════════════════════════════════════════════════════════════════════
    //
    // KEY CONCEPT: We are building a QUERY EXPRESSION (specification tree), NOT evaluating it.
    //
    // What would happen if 'operator true' returned 'true'?
    // ------------------------------------------------------
    // Code: var combined = isEven || isPositive;
    //
    // Execution steps:
    // 1. C# calls: operator true(isEven)
    // 2. Result: true (hypothetically)
    // 3. Short-circuit: Return isEven directly, NEVER calling operator |
    // 4. Final result: combined = isEven (NOT an OrSpecification!)
    //
    // The Problem:
    // When you later call combined.IsSatisfiedBy(5):
    //   - Only isEven is evaluated: 5 % 2 == 0 → false
    //   - The isPositive check is COMPLETELY LOST
    //   - Expected: false || true = true
    //   - Actual: false (WRONG!)
    //
    // Why can't we make it conditional (like "return this.IsSatisfiedBy(someValue)")?
    // ---------------------------------------------------------------------------------
    // - At composition time, we have NO candidate value to test against
    // - The expression 'spec1 || spec2' happens BEFORE any data exists
    // - Specifications are built ONCE, then reused for MANY different candidates
    // - Evaluation is deferred until IsSatisfiedBy(candidate) is explicitly called
    //
    // By returning 'false' from BOTH 'operator true' and 'operator false':
    // - We guarantee that && ALWAYS calls operator & (creating AndSpecification)
    // - We guarantee that || ALWAYS calls operator | (creating OrSpecification)
    // - Short-circuiting is disabled during composition (but enabled during evaluation)
    // - The full specification tree is preserved for later evaluation
    //
    // ═══════════════════════════════════════════════════════════════════════════════════════════════
    // Examples: Composition vs Evaluation
    // ═══════════════════════════════════════════════════════════════════════════════════════════════
    //
    // Example 1: OR composition
    // -------------------------
    // var spec = isEven || isPositive;       // Returns OrSpecification<int>, NOT a bool
    // bool result = spec.IsSatisfiedBy(5);   // Now we evaluate: false || true = true
    //
    // Example 2: AND composition
    // --------------------------
    // var spec2 = isEven && isPositive;      // Returns AndSpecification<int>, NOT a bool
    // bool result2 = spec2.IsSatisfiedBy(4); // Now we evaluate: true && true = true
    //
    // ═══════════════════════════════════════════════════════════════════════════════════════════════
    // What's actually created under the hood
    // ═══════════════════════════════════════════════════════════════════════════════════════════════
    //
    // When you write: var spec = isEven || isPositive;
    //
    // This creates an object tree:
    //   OrSpecification<int>
    //   ├── LeftSpecification: isEven
    //   └── RightSpecification: isPositive
    //
    // When IsSatisfiedBy(-4) is called:
    //   OrSpecification.IsSatisfiedBy(-4) evaluates:
    //     isEven.IsSatisfiedBy(-4)      → true  (because -4 % 2 == 0)
    //     isPositive.IsSatisfiedBy(-4)  → false (because -4 > 0 is false)
    //     Result: true || false = true
    //
    // When you write: var spec2 = isEven && isPositive;
    //
    // This creates an object tree:
    //   AndSpecification<int>
    //   ├── LeftSpecification: isEven
    //   └── RightSpecification: isPositive
    //
    // When IsSatisfiedBy(4) is called:
    //   AndSpecification.IsSatisfiedBy(4) evaluates:
    //     isEven.IsSatisfiedBy(4)      → true  (because 4 % 2 == 0)
    //     isPositive.IsSatisfiedBy(4)  → true  (because 4 > 0 is true)
    //     Result: true && true = true
    //
    // When IsSatisfiedBy(-4) is called:
    //   AndSpecification.IsSatisfiedBy(-4) evaluates:
    //     isEven.IsSatisfiedBy(-4)      → true  (because -4 % 2 == 0)
    //     isPositive.IsSatisfiedBy(-4)  → false (because -4 > 0 is false)
    //     Result: true && false = false
    //
    // ═══════════════════════════════════════════════════════════════════════════════════════════════
    // Key Takeaways
    // ═══════════════════════════════════════════════════════════════════════════════════════════════
    //
    // 1. The || and && operators create TREE STRUCTURES, not immediate boolean values
    // 2. Returning 'false' ensures both operands are captured in the specification tree
    // 3. Composition happens at design time (building the tree)
    // 4. Evaluation happens at runtime (calling IsSatisfiedBy with actual candidates)
    // 5. This pattern enables deferred execution and composable specifications
    //
    // This same pattern is used in LINQ expression trees, query objects, and other deferred
    // execution scenarios throughout .NET.

    /// <summary>An abstract base class for all concrete specifications.</summary>
    /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
    public abstract class SpecificationBase<TType>
    {
        /// <summary>Implemented by concrete classes to perform the required specification test.</summary>
        /// <param name="candidate">The subject to be tested against the specification.</param>
        /// <returns><see langword="True" /> if the candidate satisfies the specification, otherwise <see langword="False" />.</returns>
        public abstract bool IsSatisfiedBy(TType candidate);

        /// <summary>Required in combination with operator &amp; and | to support operator &amp;&amp; and ||.</summary>
        public static bool operator true(SpecificationBase<TType> _)
        {
            return false;
        }

        /// <summary>Required in combination with operator &amp; and | to support operator &amp;&amp; and ||.</summary>
        public static bool operator false(SpecificationBase<TType> _)
        {
            return false;
        }
    }
}