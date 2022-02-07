using AllOverIt.Exceptions;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Formatters.Objects;
using AllOverIt.Patterns.Enumeration;
using AllOverIt.Reflection;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Extensions
{
    public class ObjectExtensionsFixture : FixtureBase
    {
        private class DummyClassBase
        {
            public int Prop1 { get; set; }
            private int Prop2 { get; set; }
            internal int Prop3 { get; set; }
            internal int Prop4 { get; private set; }
            public string Prop5 { get; set; }

            // used for hash code testing
            public int GetProp2() => Prop2;

            public DummyClassBase()
            {
                Prop1 = 1;
                Prop2 = 2;
                Prop3 = 3;
                Prop4 = 4;
                Prop5 = "5";
            }
        }

        private class DummyClass : DummyClassBase
        {
            public double Prop6 { get; private set; }
            public static bool Prop7 { get; set; }
            public double? Prop8 { get; set; }

            public DummyClass()
            {
                Prop6 = 6.7d;
                Prop7 = true;
            }
        }

        private sealed class CollectionRoot
        {
            internal sealed class RootItem
            {
                public IEnumerable<double> Values { get; set; }
            }

            public IList<RootItem> Items { get; } = new List<RootItem>();

            public IDictionary<int, IEnumerable<RootItem>> Maps { get; } = new Dictionary<int, IEnumerable<RootItem>>();
        }

        private class EnrichedEnumDummy : EnrichedEnum<EnrichedEnumDummy>
        {
            public static readonly EnrichedEnumDummy Value1 = new(1);

            private EnrichedEnumDummy(int value, [CallerMemberName] string name = null)
                : base(value, name)
            {
            }
        }

        public ObjectExtensionsFixture()
        {
            // prevent self-references
            Fixture.Customizations.Add(new PropertyNameOmitter("Prop2", "Prop6", "Prop7", "Prop12"));
        }

        public class ToPropertyDictionary : ObjectExtensionsFixture
        {
            [Fact]
            public void Should_Return_Public_PropertyDictionary()
            {
                var source = Create<DummyClass>();

                var expected = new Dictionary<string, object>
                {
                    {"Prop1", source.Prop1}, {"Prop5", source.Prop5}, {"Prop6", source.Prop6}, {"Prop8", source.Prop8}
                };

                var actual = ObjectExtensions.ToPropertyDictionary(source, true, BindingOptions.Instance | BindingOptions.Public);

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Return_Private_PropertyDictionary()
            {
                var source = Create<DummyClass>();

                var expected = new Dictionary<string, object> {{"Prop2", 2}};

                var actual = ObjectExtensions.ToPropertyDictionary(source, true, BindingOptions.Instance | BindingOptions.Private);

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Return_Protected_PropertyDictionary()
            {
                var source = Create<DummyClass>();

                var expected = new Dictionary<string, object>
                {
                };

                var actual = ObjectExtensions.ToPropertyDictionary(source, true, BindingOptions.Instance | BindingOptions.Protected);

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Return_Static_PropertyDictionary()
            {
                var source = Create<DummyClass>();

                var expected = new Dictionary<string, object> {{"Prop7", true}};

                var actual = ObjectExtensions.ToPropertyDictionary(source, true,
                  BindingOptions.Static | BindingOptions.AllAccessor | BindingOptions.AllVisibility);

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Return_Default_PropertyDictionary()
            {
                var source = new DummyClass();

                var expected = new Dictionary<string, object>
                {
                    {"Prop1", 1},
                    {"Prop5", "5"},
                    {"Prop6", 6.7},
                    {"Prop7", true},
                    {"Prop8", null}
                };

                var actual = ObjectExtensions.ToPropertyDictionary(source, true /*, BindingOptions.Default*/);

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Return_All_PropertyDictionary()
            {
                var source = new DummyClass();

                var expected = new Dictionary<string, object>
                {
                    {"Prop1", 1},
                    {"Prop2", 2},
                    {"Prop3", 3},
                    {"Prop4", 4},
                    {"Prop5", "5"},
                    {"Prop6", 6.7},
                    {"Prop7", true},
                    {"Prop8", null}
                };

                var actual = ObjectExtensions.ToPropertyDictionary(source, true, BindingOptions.All);

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Return_Null_Property_Values()
            {
                var source = new DummyClass();

                var expected = new Dictionary<string, object> {{"Prop1", 1}, {"Prop5", "5"}, {"Prop6", 6.7}, {"Prop8", null}};

                var actual = ObjectExtensions.ToPropertyDictionary(source, true, BindingOptions.Instance | BindingOptions.Public);

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Not_Return_Null_Property_Values()
            {
                var source = new DummyClass();

                var expected = new Dictionary<string, object> {{"Prop1", 1}, {"Prop5", "5"}, {"Prop6", 6.7}};

                var actual = ObjectExtensions.ToPropertyDictionary(source, false, BindingOptions.Instance | BindingOptions.Public);

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Convert_Where_Property_Is_An_Indexer()
            {
                var source = new Dictionary<string, int>
                {
                    {"one", 1},
                    {"two", 2}
                };

                var expected = new Dictionary<string, object>
                {
                    { "Comparer", source.Comparer }, { "Count", source.Count }, { "Keys", source.Keys }, { "Values", source.Values }
                };

                var actual = ObjectExtensions.ToPropertyDictionary(source);

                actual
                    .Should()
                    .BeEquivalentTo(expected);
            }
        }

        public class ToSerializedDictionary : ObjectExtensionsFixture
        {
            private class DummyType
            {
                public int Prop1 { get; set; }
                public DummyType Prop2 { get; set; }
                public Task Prop3 { get; set; }
                public IEnumerable<string> Prop4 { get; set; }
                public IDictionary<int, bool> Prop5 { get; set; }
                public IDictionary<DummyType, string> Prop6 { get; set; }
                public IDictionary<string, DummyType> Prop7 { get; set; }
                public double? Prop8 { get; set; }
                public Action<int, string> Prop9 { get; set; }
                public Func<bool> Prop10 { get; set; }
                public IDictionary<string, Task> Prop11 { get; set; }
                public IDictionary<int, DummyType> Prop12 { get; set; }

#pragma warning disable IDE0052 // Remove unread private members
                private string Prop13 { get; set; }
#pragma warning restore IDE0052 // Remove unread private members

                public string Prop14 { get; set; }

                public DummyType()
                {
                    Prop13 = "13";
                }
            }

            private class DummyTypePropertyNameFilter : ObjectPropertyFilter
            {
                private readonly Func<string, bool> _predicate;

                public DummyTypePropertyNameFilter(Func<string, bool> predicate)
                {
                    _predicate = predicate;
                }

                public override bool OnIncludeProperty()
                {
                    return _predicate.Invoke(Path);
                }
            }

            private class DummyTypePropertyValueFilter : ObjectPropertyFilter
            {
                private readonly string _expectedPath;
                private readonly object _expectedValue;

                public DummyTypePropertyValueFilter(string expectedPath, object expectedValue)
                {
                    _expectedPath = expectedPath;
                    _expectedValue = expectedValue;
                }

                public override bool OnIncludeProperty()
                {
                    return Path == _expectedPath && Value == _expectedValue;
                }
            }

            private class DummyTypePropertyPathFilter : ObjectPropertyFilter, IFormattableObjectPropertyFilter
            {
                public override bool OnIncludeValue()
                {
                    return Path == nameof(DummyType.Prop1);
                }

                public string OnFormatValue(string value)
                {
                    return Path == nameof(DummyType.Prop1)
                        ? "Included"
                        : value;
                }
            }

            private class DummyTypePropertyNameValueFilter : ObjectPropertyFilter, IFormattableObjectPropertyFilter
            {
                public override bool OnIncludeProperty()
                {
                    return Path == nameof(DummyType.Prop1) || Path.StartsWith("Prop2");
                }

                public string OnFormatValue(string value)
                {
                    return Path == nameof(DummyType.Prop1)
                        ? "Included"
                        : value;
                }
            }
          
            private class Typed<TType>
            {
                public TType Prop { get; set; }
            }

            private sealed class DummyWithIndexer
            {
                public string this[int key] => string.Empty;
                public int That { get; set; }
            }

            [Fact]
            public void Should_Serialize_Type_Using_Default_Settings()
            {
                var dummy = Create<DummyType>();

                var actual = dummy.ToSerializedDictionary();

                actual
                    .Should()
                    .BeEquivalentTo(new Dictionary<string, string>
                    {
                        { "Prop1", $"{dummy.Prop1}" },
                        { "Prop4[0]", $"{dummy.Prop4.ElementAt(0)}" },
                        { "Prop4[1]", $"{dummy.Prop4.ElementAt(1)}" },
                        { "Prop4[2]", $"{dummy.Prop4.ElementAt(2)}" },
                        { $"Prop5.{dummy.Prop5.ElementAt(0).Key}", $"{dummy.Prop5.ElementAt(0).Value}" },
                        { $"Prop5.{dummy.Prop5.ElementAt(1).Key}", $"{dummy.Prop5.ElementAt(1).Value}" },
                        { $"Prop5.{dummy.Prop5.ElementAt(2).Key}", $"{dummy.Prop5.ElementAt(2).Value}" },
                        { "Prop8", $"{dummy.Prop8}" },
                        { "Prop14", $"{dummy.Prop14}" }
                    });
            }

            [Fact]
            public void Should_Serialize_Type_Using_Custom_Binding()
            {
                var dummy = Create<DummyType>();

                var options = new ObjectPropertySerializerOptions
                {
                    BindingOptions = BindingOptions.Private | BindingOptions.Instance
                };

                var actual = dummy.ToSerializedDictionary(options);

                actual
                    .Should()
                    .BeEquivalentTo(new Dictionary<string, string>
                    {
                        { "Prop13", "13" }
                    });
            }

            [Fact]
            public void Should_Detect_Self_Reference()
            {
                var dummy1 = Create<DummyType>();
                var dummy2 = Create<DummyType>();

                dummy1.Prop2 = dummy2;
                dummy2.Prop2 = dummy1;

                Invoking(() =>
                    {
                        _ = dummy1.ToSerializedDictionary();
                    })
                    .Should()
                    .Throw<SelfReferenceException>()
                    .WithMessage("Self referencing detected at 'Prop2.Prop2.Prop2' of type 'DummyType'");
            }

            [Fact]
            public void Should_Serialize_Nested_Types()
            {
                var dummy1 = Create<DummyType>();
                var dummy2 = Create<DummyType>();
                var dummy3 = Create<DummyType>();

                dummy1.Prop2 = dummy2;
                dummy2.Prop2 = dummy3;

                var actual = dummy1.ToSerializedDictionary();

                actual
                    .Should()
                    .BeEquivalentTo(new Dictionary<string, string>
                    {
                        { "Prop1", $"{dummy1.Prop1}" },
                        { "Prop4[0]", $"{dummy1.Prop4.ElementAt(0)}" },
                        { "Prop4[1]", $"{dummy1.Prop4.ElementAt(1)}" },
                        { "Prop4[2]", $"{dummy1.Prop4.ElementAt(2)}" },
                        { $"Prop5.{dummy1.Prop5.ElementAt(0).Key}", $"{dummy1.Prop5.ElementAt(0).Value}" },
                        { $"Prop5.{dummy1.Prop5.ElementAt(1).Key}", $"{dummy1.Prop5.ElementAt(1).Value}" },
                        { $"Prop5.{dummy1.Prop5.ElementAt(2).Key}", $"{dummy1.Prop5.ElementAt(2).Value}" },
                        { "Prop8", $"{dummy1.Prop8}" },
                        { "Prop14", $"{dummy1.Prop14}" },

                        { "Prop2.Prop1", $"{dummy2.Prop1}" },
                        { "Prop2.Prop4[0]", $"{dummy2.Prop4.ElementAt(0)}" },
                        { "Prop2.Prop4[1]", $"{dummy2.Prop4.ElementAt(1)}" },
                        { "Prop2.Prop4[2]", $"{dummy2.Prop4.ElementAt(2)}" },
                        { $"Prop2.Prop5.{dummy2.Prop5.ElementAt(0).Key}", $"{dummy2.Prop5.ElementAt(0).Value}" },
                        { $"Prop2.Prop5.{dummy2.Prop5.ElementAt(1).Key}", $"{dummy2.Prop5.ElementAt(1).Value}" },
                        { $"Prop2.Prop5.{dummy2.Prop5.ElementAt(2).Key}", $"{dummy2.Prop5.ElementAt(2).Value}" },
                        { "Prop2.Prop8", $"{dummy2.Prop8}" },
                        { "Prop2.Prop14", $"{dummy2.Prop14}" },

                        { "Prop2.Prop2.Prop1", $"{dummy3.Prop1}" },
                        { "Prop2.Prop2.Prop4[0]", $"{dummy3.Prop4.ElementAt(0)}" },
                        { "Prop2.Prop2.Prop4[1]", $"{dummy3.Prop4.ElementAt(1)}" },
                        { "Prop2.Prop2.Prop4[2]", $"{dummy3.Prop4.ElementAt(2)}" },
                        { $"Prop2.Prop2.Prop5.{dummy3.Prop5.ElementAt(0).Key}", $"{dummy3.Prop5.ElementAt(0).Value}" },
                        { $"Prop2.Prop2.Prop5.{dummy3.Prop5.ElementAt(1).Key}", $"{dummy3.Prop5.ElementAt(1).Value}" },
                        { $"Prop2.Prop2.Prop5.{dummy3.Prop5.ElementAt(2).Key}", $"{dummy3.Prop5.ElementAt(2).Value}" },
                        { "Prop2.Prop2.Prop8", $"{dummy3.Prop8}" },
                        { "Prop2.Prop2.Prop14", $"{dummy3.Prop14}" }
                    });
            }

            [Fact]
            public void Should_Serialize_Null_Values()
            {
                var dummy = new DummyType();

                var options = new ObjectPropertySerializerOptions
                {
                    IncludeNulls = true
                };

                var actual = dummy.ToSerializedDictionary(options);

                actual
                   .Should()
                   .BeEquivalentTo(new Dictionary<string, string>
                   {
                        { "Prop1", "0" },
                        { "Prop2", "<null>" },
                        { "Prop4", "<null>" },
                        { "Prop5", "<null>" },
                        { "Prop6", "<null>" },
                        { "Prop7", "<null>" },
                        { "Prop8", "<null>" },
                        { "Prop11", "<null>" },
                        { "Prop12", "<null>" },
                        { "Prop14", "<null>" }
                   });
            }

            [Fact]
            public void Should_Serialize_Empty_Values()
            {
                var dummy = new DummyType
                {
                    Prop4 = new List<string>(),
                    Prop5 = new Dictionary<int, bool>(),
                    Prop6 = new Dictionary<DummyType, string>(),
                    Prop7 = new Dictionary<string, DummyType>(),
                    Prop11 = new Dictionary<string, Task>(),        // should not be serialized
                    Prop12 = new Dictionary<int, DummyType>()
                };

                var options = new ObjectPropertySerializerOptions
                {
                    IncludeEmptyCollections = true
                };

                var actual = dummy.ToSerializedDictionary(options);

                actual
                    .Should()
                    .BeEquivalentTo(new Dictionary<string, string>
                    {
                        { "Prop1", "0" },
                        { "Prop4", "<empty>" },
                        { "Prop5", "<empty>" },
                        { "Prop6", "<empty>" },
                        { "Prop7", "<empty>" },
                        { "Prop12", "<empty>" }
                    });
            }

            [Fact]
            public void Should_Serialize_Null_And_Empty_Values()
            {
                var dummy = new DummyType
                {
                    Prop4 = new List<string>(),
                    Prop5 = new Dictionary<int, bool>(),
                    Prop6 = new Dictionary<DummyType, string>(),
                    Prop7 = new Dictionary<string, DummyType>(),
                    Prop11 = new Dictionary<string, Task>(),        // should not be serialized
                    Prop12 = new Dictionary<int, DummyType>()
                };

                var options = new ObjectPropertySerializerOptions
                {
                    IncludeNulls = true,
                    IncludeEmptyCollections = true
                };

                var actual = dummy.ToSerializedDictionary(options);

                actual
                    .Should()
                    .BeEquivalentTo(new Dictionary<string, string>
                    {
                        { "Prop1", "0" },
                        { "Prop2", "<null>" },
                        { "Prop4", "<empty>" },
                        { "Prop5", "<empty>" },
                        { "Prop6", "<empty>" },
                        { "Prop7", "<empty>" },
                        { "Prop8", "<null>" },
                        { "Prop12", "<empty>" },
                        { "Prop14", "<null>" }
                    });
            }

            [Fact]
            public void Should_Serialize_Dictionary()
            {
                var dictionary = Create<Dictionary<string, int>>();
                var keys = dictionary.Keys.Select(item => item).ToList();
                var values = dictionary.Values.Select(item => $"{item}").ToList();

                var actual = dictionary.ToSerializedDictionary();

                actual
                    .Should()
                    .BeEquivalentTo(new Dictionary<string, string>
                    {
                        { keys[0], values[0] },
                        { keys[1], values[1] },
                        { keys[2], values[2] }
                    });
            }

            [Fact]
            public void Should_Serialize_Nested_Dictionary()
            {
                var dictionary = Create<Dictionary<string, Dictionary<bool, int>>>();
                var keys = dictionary.Keys.Select(item => item).ToList();

                var actual = dictionary.ToSerializedDictionary();

                actual
                    .Should()
                    .BeEquivalentTo(new Dictionary<string, string>
                    {
                        { $"{keys[0]}.{dictionary[keys[0]].Keys.ElementAt(0)}", $"{dictionary[keys[0]].Values.ElementAt(0)}" },
                        { $"{keys[0]}.{dictionary[keys[0]].Keys.ElementAt(1)}", $"{dictionary[keys[0]].Values.ElementAt(1)}" },

                        { $"{keys[1]}.{dictionary[keys[1]].Keys.ElementAt(0)}", $"{dictionary[keys[1]].Values.ElementAt(0)}" },
                        { $"{keys[1]}.{dictionary[keys[1]].Keys.ElementAt(1)}", $"{dictionary[keys[1]].Values.ElementAt(1)}" },

                        { $"{keys[2]}.{dictionary[keys[2]].Keys.ElementAt(0)}", $"{dictionary[keys[2]].Values.ElementAt(0)}" },
                        { $"{keys[2]}.{dictionary[keys[2]].Keys.ElementAt(1)}", $"{dictionary[keys[2]].Values.ElementAt(1)}" }
                    });
            }

            [Fact]
            public void Should_Serialize_List()
            {
                var list = CreateMany<string>();

                var actual = list.ToSerializedDictionary();

                actual
                    .Should()
                    .BeEquivalentTo(new Dictionary<string, string>
                    {
                        { "[0]", list[0] },
                        { "[1]", list[1] },
                        { "[2]", list[2] },
                        { "[3]", list[3] },
                        { "[4]", list[4] }
                    });
            }

            [Fact]
            public void Should_Serialize_Dictionary_With_Keys_Of_Type_Class_Using_Name_And_Index()
            {
                var dummy = new DummyType
                {
                    Prop6 = new Dictionary<DummyType, string>
                    {
                        { new DummyType(), "one" },
                        { new DummyType(), "two" }
                    }
                };

                var actual = dummy.ToSerializedDictionary();

                actual
                    .Should()
                    .BeEquivalentTo(new Dictionary<string, string>
                    {
                        { "Prop1", "0" },
                        { $"Prop6.{nameof(DummyType)}`0", "one" },
                        { $"Prop6.{nameof(DummyType)}`1", "two" }
                    });
            }

            [Fact]
            public void Should_Serialize_Dictionary_With_Keys_Of_Type_Generic_Using_Friendly_Name_And_Index()
            {
                var dummy = new Dictionary<Typed<DummyType>, bool>()
                {
                    // only the class name (and index) is serialized because it is a key
                    {new Typed<DummyType>{Prop = new DummyType()}, true},
                    {new Typed<DummyType>{Prop = new DummyType()}, false}
                };

                var actual = dummy.ToSerializedDictionary();

                actual
                    .Should()
                    .BeEquivalentTo(new Dictionary<string, string>
                    {
                        { $"{typeof(Typed<DummyType>).GetFriendlyName()}`0", "True" },
                        { $"{typeof(Typed<DummyType>).GetFriendlyName()}`1", "False" }
                    });
            }

            [Fact]
            public void Should_Exclude_Delegates()
            {
                var dummy = new DummyType
                {
                    Prop9 = (_, _) => { },
                    Prop10 = () => true
                };

                var actual = dummy.ToSerializedDictionary();

                actual
                    .Should()
                    .BeEquivalentTo(new Dictionary<string, string>
                    {
                        { "Prop1", "0" }
                    });
            }

            [Fact]
            public void Should_Serialize_Non_Enumerable_With_Indexer()
            {
                var dummy = new DummyWithIndexer();

                var actual = dummy.ToSerializedDictionary();

                actual
                    .Should()
                    .BeEquivalentTo(new Dictionary<string, string>
                    {
                        { "That", "0" }
                    });
            }

            [Fact]
            public void Should_Serialize_Untyped_Dictionary()
            {
                var table = new Hashtable
                {
                    { 1, 1 },
                    //{ "null", null },               // WILL NOT serialize as expected
                    { true, 1 },
                    { 10, "ten" },
                    //{ "list", new List<int>() }     // WILL NOT serialize as expected
                };

                var actual = table.ToSerializedDictionary();

                actual
                    .Should()
                    .BeEquivalentTo(new Dictionary<string, string>
                    {
                        { "1", "1" },
                        //{ "null", string.Empty },
                        { "True", "1" },
                        { "10", "ten" }
                        //{ "list", "System.Collections.Generic.List`1[System.Int32]" }
                    });
            }

            [Fact]
            public void Should_Not_Throw_When_Sharing_Non_Self_Referencing_Data()
            {
                var values = CreateMany<double>();

                var root = new CollectionRoot();

                root.Items.Add(new CollectionRoot.RootItem
                {
                    Values = values
                });

                root.Items.Add(new CollectionRoot.RootItem
                {
                    Values = values
                });

                root.Maps[0] = root.Items;
                root.Maps[1] = root.Items;

                Invoking(() =>
                    {
                        _ = root.ToSerializedDictionary();
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Filter_To_Two_Properties_By_Name()
            {
                var dummy = Create<DummyType>();
                dummy.Prop2 = Create<DummyType>();

                // Prop2 is a class type so to include all values we need to check for "Prop2" as well as "Prop2.XXX"
                // Checking for Prop2 is required to ensure the sub-properties are not filtered out.
                var options = new ObjectPropertySerializerOptions
                {
                    Filter = new DummyTypePropertyNameFilter(name =>
                        name is nameof(DummyType.Prop1) or nameof(DummyType.Prop2) ||
                        name.StartsWith("Prop2.Prop4"))
                };

                var actual = dummy.ToSerializedDictionary(options);

                actual
                    .Should()
                    .BeEquivalentTo(new Dictionary<string, string>
                    {
                        { "Prop1", $"{dummy.Prop1}" },
                        { "Prop2.Prop4[0]", $"{dummy.Prop2.Prop4.ElementAt(0)}" },
                        { "Prop2.Prop4[1]", $"{dummy.Prop2.Prop4.ElementAt(1)}" },
                        { "Prop2.Prop4[2]", $"{dummy.Prop2.Prop4.ElementAt(2)}" }
                    });
            }

            [Fact]
            public void Should_Filter_Out_A_Complete_Nested_Property()
            {
                var dummy1 = Create<DummyType>();
                var dummy2 = Create<DummyType>();
                var dummy3 = Create<DummyType>();

                dummy1.Prop2 = dummy2;
                dummy2.Prop2 = dummy3;

                var options = new ObjectPropertySerializerOptions
                {
                    Filter = new DummyTypePropertyNameFilter(name => name != nameof(DummyType.Prop2))
                };

                var actual = dummy1.ToSerializedDictionary(options);

                actual
                    .Should()
                    .BeEquivalentTo(new Dictionary<string, string>
                    {
                            { "Prop1", $"{dummy1.Prop1}" },
                            { "Prop4[0]", $"{dummy1.Prop4.ElementAt(0)}" },
                            { "Prop4[1]", $"{dummy1.Prop4.ElementAt(1)}" },
                            { "Prop4[2]", $"{dummy1.Prop4.ElementAt(2)}" },
                            { $"Prop5.{dummy1.Prop5.ElementAt(0).Key}", $"{dummy1.Prop5.ElementAt(0).Value}" },
                            { $"Prop5.{dummy1.Prop5.ElementAt(1).Key}", $"{dummy1.Prop5.ElementAt(1).Value}" },
                            { $"Prop5.{dummy1.Prop5.ElementAt(2).Key}", $"{dummy1.Prop5.ElementAt(2).Value}" },
                            { "Prop8", $"{dummy1.Prop8}" },
                            { "Prop14", $"{dummy1.Prop14}" },

                        // the applied filter will result in the following being excluded:
                        //
                        //{ "Prop2.Prop1", $"{dummy2.Prop1}" },
                        //{ "Prop2.Prop4[0]", $"{dummy2.Prop4.ElementAt(0)}" },
                        //{ "Prop2.Prop4[1]", $"{dummy2.Prop4.ElementAt(1)}" },
                        //{ "Prop2.Prop4[2]", $"{dummy2.Prop4.ElementAt(2)}" },
                        //{ $"Prop2.Prop5.{dummy2.Prop5.ElementAt(0).Key}", $"{dummy2.Prop5.ElementAt(0).Value}" },
                        //{ $"Prop2.Prop5.{dummy2.Prop5.ElementAt(1).Key}", $"{dummy2.Prop5.ElementAt(1).Value}" },
                        //{ $"Prop2.Prop5.{dummy2.Prop5.ElementAt(2).Key}", $"{dummy2.Prop5.ElementAt(2).Value}" },
                        //{ "Prop2.Prop8", $"{dummy2.Prop8}" },
                        //{ "Prop2.Prop14", $"{dummy2.Prop14}" },

                        //{ "Prop2.Prop2.Prop1", $"{dummy3.Prop1}" },
                        //{ "Prop2.Prop2.Prop4[0]", $"{dummy3.Prop4.ElementAt(0)}" },
                        //{ "Prop2.Prop2.Prop4[1]", $"{dummy3.Prop4.ElementAt(1)}" },
                        //{ "Prop2.Prop2.Prop4[2]", $"{dummy3.Prop4.ElementAt(2)}" },
                        //{ $"Prop2.Prop2.Prop5.{dummy3.Prop5.ElementAt(0).Key}", $"{dummy3.Prop5.ElementAt(0).Value}" },
                        //{ $"Prop2.Prop2.Prop5.{dummy3.Prop5.ElementAt(1).Key}", $"{dummy3.Prop5.ElementAt(1).Value}" },
                        //{ $"Prop2.Prop2.Prop5.{dummy3.Prop5.ElementAt(2).Key}", $"{dummy3.Prop5.ElementAt(2).Value}" },
                        //{ "Prop2.Prop2.Prop8", $"{dummy3.Prop8}" },
                        //{ "Prop2.Prop2.Prop14", $"{dummy3.Prop14}" }
                    });
            }

            [Fact]
            public void Should_Filter_Out_A_Single_Nested_Property()
            {
                var dummy1 = Create<DummyType>();
                var dummy2 = Create<DummyType>();
                var dummy3 = Create<DummyType>();

                dummy1.Prop2 = dummy2;
                dummy2.Prop2 = dummy3;

                var nameToFilter = $"Prop2.Prop5.{dummy2.Prop5.ElementAt(1).Key}";

                var options = new ObjectPropertySerializerOptions
                {
                    Filter = new DummyTypePropertyNameFilter(name => name != nameToFilter)
                };

                var actual = dummy1.ToSerializedDictionary(options);

                actual
                    .Should()
                    .BeEquivalentTo(new Dictionary<string, string>
                    {
                            { "Prop1", $"{dummy1.Prop1}" },
                            { "Prop4[0]", $"{dummy1.Prop4.ElementAt(0)}" },
                            { "Prop4[1]", $"{dummy1.Prop4.ElementAt(1)}" },
                            { "Prop4[2]", $"{dummy1.Prop4.ElementAt(2)}" },
                            { $"Prop5.{dummy1.Prop5.ElementAt(0).Key}", $"{dummy1.Prop5.ElementAt(0).Value}" },
                            { $"Prop5.{dummy1.Prop5.ElementAt(1).Key}", $"{dummy1.Prop5.ElementAt(1).Value}" },
                            { $"Prop5.{dummy1.Prop5.ElementAt(2).Key}", $"{dummy1.Prop5.ElementAt(2).Value}" },
                            { "Prop8", $"{dummy1.Prop8}" },
                            { "Prop14", $"{dummy1.Prop14}" },
                            { "Prop2.Prop1", $"{dummy2.Prop1}" },
                            { "Prop2.Prop4[0]", $"{dummy2.Prop4.ElementAt(0)}" },
                            { "Prop2.Prop4[1]", $"{dummy2.Prop4.ElementAt(1)}" },
                            { "Prop2.Prop4[2]", $"{dummy2.Prop4.ElementAt(2)}" },
                            { $"Prop2.Prop5.{dummy2.Prop5.ElementAt(0).Key}", $"{dummy2.Prop5.ElementAt(0).Value}" },
                            
                            // the applied filter will result in the following being excluded:
                            // { $"Prop2.Prop5.{dummy2.Prop5.ElementAt(1).Key}", $"{dummy2.Prop5.ElementAt(1).Value}" },

                            { $"Prop2.Prop5.{dummy2.Prop5.ElementAt(2).Key}", $"{dummy2.Prop5.ElementAt(2).Value}" },
                            { "Prop2.Prop8", $"{dummy2.Prop8}" },
                            { "Prop2.Prop14", $"{dummy2.Prop14}" },
                            { "Prop2.Prop2.Prop1", $"{dummy3.Prop1}" },
                            { "Prop2.Prop2.Prop4[0]", $"{dummy3.Prop4.ElementAt(0)}" },
                            { "Prop2.Prop2.Prop4[1]", $"{dummy3.Prop4.ElementAt(1)}" },
                            { "Prop2.Prop2.Prop4[2]", $"{dummy3.Prop4.ElementAt(2)}" },
                            { $"Prop2.Prop2.Prop5.{dummy3.Prop5.ElementAt(0).Key}", $"{dummy3.Prop5.ElementAt(0).Value}" },
                            { $"Prop2.Prop2.Prop5.{dummy3.Prop5.ElementAt(1).Key}", $"{dummy3.Prop5.ElementAt(1).Value}" },
                            { $"Prop2.Prop2.Prop5.{dummy3.Prop5.ElementAt(2).Key}", $"{dummy3.Prop5.ElementAt(2).Value}" },
                            { "Prop2.Prop2.Prop8", $"{dummy3.Prop8}" },
                            { "Prop2.Prop2.Prop14", $"{dummy3.Prop14}" }
                    });
            }

            [Fact]
            public void Should_Filter_To_One_Property_And_Change_Value()
            {
                var dummy = Create<DummyType>();

                var options = new ObjectPropertySerializerOptions
                {
                    Filter = new DummyTypePropertyPathFilter()
                };

                var actual = dummy.ToSerializedDictionary(options);

                actual
                    .Should()
                    .BeEquivalentTo(new Dictionary<string, string>
                    {
                        { "Prop1", "Included" }
                    });
            }

            [Fact]
            public void Should_Filter_To_Property_With_Given_Change_Value()
            {
                var dummy = Create<DummyType>();

                var options = new ObjectPropertySerializerOptions
                {
                    Filter = new DummyTypePropertyValueFilter(nameof(dummy.Prop14), dummy.Prop14)
                };

                var actual = dummy.ToSerializedDictionary(options);

                actual
                    .Should()
                    .BeEquivalentTo(new Dictionary<string, string>
                    {
                        { "Prop14", dummy.Prop14 }
                    });
            }

            [Fact]
            public void Should_Change_One_Property_Value()
            {
                var dummy = Create<DummyType>();
                var dummy2 = Create<DummyType>();
                dummy.Prop2 = dummy2;

                var options = new ObjectPropertySerializerOptions
                {
                    Filter = new DummyTypePropertyNameValueFilter()
                };

                var actual = dummy.ToSerializedDictionary(options);

                actual
                    .Should()
                    .BeEquivalentTo(new Dictionary<string, string>
                    {
                        {"Prop1", "Included"},
                        { "Prop2.Prop1", $"{dummy2.Prop1}" },
                        {"Prop2.Prop4[0]", $"{dummy2.Prop4.ElementAt(0)}"},
                        {"Prop2.Prop4[1]", $"{dummy2.Prop4.ElementAt(1)}"},
                        {"Prop2.Prop4[2]", $"{dummy2.Prop4.ElementAt(2)}"},
                        {$"Prop2.Prop5.{dummy2.Prop5.ElementAt(0).Key}", $"{dummy2.Prop5.ElementAt(0).Value}"},
                        {$"Prop2.Prop5.{dummy2.Prop5.ElementAt(1).Key}", $"{dummy2.Prop5.ElementAt(1).Value}"},
                        {$"Prop2.Prop5.{dummy2.Prop5.ElementAt(2).Key}", $"{dummy2.Prop5.ElementAt(2).Value}"},
                        {"Prop2.Prop8", $"{dummy2.Prop8}"},
                        {"Prop2.Prop14", $"{dummy2.Prop14}"},
                    });
            }

            public class SerializeObjectPropertyFilter : ToSerializedDictionary
            {
                private sealed class DummyTypeTrackingFilter : ObjectPropertyFilter
                {
                    public List<Type> Types { get; } = new();
                    public List<string> Paths { get; } = new();
                    public List<string> Names { get; } = new();
                    public List<int?> Indexes { get; } = new();
                    public List<IReadOnlyCollection<ObjectPropertyParent>> ParentChains { get; } = new();

                    public override bool OnIncludeProperty()
                    {
                        Types.Add(Type);
                        Paths.Add(Path);
                        Names.Add(Name);
                        Indexes.Add(Index);
                        ParentChains.Add(Parents);

                        return true;
                    }
                }

                [Fact]
                public void Should_Track_Types()
                {
                    var dummy = Create<DummyType>();

                    dummy.Prop11 = new Dictionary<string, Task>();      // will be ignored because of Task
                    dummy.Prop12 = new Dictionary<int, DummyType>();    // will include an empty value in the output

                    var dummy2 = Create<DummyType>();
                    dummy.Prop2 = dummy2;

                    // fix types so not being determined as AutoFixture based collections
                    dummy.Prop4 = dummy.Prop4.ToList();
                    dummy.Prop2.Prop4 = dummy.Prop2.Prop4.ToList();

                    var filter = new DummyTypeTrackingFilter();

                    var options = new ObjectPropertySerializerOptions
                    {
                        IncludeEmptyCollections = true,
                        Filter = filter
                    };

                    dummy.ToSerializedDictionary(options);

                    filter.Types.Should().HaveCount(25);

                    // Prop1, Prop2, Prop1, Prop4, , , , Prop5, , , , Prop8, Prop14, Prop4, , , , Prop5, , , , Prop8, Prop12, , Prop14

                    filter.Types
                        .Should()
                        .BeEquivalentTo(new[]
                        {
                            typeof(int),                                        // Prop1
                            typeof(DummyType),                                  // Prop2
                            typeof(int),                                        // Prop2.Prop1
                            typeof(List<string>),                               // Prop2.Prop4
                            typeof(string), typeof(string), typeof(string),     // Prop2.Prop4 elements
                            typeof(Dictionary<int, bool>),                      // Prop2.Prop5
                            typeof(bool), typeof(bool), typeof(bool),           // Prop2.Prop5 elements
                            typeof(double),                                     // Prop2.Prop8
                            typeof(string),                                     // Prop2.Prop14
                            typeof(List<string>),                               // Prop4
                            typeof(string), typeof(string), typeof(string),     // Prop4 elements
                            typeof(Dictionary<int, bool>),                      // Prop5
                            typeof(bool), typeof(bool), typeof(bool),           // Prop5 elements
                            typeof(double),                                     // Prop8
                            typeof(Dictionary<int, DummyType>),                 // Prop12
                            typeof(string),                                     // Empty value for Prop12
                            typeof(string)                                      // Prop14
                        });
                }

                [Fact]
                public void Should_Track_Names()
                {
                    var dummy = Create<DummyType>();

                    dummy.Prop11 = new Dictionary<string, Task>();      // will be ignored because of Task
                    dummy.Prop12 = new Dictionary<int, DummyType>();    // will include an empty value in the output

                    var dummy2 = Create<DummyType>();
                    dummy.Prop2 = dummy2;

                    var filter = new DummyTypeTrackingFilter();

                    var options = new ObjectPropertySerializerOptions
                    {
                        IncludeEmptyCollections = true,
                        Filter = filter
                    };

                    dummy.ToSerializedDictionary(options);

                    filter.Names.Should().HaveCount(25);

                    filter.Names
                        .Should()
                        .BeEquivalentTo(new string[]
                        {
                           "Prop1", "Prop2", "Prop1", "Prop4", null, null, null, "Prop5", null, null, null,
                            "Prop8", "Prop14", "Prop4", null, null, null, "Prop5", null, null, null, "Prop8",
                            "Prop12", null, "Prop14"
                        });
                }

                [Fact]
                public void Should_Track_Paths()
                {
                    var dummy = Create<DummyType>();

                    dummy.Prop11 = new Dictionary<string, Task>();      // will be ignored because of Task
                    dummy.Prop12 = new Dictionary<int, DummyType>();    // will include an empty value in the output

                    var dummy2 = Create<DummyType>();
                    dummy.Prop2 = dummy2;

                    var filter = new DummyTypeTrackingFilter();

                    var options = new ObjectPropertySerializerOptions
                    {
                        IncludeEmptyCollections = true,
                        Filter = filter
                    };

                    dummy.ToSerializedDictionary(options);

                    filter.Paths.Should().HaveCount(25);

                    // Prop12 is listed twice because it includes the root property as well as an <empty> value
                    filter.Paths
                        .Should()
                        .BeEquivalentTo(new string[]
                        {
                            "Prop1", "Prop2", "Prop2.Prop1", "Prop2.Prop4", "Prop2.Prop4[0]", "Prop2.Prop4[1]", "Prop2.Prop4[2]",
                            "Prop2.Prop5", $"Prop2.Prop5.{dummy.Prop2.Prop5.ElementAt(0).Key}",
                            $"Prop2.Prop5.{dummy.Prop2.Prop5.ElementAt(1).Key}", $"Prop2.Prop5.{dummy.Prop2.Prop5.ElementAt(2).Key}",
                            "Prop2.Prop8", "Prop2.Prop14", "Prop4", "Prop4[0]", "Prop4[1]", "Prop4[2]", "Prop5",
                            $"Prop5.{dummy.Prop5.ElementAt(0).Key}", $"Prop5.{dummy.Prop5.ElementAt(1).Key}",
                            $"Prop5.{dummy.Prop5.ElementAt(2).Key}", "Prop8", "Prop12", "Prop12", "Prop14"
                        });
                }

                [Fact]
                public void Should_Track_Indexes()
                {
                    var dummy = Create<DummyType>();

                    dummy.Prop11 = new Dictionary<string, Task>();      // will be ignored because of Task
                    dummy.Prop12 = new Dictionary<int, DummyType>();    // will include an empty value in the output

                    var dummy2 = Create<DummyType>();
                    dummy.Prop2 = dummy2;

                    var filter = new DummyTypeTrackingFilter();

                    var options = new ObjectPropertySerializerOptions
                    {
                        IncludeEmptyCollections = true,
                        Filter = filter
                    };

                    dummy.ToSerializedDictionary(options);

                    filter.Indexes.Should().HaveCount(25);

                    filter.Indexes
                        .Should()
                        .BeEquivalentTo(new int?[]
                        {
                            null, null, null, null, 0, 1, 2, null, 0, 1, 2, null, null, null,
                            0, 1, 2, null, 0, 1, 2, null, null, null, null
                        });
                }
            }
        }

        public class GetPropertyValue_BindingFlags : ObjectExtensionsFixture
        {
            [Fact]
            public void Should_Get_Property_Value_With_Public_BindingFlags()
            {
                var subject = Create<DummyClass>();
                var expected = subject.Prop1;

                var actual = subject.GetPropertyValue<int>(nameof(DummyClass.Prop1), BindingFlags.Instance | BindingFlags.Public);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Get_Property_Value_With_NonPublic_BindingFlags()
            {
                var subject = Create<DummyClass>();
                var expected = subject.GetProp2();

                var actual = subject.GetPropertyValue<int>("Prop2", BindingFlags.Instance | BindingFlags.NonPublic);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Not_Get_Property_Value()
            {
                var propertyName = Create<string>();

                Invoking(() =>
                  {
                      var subject = Create<DummyClass>();

                      subject.GetPropertyValue<int>(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
                  })
                  .Should()
                  .Throw<MemberAccessException>()
                  .WithMessage($"The property '{propertyName}' was not found");
            }
        }
        public class GetPropertyValue_BindingOptions : ObjectExtensionsFixture
        {
            [Fact]
            public void Should_Get_Property_Value_With_Default_BindingOptions()
            {
                var subject = Create<DummyClass>();
                var expected = subject.Prop1;

                var actual = subject.GetPropertyValue<int>(nameof(DummyClass.Prop1));

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Get_Property_Value_With_Private_BindingOptions()
            {
                var subject = Create<DummyClass>();
                var expected = subject.GetProp2();

                var actual = subject.GetPropertyValue<int>("Prop2", BindingOptions.Instance | BindingOptions.Private);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Not_Get_Property_Value()
            {
                var propertyName = Create<string>();

                Invoking(() =>
                  {
                      var subject = Create<DummyClass>();

                      subject.GetPropertyValue<int>(propertyName, BindingOptions.Instance | BindingOptions.Private);
                  })
                  .Should()
                  .Throw<MemberAccessException>()
                  .WithMessage($"The property '{propertyName}' was not found");
            }
        }

        public class SetPropertyValue_BindingFlags : ObjectExtensionsFixture
        {
            [Fact]
            public void Should_Set_Property_Value_With_Public_BindingFlags()
            {
                var subject = new DummyClass();
                var expected = Create<int>();

                subject.SetPropertyValue(nameof(DummyClass.Prop1), expected, BindingFlags.Instance | BindingFlags.Public);

                var actual = subject.Prop1;

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Set_Property_Value_With_NonPublic_BindingFlags()
            {
                var subject = new DummyClass();
                var expected = Create<int>();

                subject.SetPropertyValue("Prop2", expected, BindingFlags.Instance | BindingFlags.NonPublic);

                var actual = subject.GetProp2();

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Not_Set_Property_Value()
            {
                var propertyName = Create<string>();

                Invoking(() =>
                  {
                      var subject = new DummyClass();

                      subject.SetPropertyValue(propertyName, Create<int>(), BindingFlags.Instance | BindingFlags.NonPublic);
                  })
                  .Should()
                  .Throw<MemberAccessException>()
                  .WithMessage($"The property '{propertyName}' was not found");
            }
        }

        public class SetPropertyValue_BindingOptions : ObjectExtensionsFixture
        {
            [Fact]
            public void Should_Set_Property_Value_With_Default_BindingFlags()
            {
                var subject = new DummyClass();
                var expected = Create<int>();

                subject.SetPropertyValue(nameof(DummyClass.Prop1), expected);

                var actual = subject.Prop1;

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Set_Property_Value_With_Private_BindingFlags()
            {
                var subject = new DummyClass();
                var expected = Create<int>();

                subject.SetPropertyValue("Prop2", expected, BindingOptions.Instance | BindingOptions.Private);

                var actual = subject.GetProp2();

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Not_Set_Property_Value()
            {
                var propertyName = Create<string>();

                Invoking(() =>
                  {
                      var subject = new DummyClass();

                      subject.SetPropertyValue(propertyName, Create<int>(), BindingOptions.Instance | BindingOptions.Private);
                  })
                  .Should()
                  .Throw<MemberAccessException>()
                  .WithMessage($"The property '{propertyName}' was not found");
            }
        }

        public class IsIntegral : ObjectExtensionsFixture
        {
            [Theory]
            [InlineData((byte)0, true)]
            [InlineData((sbyte)0, true)]
            [InlineData((short)0, true)]
            [InlineData((ushort)0, true)]
            [InlineData(0, true)]
            [InlineData((uint)0, true)]
            [InlineData((long)0, true)]
            [InlineData((ulong)0, true)]
            [InlineData(0.0d, false)]
            [InlineData(0.0f, false)]
            [InlineData("some value", false)]
            public void Should_Determine_If_Integral(object value, bool expected)
            {
                var actual = ObjectExtensions.IsIntegral(value);

                actual.Should().Be(expected);
            }
        }

        public class As : ObjectExtensionsFixture
        {
            private class DummyUnrelatedClass
            {
            }

            public enum DummyEnum : short
            {
                Dummy1, Dummy2, Dummy3
            }

            [Fact]
            public void Should_Return_String_Default_When_Null()
            {
                var actual = ObjectExtensions.As<string>((object)null);

                actual.Should().Be(default);
            }

            [Fact]
            public void Should_Return_Int_Default_When_Null()
            {
                var actual = ObjectExtensions.As<int>((object)null);

                actual.Should().Be(default);
            }

            [Fact]
            public void Should_Return_Explicit_Default_When_Null()
            {
                var expected = Create<int>();
                var actual = ObjectExtensions.As(null, expected);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Return_Same_Instance_As_Base_Class()
            {
                var expected = Create<DummyClass>();

                var actual = ObjectExtensions.As<DummyClassBase>(expected);

                actual.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Return_Same_Instance_As_Derived_Class()
            {
                var expected = Create<DummyClass>();

                var actual = ObjectExtensions.As<DummyClass>((DummyClassBase)expected);

                actual.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Return_Same_Object()
            {
                var expected = Create<DummyClass>();

                var actual = ObjectExtensions.As<object>(expected);

                actual.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Throw_When_Invalid_Cast()
            {
                Invoking(() => ObjectExtensions.As<DummyUnrelatedClass>(Create<DummyClass>()))
                  .Should()
                  .Throw<InvalidCastException>()
                  .WithMessage("Unable to cast object of type 'DummyClass' to type 'DummyUnrelatedClass'.");
            }

            [Fact]
            public void Should_Not_Convert_Positive_Integral_To_Boolean()
            {
                var value = CreateExcluding<short>(0, 1);

                if (value < 0)
                {
                    value = (short)(-value);
                }

                Invoking(() => ObjectExtensions.As<bool>(value))
                  .Should()
                  .Throw<ArgumentOutOfRangeException>()
                  .WithMessage($"Cannot convert integral '{value}' to a Boolean. (Parameter 'instance')");
            }

            [Fact]
            public void Should_Not_Convert_Negative_Integral_To_Boolean()
            {
                var value = -CreateExcluding<short>(0, 1);

                if (value > 0)
                {
                    value = (short)(-value);
                }

                Invoking(() => ObjectExtensions.As<bool>(value))
                  .Should()
                  .Throw<ArgumentOutOfRangeException>()
                  .WithMessage($"Cannot convert integral '{value}' to a Boolean. (Parameter 'instance')");
            }

            [Theory]
            [InlineData((short)0, false)]
            [InlineData((short)1, true)]
            [InlineData(0, false)]
            [InlineData(1, true)]
            [InlineData(0L, false)]
            [InlineData(1L, true)]
            public void Should_Convert_Integral_To_Boolean(object value, bool expected)
            {
                var actual = ObjectExtensions.As<bool>(value);

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData(DummyEnum.Dummy1)]    // underyling type is a short
            [InlineData(DummyEnum.Dummy2)]
            [InlineData(DummyEnum.Dummy3)]
            public void Should_Convert_Enum_To_Integer(DummyEnum value)
            {
                var expected = (int)value;

                var actual = ObjectExtensions.As<int>(value);

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData(DummyEnum.Dummy1)]    // underyling type is a short
            [InlineData(DummyEnum.Dummy2)]
            [InlineData(DummyEnum.Dummy3)]
            public void Should_Convert_Enum_To_Short(DummyEnum value)
            {
                var expected = (short)value;

                var actual = ObjectExtensions.As<short>(value);

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData(0, DummyEnum.Dummy1)]    // underyling type is a short
            [InlineData(1, DummyEnum.Dummy2)]
            [InlineData(2, DummyEnum.Dummy3)]
            public void Should_Convert_Integer_To_Enum(int value, DummyEnum expected)
            {
                var actual = ObjectExtensions.As<DummyEnum>(value);

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData(0, DummyEnum.Dummy1)]    // underyling type is a short
            [InlineData(1, DummyEnum.Dummy2)]
            [InlineData(2, DummyEnum.Dummy3)]
            public void Should_Convert_Short_To_Enum(short value, DummyEnum expected)
            {
                var actual = ObjectExtensions.As<DummyEnum>(value);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Not_Convert_Integral_To_Enum()
            {
                var value = Create<int>() + 100;

                Invoking(() => ObjectExtensions.As<DummyEnum>(value))
                  .Should()
                  .Throw<ArgumentOutOfRangeException>()
                  .WithMessage($"Cannot cast '{value}' to a '{typeof(DummyEnum)}' value. (Parameter 'instance')");
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Convert_Bool_To_Bool(bool expected)
            {
                var actual = ObjectExtensions.As<bool>(expected);

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData(false, 0)]
            [InlineData(true, 1)]
            public void Should_Convert_Bool_To_Int(bool value, int expected)
            {
                var actual = ObjectExtensions.As<int>(value);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Convert_Char_To_Int()
            {
                var value = ' ';
                var actual = ObjectExtensions.As<int>(value);

                actual.Should().Be(32);
            }

            [Fact]
            public void Should_Convert_Int_To_Char()
            {
                var value = 32;
                var actual = ObjectExtensions.As<char>(value);

                actual.Should().Be(' ');
            }

            [Theory]
            [InlineData(100, "100")]
            [InlineData(5.7, "5.7")]
            [InlineData(DummyEnum.Dummy2, "Dummy2")]
            [InlineData(true, "True")]
            [InlineData(false, "False")]
            public void Should_Convert_To_String(object value, string expected)
            {
                var actual = ObjectExtensions.As<string>(value);

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData("True", true)]
            [InlineData("true", true)]
            [InlineData("False", false)]
            [InlineData("false", false)]
            public void Should_Convert_String_To_Bool(string value, bool expected)
            {
                var actual = ObjectExtensions.As<bool>(value);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Convert_String_To_Integral()
            {
                var actual = ObjectExtensions.As<int>("100");

                actual.Should().Be(100);
            }

            [Fact]
            public void Should_Convert_String_To_Enum()
            {
                // there is a StringExtensions version, but this should also work
                var actual = ObjectExtensions.As<DummyEnum>("Dummy2");

                actual.Should().Be(DummyEnum.Dummy2);
            }
        }

        public class AsNullable : ObjectExtensionsFixture
        {
            [Fact]
            public void Should_Return_Null_When_Null()
            {
                int? value = null;

                var actual = ObjectExtensions.AsNullable<int>(value);

                actual.Should().Be(null);
            }

            [Fact]
            public void Should_Return_Same_Value()
            {
                var value = Create<int?>();

                var actual = ObjectExtensions.AsNullable<int>(value);

                actual.Should().Be(value);
            }

            [Fact]
            public void Should_Return_Specified_Default_Value()
            {
                var expected = Create<int?>();

                var actual = ObjectExtensions.AsNullable((int?)null, expected);

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData(1, true)]
            [InlineData(0, false)]
            public void Should_Return_Converted_Value(int? value, bool expected)
            {
                var actual = ObjectExtensions.AsNullable<bool>(value);

                actual.Should().Be(expected);
            }
        }

        public class CalculateHashCode : ObjectExtensionsFixture
        {
            [Fact]
            public void Should_Have_Well_Known_Default_HashCode_Binding_option()
            {
                var expected = BindingOptions.Instance | BindingOptions.AllAccessor | BindingOptions.AllVisibility;

                var actual = ObjectExtensions.DefaultHashCodeBindings;

                actual.Should().Be(expected);
            }
        }

        public class CalculateHashCode_By_Name : ObjectExtensionsFixture
        {
            [Fact]
            public void Should_Be_Equivalent_To_Default_HashCode_Binding()
            {
                var subject = Create<DummyClass>();

                var expected = GetCumulativeHash(subject, ObjectExtensions.DefaultHashCodeBindings);

                var actual = ObjectExtensions.CalculateHashCode(subject);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Be_Equivalent_To_Non_Static_Properties()
            {
                var subject = Create<DummyClass>();

                // Prop7 is static - the default binding excludes statics
                var expected = ObjectExtensions.CalculateHashCode(subject,
                  model => model.Prop1, model => model.GetProp2(), model => model.Prop3, model => model.Prop4,
                  model => model.Prop5, model => model.Prop6, model => model.Prop8
                );

                var actual = ObjectExtensions.CalculateHashCode(subject);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Support_Null_Values()
            {
                var subject = Create<DummyClass>();
                subject.Prop8 = null;

                // Prop7 is static - the default binding excludes statics
                var expected = ObjectExtensions.CalculateHashCode(subject,
                  model => model.Prop1, model => model.GetProp2(), model => model.Prop3, model => model.Prop4,
                  model => model.Prop5, model => model.Prop6, model => model.Prop8
                );

                var actual = ObjectExtensions.CalculateHashCode(subject);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Support_Static_Values()
            {
                var subject = Create<DummyClass>();

                var expected = ObjectExtensions.CalculateHashCode(subject,
                  model => model.Prop1, model => model.GetProp2(), model => model.Prop3, model => model.Prop4,
                  model => model.Prop5, model => model.Prop6, model => DummyClass.Prop7, model => model.Prop8
                );

                int actual;
                var oldBindings = ObjectExtensions.DefaultHashCodeBindings;

                try
                {
                    ObjectExtensions.DefaultHashCodeBindings = BindingOptions.All;
                    actual = ObjectExtensions.CalculateHashCode(subject);
                }
                finally
                {
                    ObjectExtensions.DefaultHashCodeBindings = oldBindings;
                }

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Be_Calculated_Based_On_Ordered_Names()
            {
                var subject = Create<DummyClass>();

                // Prop7 is static - the default binding excludes statics
                var expected1 = ObjectExtensions.CalculateHashCode(subject,
                  model => model.Prop1, model => model.GetProp2(), model => model.Prop3, model => model.Prop4,
                  model => model.Prop5, model => model.Prop6, model => model.Prop8
                );

                var expected2 = ObjectExtensions.CalculateHashCode(subject,
                  model => model.Prop4, model => model.Prop1, model => model.Prop3, model => model.Prop8,
                  model => model.Prop5, model => model.Prop6, model => model.GetProp2()
                );

                var actual = ObjectExtensions.CalculateHashCode(subject);

                expected1.Should().NotBe(expected2);
                actual.Should().Be(expected1);
            }

            [Fact]
            public void Should_Include_Specified_Properties_And_Exclude_None()
            {
                var subject = Create<DummyClass>();

                var expected = ObjectExtensions.CalculateHashCode(subject, model => model.Prop1);

                var actual = ObjectExtensions.CalculateHashCode(subject, new[] { "Prop1" }, null);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Include_And_Exclude_Specified_Properties_With_Overlap()
            {
                var subject = Create<DummyClass>();

                var expected = ObjectExtensions.CalculateHashCode(subject, model => model.Prop1, model => model.Prop5);

                var actual = ObjectExtensions.CalculateHashCode(subject, new[] { "Prop1", "Prop4", "Prop5" }, new[] { "Prop4" });

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Include_Specified_Properties_And_Ignore_Non_Existing_Exclusion()
            {
                var subject = Create<DummyClass>();

                var expected = ObjectExtensions.CalculateHashCode(subject, model => model.Prop1, model => model.Prop4, model => model.Prop5);

                var actual = ObjectExtensions.CalculateHashCode(subject, new[] { "Prop1", "Prop4", "Prop5" }, new[] { "Prop99" });

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Exclude_Specified_Properties()
            {
                var subject = Create<DummyClass>();

                var expected = ObjectExtensions.CalculateHashCode(subject, model => model.GetProp2(), model => model.Prop3,
                  model => model.Prop6, model => model.Prop8);

                var actual = ObjectExtensions.CalculateHashCode(subject, null, new[] { "Prop1", "Prop4", "Prop5" });

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Ignore_Non_Existing_Inclusions()
            {
                var subject = Create<DummyClass>();

                var expected = ObjectExtensions.CalculateHashCode(subject, model => model.Prop1);

                var actual = ObjectExtensions.CalculateHashCode(subject, new[] { "Prop1", "Prop44", "Prop55" }, null);

                actual.Should().Be(expected);
            }
        }

        public class CalculateHashCode_By_Property_Or_Value : ObjectExtensionsFixture
        {
            [Fact]
            public void Should_Calculate_Using_Specified_Properties()
            {
                var subject = Create<DummyClass>();

                var expected = ObjectExtensions.CalculateHashCode(subject, new[] { "Prop1", "Prop5" });

                var actual = ObjectExtensions.CalculateHashCode(subject, model => model.Prop1, model => model.Prop5);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Calculate_Using_Specified_Values()
            {
                var subject = Create<DummyClass>();

                var expected = ObjectExtensions.CalculateHashCode(subject, new[] { "Prop1", "Prop2", "Prop5" });

                var actual = ObjectExtensions.CalculateHashCode(subject, model => model.Prop1, model => model.GetProp2(), model => model.Prop5);

                actual.Should().Be(expected);
            }
        }

        public class IsEnrichedEnum : ObjectExtensionsFixture
        {
            [Fact]
            public void Should_Return_False()
            {
                var dummy = Create<DummyClass>();

                var actual = dummy.IsEnrichedEnum();

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_True()
            {
                var dummy = EnrichedEnumDummy.Value1;

                var actual = dummy.IsEnrichedEnum();

                actual.Should().BeTrue();
            }
        }

        // get property values based on binding options (for cases when we want to include private variables
        private static int GetCumulativeHash(object instance, BindingOptions bindingOptions)
        {
            var properties = instance
              .GetType()
              .GetPropertyInfo(bindingOptions)
              .OrderBy(propertyInfo => propertyInfo.Name)
              .Select(propertyInfo => propertyInfo.GetValue(instance));

            return GetCumulativeHash(properties);
        }

        private static int GetCumulativeHash(IEnumerable<object> properties)
        {
            return properties.Aggregate(17, (current, property) => current * 23 + (property?.GetHashCode() ?? 0));
        }
    }
}