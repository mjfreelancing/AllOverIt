using AllOverIt.Csv.Exceptions;
using AllOverIt.Csv.Exporter;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using CsvHelper.Configuration;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Csv.Tests.Exporter
{
    public class BufferedCsvExporterBaseFixture : FixtureBase
    {
        private sealed class DummyData
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }

        private sealed class DummyBufferedCsvExporter : BufferedCsvExporterBase<DummyData>
        {
            private Stream _stream;

            public IEnumerable<DummyData> ConfigData { get; private set; }

            public int Length => _stream is null
                ? 0
                : (int) _stream.Length;

            public DummyBufferedCsvExporter()
            {
            }

            public DummyBufferedCsvExporter(BufferedCsvExporterConfiguration configuration)
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

            protected override Stream CreateStream()
            {
                _stream = new MemoryStream();

                return _stream;
            }
        }

        private readonly DummyBufferedCsvExporter _exporter = new();

        public class Constructor_Configuration : BufferedCsvExporterBaseFixture
        {
            [Fact]
            public void Should_Throw_When_Configuration_Null()
            {
                Invoking(() =>
                {
                    _ = new DummyBufferedCsvExporter(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("configuration");
            }
        }

        public class Configure : BufferedCsvExporterBaseFixture
        {
            [Fact]
            public void Should_Not_Throw_When_ConfigData_Null()
            {
                Invoking(() =>
                {
                    _exporter.Configure(null);
                })
                .Should()
                .NotThrow();
            }

            [Fact]
            public void Should_Configure_With_Provided_Data()
            {
                var expected = CreateMany<DummyData>();

                _exporter.Configure(expected);

                _exporter.ConfigData.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Throw_When_Already_Configured()
            {
                _exporter.Configure();

                Invoking(() =>
                {
                    _exporter.Configure();
                })
                .Should()
                .Throw<CsvExporterException>()
                .WithMessage("The CSV serializer is already configured.");
            }
        }

        public class AddDataAsync : BufferedCsvExporterBaseFixture
        {
            [Fact]
            public async Task Should_Throw_When_Data_Null()
            {
                await Invoking(async () =>
                {
                    await _exporter.AddDataAsync(null, CancellationToken.None);
                })
                .Should()
                .ThrowAsync<ArgumentNullException>()
                .WithNamedMessageWhenNull("data");
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                _exporter.Configure();

                var cts = new CancellationTokenSource();
                cts.Cancel();

                await Invoking(async () =>
                {
                    await _exporter.AddDataAsync(Create<DummyData>(), cts.Token);
                })
                .Should()
                .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Add_Data()
            {
                _exporter.Configure();

                var data = Create<DummyData>();

                _exporter.Length.Should().Be(0);

                await _exporter.AddDataAsync(data, CancellationToken.None);

                await _exporter.FlushAsync(CancellationToken.None);

                _exporter.Length.Should().NotBe(0);
            }

            [Fact]
            public async Task Should_Flush_Data_When_Buffer_Full()
            {
                _exporter.Configure();

                var bufferSize = new BufferedCsvExporterConfiguration().BufferSize;
                DummyData data;

                for (var i = 0; i < bufferSize - 1; i++)
                {
                    data = Create<DummyData>();

                    await _exporter.AddDataAsync(data, CancellationToken.None);
                }

                _exporter.Length.Should().Be(0);

                data = Create<DummyData>();

                await _exporter.AddDataAsync(data, CancellationToken.None);

                _exporter.Length.Should().NotBe(0);
            }
        }

        public class FlushAsync : BufferedCsvExporterBaseFixture
        {
            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                _exporter.Configure();

                var cts = new CancellationTokenSource();
                cts.Cancel();

                await Invoking(async () =>
                {
                    await _exporter.FlushAsync(cts.Token);
                })
                .Should()
                .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Not_Throw_When_No_Data()
            {
                _exporter.Configure();

                await Invoking(async () =>
                {
                    await _exporter.FlushAsync(CancellationToken.None);
                })
                .Should()
                .NotThrowAsync();
            }

            [Fact]
            public async Task Should_Flush_Data()
            {
                _exporter.Configure();

                var data = Create<DummyData>();

                _exporter.Length.Should().Be(0);

                await _exporter.AddDataAsync(data, CancellationToken.None);

                _exporter.Length.Should().Be(0);

                await _exporter.FlushAsync(CancellationToken.None);

                _exporter.Length.Should().NotBe(0);
            }
        }

        public class CloseAsync : BufferedCsvExporterBaseFixture
        {
            [Fact]
            public async Task Should_Throw_When_Cancelled_When_No_Data()
            {
                _exporter.Configure();

                var cts = new CancellationTokenSource();
                cts.Cancel();

                await Invoking(async () =>
                {
                    await _exporter.CloseAsync(cts.Token);
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
                    await _exporter.CloseAsync(cts.Token);
                })
                .Should()
                .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Not_Throw_When_No_Data()
            {
                _exporter.Configure();

                await Invoking(async () =>
                {
                    await _exporter.CloseAsync(CancellationToken.None);
                })
                .Should()
                .NotThrowAsync();
            }

            [Fact]
            public async Task Should_Dispose_Stream()
            {
                _exporter.Configure();

                var data = Create<DummyData>();

                await _exporter.AddDataAsync(data, CancellationToken.None);
                await _exporter.CloseAsync(CancellationToken.None);

                Invoking(() =>
                {
                    // Will fail in the dummy exporter because the stream has been closed
                    _ = _exporter.Length;
                })
                .Should()
                .Throw<ObjectDisposedException>()
                .WithMessage("Cannot access a closed Stream.");
            }
        }

        public class DisposeAsync : BufferedCsvExporterBaseFixture
        {
            [Fact]
            public async Task Should_Not_Throw_When_Disposed_No_Data()
            {
                await Invoking(async () =>
                {
                    await using (_exporter)
                    {
                        _exporter.Configure();
                    }
                })
                .Should()
                .NotThrowAsync();
            }

            [Fact]
            public async Task Should_Not_Throw_When_Disposed_With_Data()
            {
                await Invoking(async () =>
                {
                    await using (_exporter)
                    {
                        _exporter.Configure();

                        var data = Create<DummyData>();

                        await _exporter.AddDataAsync(data, CancellationToken.None);
                    }
                })
                .Should()
                .NotThrowAsync();
            }
        }
    }
}
