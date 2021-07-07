﻿using AllOverIt.Exceptions;
using AllOverIt.Fixture;
using AllOverIt.Helpers;
using AllOverIt.Reflection;
using AutoFixture.Kernel;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Helpers
{
    public class ObjectPropertySerializationHelperFixture : FixtureBase
    {
        private class PropertyNameOmmitter : ISpecimenBuilder
        {
            private readonly IEnumerable<string> names;

            internal PropertyNameOmmitter(params string[] names)
            {
                this.names = names;
            }

            public object Create(object request, ISpecimenContext context)
            {
                var propInfo = request as PropertyInfo;
                if (propInfo != null && names.Contains(propInfo.Name))
                {
                    return new OmitSpecimen();
                }

                return new NoSpecimen();
            }
        }

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
            private string Prop13 { get; set; }

            public DummyType()
            {
                Prop13 = "13";
            }
        }

        protected ObjectPropertySerializationHelperFixture()
        {
            // prevent self-references
            Fixture.Customizations.Add(new PropertyNameOmmitter("Prop2", "Prop6", "Prop7", "Prop12"));
        }

        public class Defaults : ObjectPropertySerializationHelperFixture
        {
            [Fact]
            public void Should_Have_Default_Bindings()
            {
                var expected = BindingOptions.DefaultScope | BindingOptions.Virtual | BindingOptions.NonVirtual | BindingOptions.Public;

                ObjectPropertySerializationHelper.DefaultBindingOptions
                    .Should()
                    .BeEquivalentTo(expected);
            }

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
        }


        //private class DummyType
        //{
        //    public int Prop1 { get; set; }
        //    public DummyType Prop2 { get; set; }
        //    public Task Prop3 { get; set; }
        //    public IEnumerable<string> Prop4 { get; set; }
        //    public IDictionary<int, bool> Prop5 { get; set; }
        //    public IDictionary<DummyType, string> Prop6 { get; set; }
        //    public IDictionary<string, DummyType> Prop7 { get; set; }
        //    public double? Prop8 { get; set; }
        //    public Action<int, string> Prop9 { get; set; }
        //    public Func<bool> Prop10 { get; set; }
        //    public IDictionary<string, Task> Prop11 { get; set; }
        //    public IDictionary<int, DummyType> Prop12 { get; set; }
        //}

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
                    .BeEquivalentTo(typeof(DummyType));
            }

            [Fact]
            public void Should_Add_Ignored_Type()
            {
                var helper = new ObjectPropertySerializationHelper();

                helper.AddIgnoredTypes(typeof(DummyType));

                helper.IgnoredTypes
                    .Should()
                    .BeEquivalentTo(typeof(Task), typeof(Task<>), typeof(DummyType));
            }
        }
    }
}