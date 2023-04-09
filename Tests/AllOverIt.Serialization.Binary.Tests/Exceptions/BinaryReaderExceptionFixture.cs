﻿using AllOverIt.Fixture;
using AllOverIt.Serialization.Binary.Exceptions;
using AllOverIt.Tests.Helpers;
using Xunit;

namespace AllOverIt.Serialization.Binary.Tests.Exceptions
{
    public class BinaryReaderExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            Fixture.AssertDefaultConstructor<BinaryReaderException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            Fixture.AssertConstructorWithMessage<BinaryReaderException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            Fixture.AssertConstructorWithMessageAndInnerException<BinaryReaderException>();
        }
    }
}