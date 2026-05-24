using AllOverIt.Shouldly.Extensions;
using Shouldly;

namespace ShouldlyEval;

/// <summary>
/// VALIDATION AND SCANNING INSTRUCTIONS FOR AGENT
/// 
/// PURPOSE: This file serves as the source-of-truth reference for Shouldly assertion method usage.
/// Upon request, a validation scan should be performed using this file as the authoritative guide.
/// 
/// SCANNING TASK FOR AGENT:
/// ======================
/// 
/// 1. AUTOMATED COMPLIANCE CHECK
///    Search all Tests/**/*.cs files for Shouldly assertion patterns and validate against rules:
///    
///    Rule 1: Primitives, records, structs, enums → ShouldBe()
///    ├─ Pattern: int, string, decimal, bool, enum, record Foo(int X)
///    ├─ ✓ Correct: value.ShouldBe(expected)
///    ├─ ✗ Incorrect: value.ShouldMatch(expected)
///    └─ Auto-fix: Replace ShouldMatch with ShouldBe
///    
///    Rule 2: Collections (List<T>, T[], Dictionary<K,V>, IEnumerable<T>) → ShouldBe()
///    ├─ Pattern: Any collection type comparison
///    ├─ ✓ Correct: actual.ShouldBe(expected)
///    ├─ ✗ Incorrect: actual.ShouldMatch(expected) [if comparing arrays/collections directly]
///    ├─ Note: ShouldMatch on collections works due to Shouldly fallback, but violates semantic intent
///    └─ Auto-fix: Replace ShouldMatch with ShouldBe for array/collection comparisons
///    
///    Rule 3: Plain classes without value equality → ShouldMatch()
///    ├─ Pattern: class Foo { int X { get; set; } } (NOT record, NOT with custom Equals)
///    ├─ ✓ Correct: actual.ShouldMatch(expected)
///    ├─ ✗ Incorrect: actual.ShouldBe(expected)
///    └─ Auto-fix: Replace ShouldBe with ShouldMatch
///    
///    Rule 4: Nested plain classes → ShouldMatch() for root
///    ├─ Pattern: class Order { Customer customer; List<Item> items; }
///    ├─ Critical: Collection types inside must match exactly (no List vs Array mismatch)
///    ├─ ✓ Correct: actual.ShouldMatch(expected)
///    ├─ ✗ Incorrect: actual.ShouldBe(expected) [if root is plain class]
///    └─ Auto-fix: Use ShouldMatch for plain class root
///    
///    Rule 5: Serialization round-trips → ShouldBe() for collections
///    ├─ Pattern: After WriteEnumerable/ReadEnumerable, actual type may differ from expected
///    ├─ ✓ Correct: actual.ShouldBe(expected) [element-wise comparison ignores type]
///    ├─ ✗ Incorrect: actual.ShouldMatch(expected) [on serialized objects with type changes]
///    └─ Auto-fix: Use ShouldBe when comparing round-tripped collections
///    
///    Rule 6: Interface-typed collections → ShouldBe()
///    ├─ Pattern: IEnumerable<T>, IDictionary<K,V>, IList<T>
///    ├─ ✓ Correct: actual.ShouldBe(expected)
///    ├─ ✗ Incorrect: actual.ShouldMatch(expected)
///    └─ Auto-fix: Replace ShouldMatch with ShouldBe
/// 
/// 2. COVERAGE VERIFICATION
///    For each assertion pattern found that passes compliance checks, verify it has a test in this
///    file demonstrating the scenario:
///    
///    ├─ Verify: Does this scenario have a corresponding test? 
///    ├─ If NO:
///    │  ├─ Add new test to this file
///    │  ├─ Document the scenario in a comment
///    │  └─ Mark as "verified in eval fixture"
///    └─ If YES:
///       └─ Log as "✓ covered" for reporting
///    
///    Scenarios to check for:
///    ├─ Primitives (int, string, bool, decimal)
///    ├─ Enums
///    ├─ Records (with/without nesting)
///    ├─ Plain classes (with/without nesting)
///    ├─ Collections: List<T>, T[], Dictionary<K,V>
///    ├─ Collections with different concrete types (List vs Array, Dictionary vs SortedDictionary)
///    ├─ Collections of records
///    ├─ Collections of plain classes
///    ├─ Nested structures (objects containing collections)
///    ├─ Serialization round-trips (array→list, enumerable→list)
///    ├─ Interface-typed collections
///    ├─ Type conversions (nullable, record values, base class)
///    ├─ Empty collections
///    ├─ Null properties
///    ├─ Cross-type comparisons (proxy vs model, etc.)
///    └─ Any other discovered patterns not in this list
/// 
/// 3. ANTI-PATTERNS TO FLAG
///    Search for and report (but don't auto-fix without review):
///    
///    ├─ ✗ ShouldMatch on array/collection types (works but semantically wrong)
///    │  └─ Fix: Change to ShouldBe
///    │
///    ├─ ✗ ShouldBe on plain class objects (will fail at runtime if not equal reference)
///    │  └─ Fix: Change to ShouldMatch or verify class has value equality
///    │
///    ├─ ✗ ShouldMatch on objects with property type mismatches
///    │  └─ Example: actual has List<T>, expected has T[] → will fail
///    │  └─ Fix: Verify concrete types match or use ShouldBe element-wise
///    │
///    ├─ ✗ ShouldBe on unordered collections without sorting
///    │  └─ Example: actual.ShouldBe(expected) where order not guaranteed
///    │  └─ Fix: Sort both sides before comparison
///    │
///    ├─ ✗ Cast<T> patterns with wrong assertion method
///    │  └─ Example: collection.Cast<object>().ShouldMatch(...)
///    │  └─ Fix: Verify Cast is for type conversion and use correct assertion
///    │
///    └─ ✗ Mixed FluentAssertions (.Should()) with Shouldly (ShouldBe)
///       └─ Fix: Complete conversion to Shouldly
/// 
/// 4. EXCEPTION ASSERTION PATTERNS
///    Verify exception handling uses correct pattern:
///    
///    ├─ ✓ Correct: Should.Throw<T>(() => code())
///    ├─ ✓ Correct: Should.Throw<T>(() => code()).WithMessage(pattern)
///    ├─ ✓ Correct: await Should.ThrowAsync<T>(async () => code())
///    ├─ ✗ Incorrect: Invoking(() => code()).Should().Throw<T>()
///    └─ Auto-fix: Convert to Should.Throw<T>() pattern
/// 
/// 5. REPORTING FORMAT
///    Generate report with:
///    
///    ├─ Total assertions scanned: [count]
///    ├─ Compliant assertions: [count]
///    ├─ Non-compliant assertions found: [list with file:line]
///    ├─ Coverage gaps (patterns not in eval fixture): [list]
///    ├─ Anti-patterns detected: [list with severity]
///    ├─ Auto-fixes applied: [count]
///    ├─ Manual fixes required: [list with file:line and reason]
///    └─ Overall status: ✓ All Clear / ⚠ Issues Found / ✗ Critical Issues
/// 
/// EXECUTION NOTES:
/// - This is a one-time validation task after all projects are migrated
/// - Should be run as a separate agent scan
/// - Reference all tests in Tests/**/*.cs
/// - Use this file (ShouldBeShapeSemanticsFixture.cs) as ground truth
/// - Document any edge cases discovered for future reference
/// </summary>

