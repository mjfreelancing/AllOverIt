using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Pagination.TokenEncoding;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace AllOverIt.Pagination.Tests.TokenEncoding
{
    public class ContinuationTokenEncoderFactoryFixture : FixtureBase
    {
        private readonly ContinuationTokenEncoderFactory _continuationTokenEncoderFactory;

        public ContinuationTokenEncoderFactoryFixture()
        {
            var serializerFactory = new ContinuationTokenSerializerFactory();

            _continuationTokenEncoderFactory = new(serializerFactory);
        }

        public class Constructor : ContinuationTokenEncoderFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Factory_Null()
            {
                Invoking(() =>
                {
                    _ = new ContinuationTokenEncoderFactory(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("serializerFactory");
            }
        }

        public class CreateContinuationTokenEncoder : ContinuationTokenEncoderFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Columns_Null()
            {
                Invoking(() =>
                {
                    _ = _continuationTokenEncoderFactory.CreateContinuationTokenEncoder(null, Create<PaginationDirection>(), Create<ContinuationTokenOptions>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("columns");
            }

            [Fact]
            public void Should_Throw_When_Columns_Empty()
            {
                Invoking(() =>
                {
                    _ = _continuationTokenEncoderFactory.CreateContinuationTokenEncoder(new List<IColumnDefinition>(), Create<PaginationDirection>(),
                        Create<ContinuationTokenOptions>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("columns");
            }

            [Fact]
            public void Should_Throw_When_Options_Null()
            {
                Invoking(() =>
                {
                    _ = _continuationTokenEncoderFactory.CreateContinuationTokenEncoder(null, Create<PaginationDirection>(), Create<ContinuationTokenOptions>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("columns");
            }

            [Fact]
            public void Should_Create_Token_Encoder()
            {
                var actual = _continuationTokenEncoderFactory.CreateContinuationTokenEncoder(this.CreateManyStubs<IColumnDefinition>(), Create<PaginationDirection>(),
                    Create<ContinuationTokenOptions>());

                actual.Should().BeOfType<ContinuationTokenEncoder>();
                actual.Serializer.Should().BeOfType<ContinuationTokenSerializer>();
            }
        }
    }
}