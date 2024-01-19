using AllOverIt.Csv.Exporter;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

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
            public void Should_Throw_When_FilePath_Null()
            {
                Invoking(() =>
                {
                    _ = new DummyFileCsvExporter(null, Create<FileMode>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("filePath");
            }

            [Fact]
            public void Should_Throw_When_FilePath_Empty()
            {
                Invoking(() =>
                {
                    _ = new DummyFileCsvExporter(string.Empty, Create<FileMode>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("filePath");
            }

            [Fact]
            public void Should_Throw_When_FilePath_Whitespace()
            {
                Invoking(() =>
                {
                    _ = new DummyFileCsvExporter("  ", Create<FileMode>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("filePath");
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
            public void Should_Throw_When_FilePath_Null()
            {
                Invoking(() =>
                {
                    _ = new DummyFileCsvExporter(null, FileMode.Create, new BufferedCsvExporterConfiguration());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("filePath");
            }

            [Fact]
            public void Should_Throw_When_FilePath_Empty()
            {
                Invoking(() =>
                {
                    _ = new DummyFileCsvExporter(string.Empty, FileMode.Create, new BufferedCsvExporterConfiguration());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("filePath");
            }

            [Fact]
            public void Should_Throw_When_FilePath_Whitespace()
            {
                Invoking(() =>
                {
                    _ = new DummyFileCsvExporter("  ", FileMode.Create, new BufferedCsvExporterConfiguration());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("filePath");
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