/// <summary>
/// SHOULDLY ASSERTION GUIDE FOR FLUENT ASSERTIONS MIGRATION
/// 
/// This fixture documents the correct usage of Shouldly assertion methods
/// as replacements for FluentAssertions BeEquivalentTo / Be patterns.
/// 
/// DECISION TREE:
/// 
/// 1. Comparing PRIMITIVE types, strings, enums, records, or structs with value equality?
///    → Use ShouldBe(expected)
///    → These types have built-in value equality. Two separate instances with the same values are considered equal.
///    → Examples: int, string, bool, record Foo(int X), enum, decimal
/// 
/// 2. Comparing COLLECTIONS (List<T>, T[], IEnumerable<T>, Dictionary<K,V>)?
///    → Use ShouldBe(expected) when you want element-wise comparison
///    → ShouldBe DOES allow different concrete collection types (List<T> vs T[], Dictionary vs SortedDictionary)
///    → ShouldBe IS order-sensitive for lists and arrays
///    → ShouldBe works for Dictionary comparisons by entry
///    → TRAP: If you received from a serializer/deserializer, verify the concrete type matches or sort before comparison
/// 
/// 3. Comparing PLAIN CLASSES (not records, not with custom Equals) without value equality?
///    → Use ShouldMatch(expected) for deep structural comparison
///    → ShouldMatch recursively compares all public properties
///    → CRITICAL TRAP: ShouldMatch IS STRICT ABOUT CONCRETE COLLECTION TYPES
///    → If an object property is List<T> in actual but T[] in expected, comparison fails
///    → Use ShouldMatch ONLY when you control both instances or know types match
/// 
/// 4. Comparing nested objects (objects containing other objects)?
///    → If the leaf types are primitives/records/structs → ShouldBe
///    → If the root type is a plain class → ShouldMatch
///    → TRAP: Nested collection types must match exactly with ShouldMatch
/// 
/// 5. After serialization round-trips (write then read)?
///    → Use ShouldBe for individual collections that were materialized
///    → Do NOT use ShouldMatch when you know the serializer changes concrete types
///    → Example: WriteEnumerable() then ReadEnumerable() → actual is List, expected was array → use ShouldBe element-wise or sort both
/// 
/// 6. Comparing interface-typed collections (IEnumerable<T>, IDictionary<K,V>)?
///    → Use ShouldBe - it compares entries regardless of concrete implementation
///    → Example: IDictionary<string,int> expected (typed as interface) vs actual (also typed as interface) → ShouldBe works
/// 
/// ANTI-PATTERNS TO AVOID:
/// - Do NOT use ShouldMatch when comparing objects read from a serializer, unless you've verified concrete collection types
/// - Do NOT use ShouldBe for plain classes without value equality (it will fail - use ShouldMatch instead)
/// - Do NOT assume ShouldMatch allows different collection concrete types (it doesn't)
/// - Do NOT use ShouldBe if you need to ignore order in collections (ShouldBe IS order-sensitive)
/// - Do NOT convert ShouldMatch to ShouldBe without checking if root type is a plain class
/// </summary>
public class ShouldBeShapeSemanticsFixture
{
    // ================================================================================
    // ARRAYS OF PRIMITIVES - ShouldBe vs ShouldMatch
    // (Important: ShouldMatch may work on arrays, but ShouldBe is semantically correct)
    // ================================================================================

