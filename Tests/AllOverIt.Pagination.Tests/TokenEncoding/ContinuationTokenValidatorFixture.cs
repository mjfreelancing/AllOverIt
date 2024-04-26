using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Pagination.TokenEncoding;
using FluentAssertions;
using System;

namespace AllOverIt.Pagination.Tests.TokenEncoding
{
    public class ContinuationTokenValidatorFixture : FixtureBase
    {
        public class Constructor : ContinuationTokenValidatorFixture
        {
            [Fact]
            public void Should_Throw_When_Serializer_Factoiry_Null()
            {
                Invoking(() =>
                {
                    _ = new ContinuationTokenValidator(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("serializerFactory");
            }
        }

        public class IsValidToken : ContinuationTokenValidatorFixture
        {
            [Fact]
            public void Should_Throw_When_Options_Null()
            {
                Invoking(() =>
                {
                    var validator = new ContinuationTokenValidator(this.CreateStub<IContinuationTokenSerializerFactory>());
                    validator.IsValidToken(Create<string>(), null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("tokenOptions");
            }

            [Fact]
            public void Should_Return_False_When_Random_String()
            {
                var validator = new ContinuationTokenValidator(this.CreateStub<IContinuationTokenSerializerFactory>());
                var actual = validator.IsValidToken(Create<string>(), this.CreateStub<IContinuationTokenOptions>());

                actual.Should().BeFalse();
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(true, false)]
            [InlineData(false, true)]
            [InlineData(false, false)]
            public void Should_Return_True_When_Valid_Token(bool includeHash, bool useCompression)
            {
                var continuationToken = new ContinuationToken
                {
                    Direction = Create<PaginationDirection>(),
                    Values = new object[]
                    {
                        Create<bool>(),
                        Create<int>()
                    }
                };

                var options = new ContinuationTokenOptions
                {
                    IncludeHash = includeHash,
                    UseCompression = useCompression
                };

                var serializer = new ContinuationTokenSerializer(options);

                var token = serializer.Serialize(continuationToken);

                var serializerFactory = new ContinuationTokenSerializerFactory();
                var validator = new ContinuationTokenValidator(serializerFactory);

                var actual = validator.IsValidToken(token, options);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_False_When_Invalid_Token()
            {
                var continuationToken = new ContinuationToken
                {
                    Direction = Create<PaginationDirection>(),
                    Values = new object[]
                    {
                        Create<bool>(),
                        Create<int>()
                    }
                };

                var options = Create<ContinuationTokenOptions>();

                var serializer = new ContinuationTokenSerializer(options);

                var token = serializer.Serialize(continuationToken);

                var serializerFactory = new ContinuationTokenSerializerFactory();
                var validator = new ContinuationTokenValidator(serializerFactory);

                var actual = validator.IsValidToken($"12{token}34", options);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_True_When_Token_Null()
            {
                var serializerFactory = new ContinuationTokenSerializerFactory();
                var validator = new ContinuationTokenValidator(serializerFactory);

                var actual = validator.IsValidToken(null, this.CreateStub<IContinuationTokenOptions>());

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_True_When_Token_Empty()
            {
                var serializerFactory = new ContinuationTokenSerializerFactory();
                var validator = new ContinuationTokenValidator(serializerFactory);

                var actual = validator.IsValidToken(string.Empty, this.CreateStub<IContinuationTokenOptions>());

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_True_When_Token_Whitespace()
            {
                var serializerFactory = new ContinuationTokenSerializerFactory();
                var validator = new ContinuationTokenValidator(serializerFactory);

                var actual = validator.IsValidToken(" ", this.CreateStub<IContinuationTokenOptions>());

                actual.Should().BeTrue();
            }
        }
    }
}