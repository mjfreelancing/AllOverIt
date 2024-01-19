using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Pagination.TokenEncoding;
using FakeItEasy;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Pagination.Tests.TokenEncoding
{
    public class ContinuationTokenSerializerFactoryFixture : FixtureBase
    {
        public class CreateContinuationTokenSerializer : ContinuationTokenSerializerFactoryFixture
        {
            private readonly ContinuationTokenSerializerFactory _serializerFactory = new();

            [Fact]
            public void Should_Throw_When_Options_Null()
            {
                Invoking(() =>
                {
                    _ = _serializerFactory.CreateContinuationTokenSerializer(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("continuationTokenOptions");
            }

            [Fact]
            public void Should_Return_Serializer()
            {
                var options = this.CreateStub<IContinuationTokenOptions>();

                var actual = _serializerFactory.CreateContinuationTokenSerializer(options);

                actual.Should().BeOfType<ContinuationTokenSerializer>();
            }
        }
    }
}