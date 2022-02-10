using AllOverIt.Exceptions;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Formatters.Objects;
using AllOverIt.Reflection;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Formatters.Objects
{
    public class ObjectPropertySerializerFixture : FixtureBase
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

        private class Typed<TType>
        {
            public TType Prop { get; set; }
        }

        private sealed class DummyWithIndexer
        {
            public string this[int key] => string.Empty;
            public int That { get; set; }
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

        private class DummyTypePropertyValueFilter : ObjectPropertyFilter, IFormattableObjectPropertyFilter
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

        private class DummyNestedChildInfo
        {
            public IEnumerable<int> TopNumbers { get; set; }
        }

        private class DummyNestedChild
        {
            public IEnumerable<DummyNestedChildInfo> Info { get; set; }
        }

        private class DummyNestedParent
        {
            public IEnumerable<DummyNestedChild> Children { get; set; }
        }

        private class DummyNestedParentFilter : ObjectPropertyFilter
        {
            public override bool OnIncludeProperty()
            {
                EnumerableOptions.CollateValues = Parents.Any() && Parents.Count >= 3;

                return true;
            }
        }

        protected ObjectPropertySerializerFixture()
        {
            // prevent self-references
            Fixture.Customizations.Add(new PropertyNameOmitter("Prop2", "Prop6", "Prop7", "Prop12"));
        }

        public class Defaults : ObjectPropertySerializerFixture
        {
            [Fact]
            public void Should_Have_Known_Ignored_Types()
            {
                object[] expected =
                {
                    typeof(Task),
                    typeof(Task<>),
                };

                var helper = new ObjectPropertySerializer();

                helper.Options.IgnoredTypes
                    .Should()
                    .BeEquivalentTo(expected);
            }
        }

        public class Constructor : ObjectPropertySerializerFixture
        {
            [Fact]
            public void Should_Have_Default_IncludeNulls()
            {
                var helper = new ObjectPropertySerializer();

                helper.Options
                    .Should()
                    .BeEquivalentTo(new
                    {
                        IncludeNulls = false,
                        IncludeEmptyCollections = false,
                        NullValueOutput = "<null>",
                        EmptyValueOutput = "<empty>"
                    });
            }

            [Theory]
            [InlineData(BindingOptions.Default)]
            [InlineData(BindingOptions.All)]
            [InlineData(BindingOptions.Instance | BindingOptions.NonVirtual | BindingOptions.Public)]
            [InlineData(BindingOptions.Protected | BindingOptions.Abstract | BindingOptions.Private)]
            public void Should_Have_Custom_BindingOptions(BindingOptions bindingOptions)
            {
                var options = new ObjectPropertySerializerOptions
                {
                    BindingOptions = bindingOptions
                };

                var helper = new ObjectPropertySerializer(options);

                helper.Options
                    .Should()
                    .BeEquivalentTo(new
                    {
                        IncludeNulls = false,
                        IncludeEmptyCollections = false,
                        NullValueOutput = "<null>",
                        EmptyValueOutput = "<empty>",
                        BindingOptions = bindingOptions
                    });
            }
        }

        public class SerializeToDictionary : ObjectPropertySerializerFixture
        {
            [Fact]
            public void Should_Serialize_Type_Using_Default_Settings()
            {
                var dummy = Create<DummyType>();

                var serializer = GetSerializer();
                var actual = serializer.SerializeToDictionary(dummy);

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

                var serializer = GetSerializer();
                serializer.Options.BindingOptions = BindingOptions.Private | BindingOptions.Instance;

                var actual = serializer.SerializeToDictionary(dummy);

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

                var serializer = GetSerializer();

                Invoking(() =>
                    {
                        _ = serializer.SerializeToDictionary(dummy1);
                    })
                    .Should()
                    .Throw<SelfReferenceException>()
                    .WithMessage("Self referencing detected at 'Prop2.Prop2.Prop2' of type 'DummyType'");
            }

            [Fact]
            public void Should_Not_Collate_Arrays()
            {
                var dummy = new DummyType
                {
                    Prop4 = CreateMany<string>(3)
                };

                var serializer = GetSerializer();

                serializer.Options.IncludeNulls = false;
                serializer.Options.IncludeEmptyCollections = false;

                //serializer.Options.EnumerableOptions.CollateValues = true;

                var actual = serializer.SerializeToDictionary(dummy);

                actual
                    .Should()
                    .BeEquivalentTo(new Dictionary<string, string>
                    {
                        { "Prop1", $"{dummy.Prop1}" },
                        { "Prop4[0]", $"{dummy.Prop4.ElementAt(0)}" },
                        { "Prop4[1]", $"{dummy.Prop4.ElementAt(1)}" },
                        { "Prop4[2]", $"{dummy.Prop4.ElementAt(2)}" }
                    });
            }

            [Fact]
            public void Should_Collate_Arrays()
            {
                var dummy = new DummyType
                {
                    Prop4 = CreateMany<string>(3)
                };

                var serializer = GetSerializer();

                serializer.Options.IncludeNulls = false;
                serializer.Options.IncludeEmptyCollections = false;
                serializer.Options.EnumerableOptions.CollateValues = true;

                var actual = serializer.SerializeToDictionary(dummy);

                actual
                    .Should()
                    .BeEquivalentTo(new Dictionary<string, string>
                    {
                        { "Prop1", $"{dummy.Prop1}" },
                        { "Prop4", $"{dummy.Prop4.ElementAt(0)}, {dummy.Prop4.ElementAt(1)}, {dummy.Prop4.ElementAt(2)}" }
                    });
            }

            [Fact]
            public void Should_Not_Collate_Arrays_When_Filtered()
            {
                var dummy = Create<DummyType>();
                dummy.Prop2 = Create<DummyType>();

                var serializer = GetSerializer();

                // Will be ignored because the filter's options is not set to collate enumerables
                serializer.Options.EnumerableOptions.CollateValues = true;

                // Prop2 is a class type so to include all values we need to check for "Prop2" as well as "Prop2.XXX"
                // Checking for Prop2 is required to ensure the sub-properties are not filtered out.
                serializer.Options.Filter = new DummyTypePropertyNameFilter(name =>
                    name is nameof(DummyType.Prop1) or nameof(DummyType.Prop2) ||
                    name.StartsWith("Prop2.Prop4"));

                var actual = serializer.SerializeToDictionary(dummy);

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
            public void Should_Collate_Arrays_Using_Filter_Options()
            {
                var dummy = Create<DummyType>();
                dummy.Prop2 = Create<DummyType>();

                var serializer = GetSerializer();


                // Prop2 is a class type so to include all values we need to check for "Prop2" as well as "Prop2.XXX"
                // Checking for Prop2 is required to ensure the sub-properties are not filtered out.
                serializer.Options.Filter = new DummyTypePropertyNameFilter(name =>
                    name is nameof(DummyType.Prop1) or nameof(DummyType.Prop2) ||
                    name.StartsWith("Prop2.Prop4"));

                serializer.Options.Filter.EnumerableOptions.CollateValues = true;

                var actual = serializer.SerializeToDictionary(dummy);

                actual
                    .Should()
                    .BeEquivalentTo(new Dictionary<string, string>
                    {
                        { "Prop1", $"{dummy.Prop1}" },
                        { "Prop2.Prop4", $"{dummy.Prop2.Prop4.ElementAt(0)}, {dummy.Prop2.Prop4.ElementAt(1)}, {dummy.Prop2.Prop4.ElementAt(2)}" }
                    });
            }

            [Fact]
            public void Should_Collate_Child_Nodes_Using_Filter()
            {
                var dummy = Create<DummyNestedParent>();

                var serializer = GetSerializer();

                serializer.Options.Filter = new DummyNestedParentFilter();

                var actual = serializer.SerializeToDictionary(dummy);

                actual
                    .Should()
                    .BeEquivalentTo(new Dictionary<string, string>
                    {
                        {"Children[0].Info[0].TopNumbers", string.Join(", ", dummy.Children.ElementAt(0).Info.ElementAt(0).TopNumbers)},
                        {"Children[0].Info[1].TopNumbers", string.Join(", ", dummy.Children.ElementAt(0).Info.ElementAt(1).TopNumbers)},
                        {"Children[0].Info[2].TopNumbers", string.Join(", ", dummy.Children.ElementAt(0).Info.ElementAt(2).TopNumbers)},
                        {"Children[1].Info[0].TopNumbers", string.Join(", ", dummy.Children.ElementAt(1).Info.ElementAt(0).TopNumbers)},
                        {"Children[1].Info[1].TopNumbers", string.Join(", ", dummy.Children.ElementAt(1).Info.ElementAt(1).TopNumbers)},
                        {"Children[1].Info[2].TopNumbers", string.Join(", ", dummy.Children.ElementAt(1).Info.ElementAt(2).TopNumbers)},
                        {"Children[2].Info[0].TopNumbers", string.Join(", ", dummy.Children.ElementAt(2).Info.ElementAt(0).TopNumbers)},
                        {"Children[2].Info[1].TopNumbers", string.Join(", ", dummy.Children.ElementAt(2).Info.ElementAt(1).TopNumbers)},
                        {"Children[2].Info[2].TopNumbers", string.Join(", ", dummy.Children.ElementAt(2).Info.ElementAt(2).TopNumbers)}
                    });
            }

            [Fact]
            public void Should_Serialize_Nested_Types()
            {
                var dummy1 = Create<DummyType>();
                var dummy2 = Create<DummyType>();
                var dummy3 = Create<DummyType>();

                dummy1.Prop2 = dummy2;
                dummy2.Prop2 = dummy3;

                var serializer = GetSerializer();
                var actual = serializer.SerializeToDictionary(dummy1);

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

                var serializer = GetSerializer();
                serializer.Options.IncludeNulls = true;

                var actual = serializer.SerializeToDictionary(dummy);

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
                    Prop12 = new Dictionary<int, DummyType>(),
                    Prop14 = string.Empty
                };

                var serializer = GetSerializer();
                serializer.Options.IncludeEmptyCollections = true;

                var actual = serializer.SerializeToDictionary(dummy);

                actual
                    .Should()
                    .BeEquivalentTo(new Dictionary<string, string>
                    {
                        { "Prop1", "0" },
                        { "Prop4", "<empty>" },
                        { "Prop5", "<empty>" },
                        { "Prop6", "<empty>" },
                        { "Prop7", "<empty>" },
                        { "Prop12", "<empty>" },
                        { "Prop14", "<empty>" }
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

                var serializer = GetSerializer();
                serializer.Options.IncludeNulls = true;
                serializer.Options.IncludeEmptyCollections = true;

                var actual = serializer.SerializeToDictionary(dummy);

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

                var serializer = GetSerializer();
                var actual = serializer.SerializeToDictionary(dictionary);

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

                var serializer = GetSerializer();
                var actual = serializer.SerializeToDictionary(dictionary);

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

                var serializer = GetSerializer();
                var actual = serializer.SerializeToDictionary(list);

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

                var serializer = GetSerializer();
                var actual = serializer.SerializeToDictionary(dummy);

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

                var serializer = GetSerializer();
                var actual = serializer.SerializeToDictionary(dummy);

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

                var serializer = GetSerializer();
                var actual = serializer.SerializeToDictionary(dummy);

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

                var serializer = GetSerializer();
                var actual = serializer.SerializeToDictionary(dummy);

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

                var serializer = GetSerializer();
                var actual = serializer.SerializeToDictionary(table);

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

                var serializer = GetSerializer();

                Invoking(() =>
                    {
                        _ = serializer.SerializeToDictionary(root);
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Filter_To_Two_Properties_By_Name()
            {
                var dummy = Create<DummyType>();
                dummy.Prop2 = Create<DummyType>();

                var serializer = GetSerializer();

                // Prop2 is a class type so to include all values we need to check for "Prop2" as well as "Prop2.XXX"
                // Checking for Prop2 is required to ensure the sub-properties are not filtered out.
                serializer.Options.Filter = new DummyTypePropertyNameFilter(name =>
                    name is nameof(DummyType.Prop1) or nameof(DummyType.Prop2) ||
                    name.StartsWith("Prop2.Prop4"));

                var actual = serializer.SerializeToDictionary(dummy);

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

                var serializer = GetSerializer();
                serializer.Options.Filter = new DummyTypePropertyNameFilter(name => name != nameof(DummyType.Prop2));

                var actual = serializer.SerializeToDictionary(dummy1);

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

                var serializer = GetSerializer();
                serializer.Options.Filter = new DummyTypePropertyNameFilter(name => name != nameToFilter);

                var actual = serializer.SerializeToDictionary(dummy1);

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

                var serializer = GetSerializer();
                serializer.Options.Filter = new DummyTypePropertyValueFilter();

                var actual = serializer.SerializeToDictionary(dummy);

                actual
                    .Should()
                    .BeEquivalentTo(new Dictionary<string, string>
                    {
                        { "Prop1", "Included" }
                    });
            }

            [Fact]
            public void Should_Change_One_Property_Value()
            {
                var dummy = Create<DummyType>();
                var dummy2 = Create<DummyType>();
                dummy.Prop2 = dummy2;

                var serializer = GetSerializer();
                serializer.Options.Filter = new DummyTypePropertyNameValueFilter();

                var actual = serializer.SerializeToDictionary(dummy);

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

            public class SerializeObjectPropertyFilter : SerializeToDictionary
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

                    var serializer = GetSerializer();

                    serializer.Options.IncludeEmptyCollections = true;
                    serializer.Options.Filter = filter;

                    _ = serializer.SerializeToDictionary(dummy);

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

                    var serializer = GetSerializer();

                    serializer.Options.IncludeEmptyCollections = true;
                    serializer.Options.Filter = filter;

                    _ = serializer.SerializeToDictionary(dummy);

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

                    var serializer = GetSerializer();

                    serializer.Options.IncludeEmptyCollections = true;
                    serializer.Options.Filter = filter;

                    _ = serializer.SerializeToDictionary(dummy);

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

                    var serializer = GetSerializer();

                    serializer.Options.IncludeEmptyCollections = true;
                    serializer.Options.Filter = filter;

                    _ = serializer.SerializeToDictionary(dummy);

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

            private ObjectPropertySerializer GetSerializer()
            {
                return new();
            }
        }
    }
}