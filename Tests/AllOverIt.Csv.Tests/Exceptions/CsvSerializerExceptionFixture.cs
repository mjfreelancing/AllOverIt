using AllOverIt.Csv.Exceptions;
using AllOverIt.Fixture;

namespace AllOverIt.Csv.Tests.Exceptions
{
    public class CsvSerializerExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<CsvSerializerException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<CsvSerializerException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<CsvSerializerException>();
        }
    }
}