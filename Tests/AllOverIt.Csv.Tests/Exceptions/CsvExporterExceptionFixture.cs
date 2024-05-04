using AllOverIt.Csv.Exceptions;
using AllOverIt.Fixture;

namespace AllOverIt.Csv.Tests.Exceptions
{
    public class CsvExporterExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<CsvExporterException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<CsvExporterException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<CsvExporterException>();
        }
    }
}