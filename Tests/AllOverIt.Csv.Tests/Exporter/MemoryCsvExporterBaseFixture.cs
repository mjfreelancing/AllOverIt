using AllOverIt.Csv.Exporter;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System.Text;

namespace AllOverIt.Csv.Tests.Exporter
{
    public class MemoryCsvExporterBaseFixture : FixtureBase
    {
        private sealed class DummyData
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }

        private sealed class DummyMemoryCsvExporter : MemoryCsvExporterBase<DummyData>
        {
            public IEnumerable<DummyData> ConfigData { get; private set; }

            public DummyMemoryCsvExporter()
            {
            }

            public DummyMemoryCsvExporter(BufferedCsvExporterConfiguration configuration)
                : base(configuration)
            {
            }

            protected override ICsvSerializer<DummyData> CreateSerializer(IEnumerable<DummyData> configData = null)
            {
                ConfigData = configData;

                var serializer = new CsvSerializer<DummyData>();

                serializer.AddField(nameof(DummyData.Name), model => model.Name);
                serializer.AddField(nameof(DummyData.Value), model => model.Value);

                return serializer;
            }
        }

        private readonly DummyMemoryCsvExporter _exporter = new();

        public class Constructor_Configuration : MemoryCsvExporterBaseFixture
        {
            [Fact]
            public void Should_Throw_When_Configuration_Null()
            {
                Invoking(() =>
                {
                    _ = new DummyMemoryCsvExporter(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("configuration");
            }
        }

        public class GetContentAsync : MemoryCsvExporterBaseFixture
        {
            [Fact]
            public async Task Should_Throw_When_Cancelled_When_No_Data()
            {
                _exporter.Configure();

                var cts = new CancellationTokenSource();
                cts.Cancel();

                await Invoking(async () =>
                {
                    await _exporter.GetContentAsync(cts.Token);
                })
                .Should()
                .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled_With_Data()
            {
                _exporter.Configure();

                var data = Create<DummyData>();

                await _exporter.AddDataAsync(data, CancellationToken.None);

                var cts = new CancellationTokenSource();
                cts.Cancel();

                await Invoking(async () =>
                {
                    await _exporter.GetContentAsync(cts.Token);
                })
                .Should()
                .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Return_Empty_Array_When_Headers_And_No_Data()
            {
                var configuration = new BufferedCsvExporterConfiguration
                {
                    IncludeHeaders = false
                };

                var exporter = new DummyMemoryCsvExporter(configuration);

                exporter.Configure();

                var actual = await exporter.GetContentAsync(CancellationToken.None);

                actual.Should().BeEmpty();
            }

            [Fact]
            public async Task Should_Return_Headers_Only_When_No_Data()
            {
                _exporter.Configure();

                var bytes = await _exporter.GetContentAsync(CancellationToken.None);

                // CsvSerializer is currently using default configuration when creating CsvWriter, so NewLine is CRLF
                var expected = $"{nameof(DummyData.Name)},{nameof(DummyData.Value)}\r\n";

                var actual = Encoding.UTF8.GetString(bytes);

                actual.Should().Be(expected);
            }
        }
    }
}