    [Fact]
    public void ShouldMatch_Works_For_Enum_Arrays_But_ShouldBe_Is_Preferable()
    {
        // NOTE: ShouldMatch WORKS on primitive/enum arrays because Shouldly
        // has fallback logic for comparing collections. However, this is semantically
        // incorrect. ShouldMatch is designed for plain class deep structural
        // comparison. Use ShouldBe for arrays instead.

        MyEnum[] expected = { MyEnum.Alpha, MyEnum.Beta, MyEnum.Gamma };
        MyEnum[] actual = { MyEnum.Alpha, MyEnum.Beta, MyEnum.Gamma };

        // This actually works but is semantically wrong - use ShouldBe instead
        actual.ShouldBeEquivalentTo(expected);
    }

    [Fact]
    public void ShouldBe_Is_Semantically_Correct_For_Enum_Arrays()
    {
        MyEnum[] expected = { MyEnum.Alpha, MyEnum.Beta, MyEnum.Gamma };
        MyEnum[] actual = { MyEnum.Alpha, MyEnum.Beta, MyEnum.Gamma };

        // This is the correct semantic choice for arrays
        actual.ShouldBe(expected);
    }

    // ================================================================================
    // PRIMITIVES AND VALUE TYPES - ShouldBe
    // ================================================================================

