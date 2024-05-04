using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Pagination.Extensions;
using AllOverIt.Pagination.TokenEncoding;
using FakeItEasy;
using FluentAssertions;

namespace AllOverIt.Pagination.Tests.Extensions
{
    public class ContinuationTokenEncoderExtensionsExtensions : FixtureBase
    {
        private readonly IContinuationTokenEncoder _continuationTokenEncoder;
        private readonly IContinuationTokenSerializer _continuationTokenSerializerFake;

        public ContinuationTokenEncoderExtensionsExtensions()
        {
            _continuationTokenSerializerFake = this.CreateStub<IContinuationTokenSerializer>();
            _continuationTokenEncoder = new ContinuationTokenEncoder(this.CreateManyStubs<IColumnDefinition>(), Create<PaginationDirection>(), _continuationTokenSerializerFake);
        }

        public class Encode : ContinuationTokenEncoderExtensionsExtensions
        {
            [Fact]
            public void Should_Throw_When_Encoder_Null()
            {
                Invoking(() =>
                {
                    ContinuationTokenEncoderExtensions.Encode(null, this.CreateStub<IContinuationToken>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("encoder");
            }

            [Fact]
            public void Should_Throw_When_Token_Null()
            {
                Invoking(() =>
                {
                    ContinuationTokenEncoderExtensions.Encode(_continuationTokenEncoder, null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("continuationToken");
            }

            [Fact]
            public void Should_Encode_Token()
            {
                var token = this.CreateStub<IContinuationToken>();
                var expected = Create<string>();

                A.CallTo(() => _continuationTokenSerializerFake.Serialize(token)).Returns(expected);

                var actual = ContinuationTokenEncoderExtensions.Encode(_continuationTokenEncoder, token);

                expected.Should().Be(actual);
            }
        }

        public class Decode : ContinuationTokenEncoderExtensionsExtensions
        {
            [Fact]
            public void Should_Throw_When_Encoder_Null()
            {
                Invoking(() =>
                {
                    ContinuationTokenEncoderExtensions.Decode(null, Create<string>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("encoder");
            }

            [Fact]
            public void Should_Not_Throw_When_Token_Null()
            {
                Invoking(() =>
                {
                    ContinuationTokenEncoderExtensions.Decode(_continuationTokenEncoder, null);
                })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Decode_Token()
            {
                var token = Create<string>();
                var expected = this.CreateStub<IContinuationToken>();

                A.CallTo(() => _continuationTokenSerializerFake.Deserialize(token)).Returns(expected);

                var actual = ContinuationTokenEncoderExtensions.Decode(_continuationTokenEncoder, token);

                expected.Should().Be(actual);
            }
        }
    }
}