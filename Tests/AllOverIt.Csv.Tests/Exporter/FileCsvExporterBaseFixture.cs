using AllOverIt.Csv.Exporter;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;

namespace AllOverIt.Csv.Tests.Exporter
{
    public class FileCsvExporterBaseFixture : FixtureBase
    {
        private sealed class DummyData
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }

        private sealed class DummyFileCsvExporter : FileCsvExporterBase<DummyData>
        {
            public DummyFileCsvExporter(string filePath, FileMode fileMode)
                : base(filePath, fileMode)
            {
            }

            public DummyFileCsvExporter(string filePath, FileMode fileMode, BufferedCsvExporterConfiguration configuration)
                : base(filePath, fileMode, configuration)
            {
            }

            protected override ICsvSerializer<DummyData> CreateSerializer(IEnumerable<DummyData> configData = null)
            {
                // Can't test a file-based serializer
                return null;
            }
        }

        public class Constructor : FileCsvExporterBaseFixture
        {
            [Fact]
            public void Should_Throw_When_FilePath_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(stringValue =>
                {
                    _ = new DummyFileCsvExporter(stringValue, Create<FileMode>());
                }, "filePath");
            }

            // Need a happy path test to get code coverage
            [Fact]
            public void Should_Not_Throw()
            {
                Invoking(() =>
                {
                    _ = new DummyFileCsvExporter(Create<string>(), Create<FileMode>());
                })
                .Should()
                .NotThrow();
            }
        }

        public class Constructor_Configuration : FileCsvExporterBaseFixture
        {
            [Fact]
            public void Should_Throw_When_FilePath_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(stringValue =>
                {
                    _ = new DummyFileCsvExporter(stringValue, FileMode.Create, new BufferedCsvExporterConfiguration());
                }, "filePath");
            }

            [Fact]
            public void Should_Throw_When_Configuration_Null()
            {
                Invoking(() =>
                {
                    _ = new DummyFileCsvExporter(Create<string>(), FileMode.Create, null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("configuration");
            }

            // Need a happy path test to get code coverage
            [Fact]
            public void Should_Not_Throw()
            {
                Invoking(() =>
                {
                    _ = new DummyFileCsvExporter(Create<string>(), FileMode.Create, new BufferedCsvExporterConfiguration());
                })
                .Should()
                .NotThrow();
            }
        }
    }
}
