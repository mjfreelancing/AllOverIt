using AllOverIt.EntityFrameworkCore.EnrichedEnum;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Enumeration;
using FluentAssertions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AllOverIt.EntityFrameworkCore.Tests.EnrichedEnum
{
    public class EnrichedEnumModelBuilderOptionsFixture : FixtureBase
    {
        private readonly EnrichedEnumModelBuilderOptions _modelBuilderOptions = new();

        private enum DummyEnum { }

        private sealed class DummyEnrichedEnum1 : EnrichedEnum<DummyEnrichedEnum1>
        {
            public static readonly DummyEnrichedEnum1 Value0 = new(0);
            public static readonly DummyEnrichedEnum1 Value1 = new(1);
            public static readonly DummyEnrichedEnum1 Value2 = new(2);

            private DummyEnrichedEnum1()
            {
            }

            private DummyEnrichedEnum1(int value, [CallerMemberName] string name = "")
                : base(value, name)
            {
            }
        }

        private sealed class DummyEnrichedEnum2 : EnrichedEnum<DummyEnrichedEnum2>
        {
            public static readonly DummyEnrichedEnum2 Value0 = new(0);
            public static readonly DummyEnrichedEnum2 Value1 = new(1);
            public static readonly DummyEnrichedEnum2 Value2 = new(2);

            private DummyEnrichedEnum2()
            {
            }

            private DummyEnrichedEnum2(int value, [CallerMemberName] string name = "")
                : base(value, name)
            {
            }
        }

        private sealed class DummyEntity1
        {
            public int Id { get; set; }
            public DummyEnrichedEnum1 Column1 { get; set; }
            public DummyEnum Column2 { get; set; }
        }

        private sealed class DummyEntity2
        {
            public int Id { get; set; }
            public DummyEnum Column1 { get; set; }
            public DummyEnrichedEnum2 Column2 { get; set; }
        }

        public class Entity_Generic : EnrichedEnumModelBuilderOptionsFixture
        {
            [Fact]
            public void Should_Return_Options_For_Type()
            {
                var actual = _modelBuilderOptions.Entity<DummyEntity1>();

                actual.Should().BeOfType<EnrichedEnumEntityOptions>();
            }
        }

        public class Entity_Type : EnrichedEnumModelBuilderOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_Type_Null()
            {
                Invoking(() =>
                {
                    _ = _modelBuilderOptions.Entity(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("entityType");
            }

            [Fact]
            public void Should_Return_Options_For_Type()
            {
                var actual = _modelBuilderOptions.Entity(typeof(DummyEntity1));

                actual.Should().BeOfType<EnrichedEnumEntityOptions>();
            }
        }

        public class Entities : EnrichedEnumModelBuilderOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_Types_Null()
            {
                Invoking(() =>
                {
                    _ = _modelBuilderOptions.Entities(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("entityTypes");
            }

            [Fact]
            public void Should_Throw_When_Types_Empty()
            {
                Invoking(() =>
                {
                    _ = _modelBuilderOptions.Entities([]);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("entityTypes");
            }

            [Fact]
            public void Should_Return_Options_For_Type()
            {
                var actual = _modelBuilderOptions.Entities(typeof(DummyEntity1), typeof(DummyEntity2));

                actual.Should().BeOfType<EnrichedEnumEntityOptions>();
            }
        }

        public class AsName : EnrichedEnumModelBuilderOptionsFixture
        {
            private readonly EnrichedEnumEntityOptions _entityOptions;
            private readonly Dictionary<string, PropertyInfo> _dummy1Properties;
            private readonly Dictionary<string, PropertyInfo> _dummy2Properties;

            public AsName()
            {
                _entityOptions = _modelBuilderOptions.Entities(typeof(DummyEntity1), typeof(DummyEntity2));

                _dummy1Properties = typeof(DummyEntity1)
                    .GetProperties()
                    .ToDictionary(item => item.Name);

                _dummy2Properties = typeof(DummyEntity2)
                    .GetProperties()
                    .ToDictionary(item => item.Name);
            }

            [Fact]
            public void Should_Configure_EntityOptions_When_No_ColumnType_No_Length()
            {
                _modelBuilderOptions.AsName();

                _entityOptions.PropertyPredicate.Invoke(_dummy1Properties[nameof(DummyEntity1.Id)]).Should().BeFalse();
                _entityOptions.PropertyPredicate.Invoke(_dummy1Properties[nameof(DummyEntity1.Column1)]).Should().BeTrue();     // Is EnrichedEnum
                _entityOptions.PropertyPredicate.Invoke(_dummy1Properties[nameof(DummyEntity1.Column2)]).Should().BeFalse();

                _entityOptions.PropertyPredicate.Invoke(_dummy2Properties[nameof(DummyEntity2.Id)]).Should().BeFalse();
                _entityOptions.PropertyPredicate.Invoke(_dummy2Properties[nameof(DummyEntity2.Column1)]).Should().BeFalse();
                _entityOptions.PropertyPredicate.Invoke(_dummy2Properties[nameof(DummyEntity2.Column2)]).Should().BeTrue();     // Is EnrichedEnum

                _entityOptions.PropertyOptions.TypeConverter.Should().Be(EnrichedEnumModelBuilderTypes.AsNameConverter);

                _entityOptions.PropertyOptions.PropertyBuilder.Should().BeNull();
            }

            [Fact]
            public void Should_Configure_EntityOptions_When_Has_ColumnType_No_Length()
            {
                _modelBuilderOptions.AsName(Create<string>());

                _entityOptions.PropertyPredicate.Invoke(_dummy1Properties[nameof(DummyEntity1.Id)]).Should().BeFalse();
                _entityOptions.PropertyPredicate.Invoke(_dummy1Properties[nameof(DummyEntity1.Column1)]).Should().BeTrue();     // Is EnrichedEnum
                _entityOptions.PropertyPredicate.Invoke(_dummy1Properties[nameof(DummyEntity1.Column2)]).Should().BeFalse();

                _entityOptions.PropertyPredicate.Invoke(_dummy2Properties[nameof(DummyEntity2.Id)]).Should().BeFalse();
                _entityOptions.PropertyPredicate.Invoke(_dummy2Properties[nameof(DummyEntity2.Column1)]).Should().BeFalse();
                _entityOptions.PropertyPredicate.Invoke(_dummy2Properties[nameof(DummyEntity2.Column2)]).Should().BeTrue();     // Is EnrichedEnum

                _entityOptions.PropertyOptions.TypeConverter.Should().Be(EnrichedEnumModelBuilderTypes.AsNameConverter);

                // Not testing PropertyBuilder action - depends on EF internals
                _entityOptions.PropertyOptions.PropertyBuilder.Should().NotBeNull();
            }

            [Fact]
            public void Should_Configure_EntityOptions_When_No_ColumnType_Has_Length()
            {
                _modelBuilderOptions.AsName(null, Create<int>());

                _entityOptions.PropertyPredicate.Invoke(_dummy1Properties[nameof(DummyEntity1.Id)]).Should().BeFalse();
                _entityOptions.PropertyPredicate.Invoke(_dummy1Properties[nameof(DummyEntity1.Column1)]).Should().BeTrue();     // Is EnrichedEnum
                _entityOptions.PropertyPredicate.Invoke(_dummy1Properties[nameof(DummyEntity1.Column2)]).Should().BeFalse();

                _entityOptions.PropertyPredicate.Invoke(_dummy2Properties[nameof(DummyEntity2.Id)]).Should().BeFalse();
                _entityOptions.PropertyPredicate.Invoke(_dummy2Properties[nameof(DummyEntity2.Column1)]).Should().BeFalse();
                _entityOptions.PropertyPredicate.Invoke(_dummy2Properties[nameof(DummyEntity2.Column2)]).Should().BeTrue();     // Is EnrichedEnum

                _entityOptions.PropertyOptions.TypeConverter.Should().Be(EnrichedEnumModelBuilderTypes.AsNameConverter);

                // Not testing PropertyBuilder action - depends on EF internals
                _entityOptions.PropertyOptions.PropertyBuilder.Should().NotBeNull();
            }
        }

        public class AsValue : EnrichedEnumModelBuilderOptionsFixture
        {
            private readonly EnrichedEnumEntityOptions _entityOptions;
            private readonly Dictionary<string, PropertyInfo> _dummy1Properties;
            private readonly Dictionary<string, PropertyInfo> _dummy2Properties;

            public AsValue()
            {
                _entityOptions = _modelBuilderOptions.Entities(typeof(DummyEntity1), typeof(DummyEntity2));

                _dummy1Properties = typeof(DummyEntity1)
                    .GetProperties()
                    .ToDictionary(item => item.Name);

                _dummy2Properties = typeof(DummyEntity2)
                    .GetProperties()
                    .ToDictionary(item => item.Name);
            }

            [Fact]
            public void Should_Configure_EntityOptions_When_No_ColumnType()
            {
                _modelBuilderOptions.AsValue();

                _entityOptions.PropertyPredicate.Invoke(_dummy1Properties[nameof(DummyEntity1.Id)]).Should().BeFalse();
                _entityOptions.PropertyPredicate.Invoke(_dummy1Properties[nameof(DummyEntity1.Column1)]).Should().BeTrue();     // Is EnrichedEnum
                _entityOptions.PropertyPredicate.Invoke(_dummy1Properties[nameof(DummyEntity1.Column2)]).Should().BeFalse();

                _entityOptions.PropertyPredicate.Invoke(_dummy2Properties[nameof(DummyEntity2.Id)]).Should().BeFalse();
                _entityOptions.PropertyPredicate.Invoke(_dummy2Properties[nameof(DummyEntity2.Column1)]).Should().BeFalse();
                _entityOptions.PropertyPredicate.Invoke(_dummy2Properties[nameof(DummyEntity2.Column2)]).Should().BeTrue();     // Is EnrichedEnum

                _entityOptions.PropertyOptions.TypeConverter.Should().Be(EnrichedEnumModelBuilderTypes.AsValueConverter);

                _entityOptions.PropertyOptions.PropertyBuilder.Should().BeNull();
            }

            [Fact]
            public void Should_Configure_EntityOptions_When_Has_ColumnType()
            {
                _modelBuilderOptions.AsValue(Create<string>());

                _entityOptions.PropertyPredicate.Invoke(_dummy1Properties[nameof(DummyEntity1.Id)]).Should().BeFalse();
                _entityOptions.PropertyPredicate.Invoke(_dummy1Properties[nameof(DummyEntity1.Column1)]).Should().BeTrue();     // Is EnrichedEnum
                _entityOptions.PropertyPredicate.Invoke(_dummy1Properties[nameof(DummyEntity1.Column2)]).Should().BeFalse();

                _entityOptions.PropertyPredicate.Invoke(_dummy2Properties[nameof(DummyEntity2.Id)]).Should().BeFalse();
                _entityOptions.PropertyPredicate.Invoke(_dummy2Properties[nameof(DummyEntity2.Column1)]).Should().BeFalse();
                _entityOptions.PropertyPredicate.Invoke(_dummy2Properties[nameof(DummyEntity2.Column2)]).Should().BeTrue();     // Is EnrichedEnum

                _entityOptions.PropertyOptions.TypeConverter.Should().Be(EnrichedEnumModelBuilderTypes.AsValueConverter);

                // Not testing PropertyBuilder action - depends on EF internals
                _entityOptions.PropertyOptions.PropertyBuilder.Should().NotBeNull();
            }
        }
    }
}