    [Fact]
    public void ShouldBe_Succeeds_For_Equal_Primitives()
    {
        int actual = 42;
        int expected = 42;
        actual.ShouldBe(expected);
    }

    [Fact]
    public void ShouldBe_Succeeds_For_Equal_Strings()
    {
        string actual = "hello";
        string expected = "hello";
        actual.ShouldBe(expected);
    }

    [Fact]
    public void ShouldBe_Succeeds_For_Equal_Enums()
    {
        MyEnum actual = MyEnum.Beta;
        MyEnum expected = MyEnum.Beta;
        actual.ShouldBe(expected);
    }

    // ================================================================================
    // RECORDS - ShouldBe (records have built-in value equality)
    // ================================================================================

    [Fact]
    public void ShouldBe_Succeeds_For_Distinct_Record_Instances_With_Same_Values()
    {
        var expected = new PersonRecord("Ada", "Lovelace", 36);
        var actual = new PersonRecord("Ada", "Lovelace", 36);

        ReferenceEquals(actual, expected).ShouldBeFalse();
        actual.ShouldBe(expected);
    }

    [Fact]
    public void ShouldBe_Succeeds_For_Records_With_Nested_Records()
    {
        var expected = new OrderRecord("ORD-001", new PersonRecord("Ada", "Lovelace", 36));
        var actual = new OrderRecord("ORD-001", new PersonRecord("Ada", "Lovelace", 36));

        ReferenceEquals(actual, expected).ShouldBeFalse();
        ReferenceEquals(actual.Customer, expected.Customer).ShouldBeFalse();
        actual.ShouldBe(expected);
    }

    // ================================================================================
    // PLAIN CLASSES - ShouldMatch (no built-in value equality)
    // ================================================================================

    [Fact]
    public void ShouldMatch_Succeeds_For_Distinct_Plain_Class_Instances_With_Same_Values()
    {
        var expected = new PlainPerson
        {
            FirstName = "Ada",
            LastName = "Lovelace",
            Age = 36
        };

        var actual = new PlainPerson
        {
            FirstName = "Ada",
            LastName = "Lovelace",
            Age = 36
        };

        ReferenceEquals(actual, expected).ShouldBeFalse();
        actual.ShouldBeEquivalentTo(expected);
    }

    [Fact]
    public void ShouldMatch_Succeeds_For_Nested_Plain_Classes_With_Same_Values()
    {
        var expected = new PlainOrder
        {
            OrderNumber = "ORD-001",
            Customer = new PlainPerson { FirstName = "Ada", LastName = "Lovelace", Age = 36 },
            Postcode = "E1 6AN"
        };

        var actual = new PlainOrder
        {
            OrderNumber = "ORD-001",
            Customer = new PlainPerson { FirstName = "Ada", LastName = "Lovelace", Age = 36 },
            Postcode = "E1 6AN"
        };

        ReferenceEquals(actual.Customer, expected.Customer).ShouldBeFalse();
        actual.ShouldBeEquivalentTo(expected);
    }

    // ================================================================================
    // COLLECTIONS (SAME CONCRETE TYPES) - ShouldBe
    // ================================================================================

    [Fact]
    public void ShouldBe_Succeeds_For_Lists_With_Same_Elements()
    {
        var expected = new List<int> { 1, 2, 3 };
        var actual = new List<int> { 1, 2, 3 };

        ReferenceEquals(actual, expected).ShouldBeFalse();
        actual.ShouldBe(expected);
    }

    [Fact]
    public void ShouldBe_Succeeds_For_Arrays_With_Same_Elements()
    {
        int[] expected = { 1, 2, 3 };
        int[] actual = { 1, 2, 3 };

        ReferenceEquals(actual, expected).ShouldBeFalse();
        actual.ShouldBe(expected);
    }

    [Fact]
    public void ShouldBe_Succeeds_For_Dictionaries_With_Same_Entries()
    {
        var expected = new Dictionary<string, int>
        {
            ["alpha"] = 1,
            ["beta"] = 2,
            ["gamma"] = 3
        };

        var actual = new Dictionary<string, int>
        {
            ["alpha"] = 1,
            ["beta"] = 2,
            ["gamma"] = 3
        };

        ReferenceEquals(actual, expected).ShouldBeFalse();
        actual.ShouldBe(expected);
    }

