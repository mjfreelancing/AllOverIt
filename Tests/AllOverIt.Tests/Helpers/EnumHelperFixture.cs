using AllOverIt.Fixture;
using AllOverIt.Helpers;
using FluentAssertions;

namespace AllOverIt.Tests.Helpers
{
    public class EnumHelperFixture : FixtureBase
    {
        [Flags]
        public enum DummyEnum
        {
            Value1 = 1,
            Value2 = 2,
            Value3 = 4
        }

        public class GetEnumValues : EnumHelperFixture
        {
            [Fact]
            public void Should_Get_Enum_Values()
            {
                var actual = EnumHelper.GetEnumValues<DummyEnum>();

                var expected = new[] { DummyEnum.Value1, DummyEnum.Value2, DummyEnum.Value3 };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0150:Prefer 'null' check over type check", Justification = "Part of the test and readability")]
            public void Should_Return_As_IReadOnlyCollection()
            {
                var actual = EnumHelper.GetEnumValues<DummyEnum>();

                var isReadOnlyCollection = actual is IReadOnlyCollection<DummyEnum>;

                // IsOfType<> treats it as DummyEnum[]
                isReadOnlyCollection.Should().BeTrue();
            }
        }

#if !NETSTANDARD2_1
        public class GetValuesFromBitMask : EnumHelperFixture
        {
            [Theory]
            [InlineData(0, new DummyEnum[] { })]
            [InlineData(1, new DummyEnum[] { DummyEnum.Value1 })]
            [InlineData(2, new DummyEnum[] { DummyEnum.Value2 })]
            [InlineData(3, new DummyEnum[] { DummyEnum.Value1, DummyEnum.Value2 })]
            [InlineData(4, new DummyEnum[] { DummyEnum.Value3 })]
            [InlineData(5, new DummyEnum[] { DummyEnum.Value1, DummyEnum.Value3 })]
            [InlineData(6, new DummyEnum[] { DummyEnum.Value2, DummyEnum.Value3 })]
            [InlineData(7, new DummyEnum[] { DummyEnum.Value1, DummyEnum.Value2, DummyEnum.Value3 })]
            public void Should_Get_Expected_Values(int mask, DummyEnum[] expected)
            {
                var actual = EnumHelper.GetValuesFromBitMask<DummyEnum, int>(mask);

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Throw_When_Out_Of_Range()
            {
                var mask = GetWithinRange(10, 50);

                Invoking(() =>
                {
                    _ = EnumHelper.GetValuesFromBitMask<DummyEnum, int>(mask);
                })
                .Should()
                .Throw<ArgumentOutOfRangeException>()
                .WithMessage($"The value has a flag that does not correspond with the '{typeof(DummyEnum).Name}' type. (Parameter 'mask')\r\nActual value was {mask}.");
            }
        }

        public class GetValuesFromEnumWithFlags : EnumHelperFixture
        {
            [Theory]
            [InlineData(DummyEnum.Value1, new DummyEnum[] { DummyEnum.Value1 })]
            [InlineData(DummyEnum.Value2, new DummyEnum[] { DummyEnum.Value2 })]
            [InlineData(DummyEnum.Value1 | DummyEnum.Value2, new DummyEnum[] { DummyEnum.Value1, DummyEnum.Value2 })]
            [InlineData(DummyEnum.Value3, new DummyEnum[] { DummyEnum.Value3 })]
            [InlineData(DummyEnum.Value1 | DummyEnum.Value3, new DummyEnum[] { DummyEnum.Value1, DummyEnum.Value3 })]
            [InlineData(DummyEnum.Value2 | DummyEnum.Value3, new DummyEnum[] { DummyEnum.Value2, DummyEnum.Value3 })]
            [InlineData(DummyEnum.Value1 | DummyEnum.Value2 | DummyEnum.Value3, new DummyEnum[] { DummyEnum.Value1, DummyEnum.Value2, DummyEnum.Value3 })]
            public void Should_Get_Expected_Values(DummyEnum mask, DummyEnum[] expected)
            {
                var actual = EnumHelper.GetValuesFromEnumWithFlags(mask);

                expected.Should().BeEquivalentTo(actual);
            }
        }
#endif
    }
}
