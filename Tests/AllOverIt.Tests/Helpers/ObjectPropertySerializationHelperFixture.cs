using AllOverIt.Exceptions;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Helpers;
using AllOverIt.Reflection;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Helpers
{
    public class ObjectPropertySerializationHelperFixture : FixtureBase
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

        protected ObjectPropertySerializationHelperFixture()
        {
            // prevent self-references
            Fixture.Customizations.Add(new PropertyNameOmitter("Prop2", "Prop6", "Prop7", "Prop12"));
        }

        public class Defaults : ObjectPropertySerializationHelperFixture
        {
            [Fact]
            public void Should_Have_Known_Ignored_Types()
            {
                object[] expected =
                {
                    typeof(Task),
                    typeof(Task<>),
                };

                var helper = new ObjectPropertySerializationHelper();

                helper.IgnoredTypes
                    .Should()
                    .BeEquivalentTo(expected);
            }
        }

        public class Constructor : ObjectPropertySerializationHelperFixture
        {
            [Fact]
            public void Should_Have_Default_IncludeNulls()
            {
                var helper = new ObjectPropertySerializationHelper();

                helper
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
                var helper = new ObjectPropertySerializationHelper(bindingOptions);

                helper
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

        public class SerializeToDictionary : ObjectPropertySerializationHelperFixture
        {
            private readonly ObjectPropertySerializationHelper _helper;

            public SerializeToDictionary()
            {
                _helper = new();
            }

            [Fact]
            public void Should_Serialize_Type_Using_Default_Settings()
            {
                var dummy = Create<DummyType>();

                var actual = _helper.SerializeToDictionary(dummy);

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
                        { "Prop8", $"{dummy.Prop8}" }
                    });
            }

            [Fact]
            public void Should_Serialize_Type_Using_Custom_Binding()
            {
                var dummy = Create<DummyType>();

                _helper.BindingOptions = BindingOptions.Private | BindingOptions.Instance;

                var actual = _helper.SerializeToDictionary(dummy);

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
                        _ = _helper.SerializeToDictionary(dummy1);
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

                var actual = _helper.SerializeToDictionary(dummy1);

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

                        { "Prop2.Prop1", $"{dummy2.Prop1}" },
                        { "Prop2.Prop4[0]", $"{dummy2.Prop4.ElementAt(0)}" },
                        { "Prop2.Prop4[1]", $"{dummy2.Prop4.ElementAt(1)}" },
                        { "Prop2.Prop4[2]", $"{dummy2.Prop4.ElementAt(2)}" },
                        { $"Prop2.Prop5.{dummy2.Prop5.ElementAt(0).Key}", $"{dummy2.Prop5.ElementAt(0).Value}" },
                        { $"Prop2.Prop5.{dummy2.Prop5.ElementAt(1).Key}", $"{dummy2.Prop5.ElementAt(1).Value}" },
                        { $"Prop2.Prop5.{dummy2.Prop5.ElementAt(2).Key}", $"{dummy2.Prop5.ElementAt(2).Value}" },
                        { "Prop2.Prop8", $"{dummy2.Prop8}" },

                        { "Prop2.Prop2.Prop1", $"{dummy3.Prop1}" },
                        { "Prop2.Prop2.Prop4[0]", $"{dummy3.Prop4.ElementAt(0)}" },
                        { "Prop2.Prop2.Prop4[1]", $"{dummy3.Prop4.ElementAt(1)}" },
                        { "Prop2.Prop2.Prop4[2]", $"{dummy3.Prop4.ElementAt(2)}" },
                        { $"Prop2.Prop2.Prop5.{dummy3.Prop5.ElementAt(0).Key}", $"{dummy3.Prop5.ElementAt(0).Value}" },
                        { $"Prop2.Prop2.Prop5.{dummy3.Prop5.ElementAt(1).Key}", $"{dummy3.Prop5.ElementAt(1).Value}" },
                        { $"Prop2.Prop2.Prop5.{dummy3.Prop5.ElementAt(2).Key}", $"{dummy3.Prop5.ElementAt(2).Value}" },
                        { "Prop2.Prop2.Prop8", $"{dummy3.Prop8}" }
                    });
            }

            [Fact]
            public void Should_Serialize_Null_Values()
            {
                var dummy = new DummyType();

                _helper.IncludeNulls = true;
                var actual = _helper.SerializeToDictionary(dummy);

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
                        { "Prop12", "<null>" }
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

                _helper.IncludeEmptyCollections = true;

                var actual = _helper.SerializeToDictionary(dummy);

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

                _helper.IncludeNulls = true;
                _helper.IncludeEmptyCollections = true;

                var actual = _helper.SerializeToDictionary(dummy);

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
                        { "Prop12", "<empty>" }
                    });
            }

            [Fact]
            public void Should_Serialize_Dictionary()
            {
                var dictionary = Create<Dictionary<string, int>>();
                var keys = dictionary.Keys.Select(item => item).ToList();
                var values = dictionary.Values.Select(item => $"{item}").ToList();

                var actual = _helper.SerializeToDictionary(dictionary);

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

                var actual = _helper.SerializeToDictionary(dictionary);

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

                var actual = _helper.SerializeToDictionary(list);

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

                var actual = _helper.SerializeToDictionary(dummy);

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

                var actual = _helper.SerializeToDictionary(dummy);

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

                var actual = _helper.SerializeToDictionary(dummy);

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

                var actual = _helper.SerializeToDictionary(dummy);

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

                var actual = _helper.SerializeToDictionary(table);

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
                        _ = _helper.SerializeToDictionary(root);
                    })
                    .Should()
                    .NotThrow();
            }
        }

        public class ClearIgnoredTypes : ObjectPropertySerializationHelperFixture
        {
            [Fact]
            public void Should_Clear_Ignored_Types()
            {
                var helper = new ObjectPropertySerializationHelper();

                helper.IgnoredTypes
                    .Should()
                    .NotBeEmpty();

                helper.ClearIgnoredTypes();

                helper.IgnoredTypes
                    .Should()
                    .BeEmpty();
            }
        }

        public class AddIgnoredTypes : ObjectPropertySerializationHelperFixture
        {
            [Fact]
            public void Should_Add_Ignored_Type_After_Clearing()
            {
                var helper = new ObjectPropertySerializationHelper();

                helper.ClearIgnoredTypes();

                helper.AddIgnoredTypes(typeof(DummyType));

                helper.IgnoredTypes
                    .Should()
                    .BeEquivalentTo(new[]{ typeof(DummyType) });
            }

            [Fact]
            public void Should_Add_Ignored_Type()
            {
                var helper = new ObjectPropertySerializationHelper();

                helper.AddIgnoredTypes(typeof(DummyType));

                helper.IgnoredTypes
                    .Should()
                    .BeEquivalentTo(new[]{ typeof(Task), typeof(Task<>), typeof(DummyType) });
            }
        }
    }
}