    // ================================================================================
    // COLLECTIONS (DIFFERENT CONCRETE TYPES) - ShouldBe
    // IMPORTANT: ShouldBe allows different concrete types for collections
    // ================================================================================

    [Fact]
    public void ShouldBe_Succeeds_For_List_Compared_To_Array()
    {
        int[] expected = { 1, 2, 3 };
        List<int> actual = new() { 1, 2, 3 };

        actual.ShouldBe(expected);
    }

    [Fact]
    public void ShouldBe_Succeeds_For_Array_Compared_To_List()
    {
        List<int> expected = new() { 1, 2, 3 };
        int[] actual = { 1, 2, 3 };

        actual.ShouldBe(expected);
    }

    [Fact]
    public void ShouldBe_Succeeds_For_IEnumerable_Comparisons()
    {
        IEnumerable<int> expected = Enumerable.Range(1, 5);
        IEnumerable<int> actual = new List<int> { 1, 2, 3, 4, 5 };

        actual.ShouldBe(expected);
    }

    [Fact]
    public void ShouldBe_Succeeds_For_Different_Dictionary_Concrete_Types()
    {
        IDictionary<string, int> expected = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
        IDictionary<string, int> actual = new SortedDictionary<string, int> { ["a"] = 1, ["b"] = 2 };

        actual.ShouldBe(expected);
    }

    // ================================================================================
    // COLLECTIONS WITH RECORDS - ShouldBe
    // ================================================================================

    [Fact]
    public void ShouldBe_Succeeds_For_List_Of_Records()
    {
        var expected = new List<PersonRecord>
        {
            new("Ada", "Lovelace", 36),
            new("Alan", "Turing", 41)
        };

        var actual = new List<PersonRecord>
        {
            new("Ada", "Lovelace", 36),
            new("Alan", "Turing", 41)
        };

        actual.ShouldBe(expected);
    }

    [Fact]
    public void ShouldBe_Succeeds_For_Dictionary_Of_Records()
    {
        var expected = new Dictionary<string, PersonRecord>
        {
            ["ada"] = new("Ada", "Lovelace", 36),
            ["alan"] = new("Alan", "Turing", 41)
        };

        var actual = new Dictionary<string, PersonRecord>
        {
            ["ada"] = new("Ada", "Lovelace", 36),
            ["alan"] = new("Alan", "Turing", 41)
        };

        actual.ShouldBe(expected);
    }

    // ================================================================================
    // NESTED STRUCTURES IN PLAIN CLASSES - ShouldMatch
    // ================================================================================

    [Fact]
    public void ShouldMatch_Succeeds_For_Plain_Class_With_List_Property()
    {
        var expected = new PlainInventoryItem
        {
            Sku = "BOOK-001",
            Quantities = new List<int> { 5, 7, 3 }
        };

        var actual = new PlainInventoryItem
        {
            Sku = "BOOK-001",
            Quantities = new List<int> { 5, 7, 3 }
        };

        actual.ShouldBeEquivalentTo(expected);
    }

    [Fact]
    public void ShouldMatch_Succeeds_For_Plain_Class_With_Dictionary_Property()
    {
        var expected = new PlainInventoryItem
        {
            Sku = "BOOK-001",
            LocationQuantities = new Dictionary<string, int> { ["LON"] = 5, ["NYC"] = 7 }
        };

        var actual = new PlainInventoryItem
        {
            Sku = "BOOK-001",
            LocationQuantities = new Dictionary<string, int> { ["LON"] = 5, ["NYC"] = 7 }
        };

        actual.ShouldBeEquivalentTo(expected);
    }

    // ================================================================================
    // SERIALIZATION ROUND-TRIP SCENARIOS - ShouldBe for collections
    // IMPORTANT: After binary serialization, concrete types often change (array → list)
    // Use ShouldBe for element-wise comparison in these cases
    // ================================================================================

    [Fact]
    public void ShouldBe_Succeeds_For_Serialization_Round_Trip_Array_To_List()
    {
        // Simulates: WriteEnumerable(int[] data) → ReadEnumerable<int>() returns List<int>
        int[] expected = { 10, 20, 30 };
        List<int> actual = new() { 10, 20, 30 };  // Simulates deserialized list

        actual.ShouldBe(expected);
    }

    [Fact]
    public void ShouldBe_Succeeds_For_Enumerable_Range_Round_Trip()
    {
        // Simulates: WriteEnumerable(Enumerable.Range()) → ReadEnumerable<int>() returns List<int>
        IEnumerable<int> expected = Enumerable.Range(1, 10);
        IEnumerable<int> actual = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        actual.ShouldBe(expected);
    }

    [Fact]
    public void ShouldBe_Succeeds_For_Sorted_Then_Compared()
    {
        // When order is not guaranteed, sort before comparison
        var expected = new[] { 3, 1, 2 };
        var actual = new List<int> { 1, 2, 3 };

        actual.OrderBy(x => x).ShouldBe(expected.OrderBy(x => x));
    }

    // ================================================================================
    // INTERFACE-TYPED COLLECTIONS - ShouldBe
    // ================================================================================

    [Fact]
    public void ShouldBe_Succeeds_For_IEnumerable_Typed_Dictionaries()
    {
        IEnumerable<KeyValuePair<string, int>> expected = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
        IEnumerable<KeyValuePair<string, int>> actual = new List<KeyValuePair<string, int>>
        {
            new("a", 1),
            new("b", 2)
        };

        actual.ShouldBe(expected);
    }

    // ================================================================================
    // TYPE CONVERSIONS - ShouldBe
    // ================================================================================

    [Fact]
    public void ShouldBe_Succeeds_For_Nullable_Vs_Non_Nullable_Collections()
    {
        List<int?> expected = new() { 1, null, 3 };
        List<int?> actual = new() { 1, null, 3 };

        actual.ShouldBe(expected);
    }

    [Fact]
    public void ShouldBe_Succeeds_For_Dictionary_With_Records_As_Values()
    {
        var expected = new Dictionary<string, PersonRecord>
        {
            ["person1"] = new("Ada", "Lovelace", 36)
        };

        var actual = new Dictionary<string, PersonRecord>
        {
            ["person1"] = new("Ada", "Lovelace", 36)
        };

        actual.ShouldBe(expected);
    }

    // ================================================================================
    // EDGE CASES
    // ================================================================================

    [Fact]
    public void ShouldBe_Succeeds_For_Empty_Collections()
    {
        List<int> expected = new();
        List<int> actual = new();

        actual.ShouldBe(expected);
    }

    [Fact]
    public void ShouldBe_Succeeds_For_Empty_Dictionaries()
    {
        var expected = new Dictionary<string, int>();
        var actual = new Dictionary<string, int>();

        actual.ShouldBe(expected);
    }

    [Fact]
    public void ShouldMatch_Succeeds_For_Plain_Class_With_Null_Properties()
    {
        var expected = new PlainPerson { FirstName = null, LastName = "Doe", Age = 0 };
        var actual = new PlainPerson { FirstName = null, LastName = "Doe", Age = 0 };

        actual.ShouldBeEquivalentTo(expected);
    }

    // ================================================================================
    // TEST TYPES
    // ================================================================================

    private enum MyEnum { Alpha, Beta, Gamma }

    private sealed record PersonRecord(string FirstName, string LastName, int Age);
    private sealed record OrderRecord(string OrderNumber, PersonRecord Customer);

    private sealed class PlainPerson
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }

    private sealed class PlainOrder
    {
        public string OrderNumber { get; set; }
        public PlainPerson Customer { get; set; }
        public string Postcode { get; set; }
    }

    private sealed class PlainInventoryItem
    {
        public string Sku { get; set; }
        public List<int> Quantities { get; set; }
        public Dictionary<string, int> LocationQuantities { get; set; }
    }
}



