﻿using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Xunit;

namespace AllOverIt.Csv.Tests
{
    public class CsvSerializerFixture : FixtureBase
    {
        private sealed class CoordinatesDummy
        {
            public double Latitude { get; }
            public double Longitude { get; }

            public CoordinatesDummy(double latitude, double longitude)
            {
                Latitude = latitude;
                Longitude = longitude;
            }
        }

        private sealed class SampleDataDummy
        {
            public string Name { get; set; }
            public int Count { get; set; }
            public IReadOnlyCollection<CoordinatesDummy> Coordinates { get; set; }
        }

        private readonly CsvSerializer<SampleDataDummy> _serializer = new();

        public class AddField : CsvSerializerFixture
        {
            [Fact]
            public void Should_Throw_When_HeaderName_Null()
            {
                Invoking(() =>
                    {
                        _serializer.AddField(null, _ => null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("headerName");
            }

            [Fact]
            public void Should_Throw_When_HeaderName_Empty()
            {
                Invoking(() =>
                    {
                        _serializer.AddField(string.Empty, _ => null);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("headerName");
            }

            [Fact]
            public void Should_Throw_When_HeaderName_Whitespace()
            {
                Invoking(() =>
                    {
                        _serializer.AddField("  ", _ => null);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("headerName");
            }

            [Fact]
            public void Should_Throw_When_Resolver_Null()
            {
                Invoking(() =>
                    {
                        _serializer.AddField(Create<string>(), null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("valueResolver");
            }

            [Fact]
            public async Task Should_Add_Field()
            {
                var sampleData = CreateSampleDataWithNames();

                _serializer.AddField(nameof(SampleDataDummy.Name), data => data.Name);

                string actual;

                using (var writer = new StringWriter())
                {
                    await _serializer.SerializeAsync(writer, sampleData);
                    actual = writer.ToString();
                }

                var sb = new StringBuilder();
                sb.AppendLine(nameof(SampleDataDummy.Name));

                foreach (var data in sampleData)
                {
                    sb.AppendLine(data.Name);
                }

                var expected = sb.ToString();

                expected.Should().Be(actual);
            }

            [Fact]
            public async Task Should_Add_Multiple_Fields()
            {
                var names = CreateMany<string>();
                var counts = CreateMany<int>();

                var sampleData = names
                    .Zip(counts, (name, count) => new {Name = name, Count = count})
                    .Select(value => new SampleDataDummy
                    {
                        Name = value.Name,
                        Count = value.Count
                    })
                    .AsReadOnlyCollection();

                _serializer.AddField(nameof(SampleDataDummy.Name), data => data.Name);
                _serializer.AddField(nameof(SampleDataDummy.Count), data => data.Count);

                string actual;

                using (var writer = new StringWriter())
                {
                    await _serializer.SerializeAsync(writer, sampleData);
                    actual = writer.ToString();
                }

                var sb = new StringBuilder();
                sb.AppendLine($"{nameof(SampleDataDummy.Name)},{nameof(SampleDataDummy.Count)}");

                foreach (var data in sampleData)
                {
                    sb.AppendLine($"{data.Name},{data.Count}");
                }

                var expected = sb.ToString();

                expected.Should().Be(actual);
            }
        }

        public class AddFields : CsvSerializerFixture
        {
            [Fact]
            public void Should_Throw_When_HeaderName_Null()
            {
                Invoking(() =>
                    {
                        _serializer.AddFields(null, _ => null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("headerNames");
            }

            [Fact]
            public void Should_Throw_When_Resolver_Null()
            {
                Invoking(() =>
                    {
                        _serializer.AddFields(CreateMany<string>(), null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("valuesResolver");
            }

            [Fact]
            public async Task Should_Add_Fields()
            {
                var sampleData = new[]
                {
                    new SampleDataDummy()
                    {
                        Coordinates = new[]
                        {
                            new CoordinatesDummy(Create<double>(), Create<double>()),
                            new CoordinatesDummy(Create<double>(), Create<double>())
                        }
                    },
                    new SampleDataDummy()
                    {
                        Coordinates = new[]
                        {
                            new CoordinatesDummy(Create<double>(), Create<double>()),
                            new CoordinatesDummy(Create<double>(), Create<double>()),
                            new CoordinatesDummy(Create<double>(), Create<double>())
                        }
                    }
                };

                // dynamically create unique header names based on the maximum number of co-ordinates per row
                var maxCount = sampleData
                    .Select(item => item.Coordinates.Count)
                    .Max();

                var uniqueIdentifiers = Enumerable
                    .Range(0, maxCount)
                    .SelectAsReadOnlyCollection(idx => new
                    {
                        Index = idx,
                        Names = new List<string>(new[] {$"Latitude {idx + 1}", $"Longitude {idx + 1}"})
                    });

                var headerNames = uniqueIdentifiers
                    .SelectMany(item => item.Names)
                    .AsReadOnlyCollection();

                foreach (var identifier in uniqueIdentifiers)
                {
                    _serializer.AddFields(identifier.Names, row =>
                    {
                        var field = row.Coordinates;

                        if (identifier.Index < field.Count)
                        {
                            var coordinate = field.ElementAt(identifier.Index);

                            // Since two headers were exported, there is an expectation that two values will be returned
                            return new object[]
                            {
                                coordinate.Latitude,
                                coordinate.Longitude
                            };
                        }

                        // no values for the provided index (blank CSV cells)
                        return Enumerable.Repeat((object) null, identifier.Names.Count).ToList();
                    });
                }

                // Get the actual output
                string actual;

                using (var writer = new StringWriter())
                {
                    await _serializer.SerializeAsync(writer, sampleData);
                    actual = writer.ToString();
                }

                // Build up the expected output
                var sb = new StringBuilder();
                sb.AppendLine(string.Join(",", headerNames));

                foreach (var data in sampleData)
                {
                    var rowData = data.Coordinates
                        .SelectMany(item => new[]
                        {
                            $"{item.Latitude}",
                            $"{item.Longitude}"
                        })
                        .ToList();

                    var blankFieldCount = maxCount * 2 - rowData.Count();
                    rowData.AddRange(Enumerable.Repeat(string.Empty, blankFieldCount));

                    sb.AppendLine(string.Join(",", rowData));
                }

                // Example:
                // Latitude 1,Longitude 1,Latitude 2,Longitude 2,Latitude 3,Longitude 3
                // 111,10,20,56,,
                // 59,28,43,182,224,94

                var expected = sb.ToString();

                expected.Should().Be(actual);
            }
        }

        public class SerializeAsync : CsvSerializerFixture
        {
            [Fact]
            public async Task Should_Throw_When_Writer_Null()
            {
                await Invoking(async () =>
                    {
                        await _serializer.SerializeAsync(null, CreateMany<SampleDataDummy>());
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public async Task Should_Throw_When_Data_Null()
            {
                await Invoking(async () =>
                    {
                        await _serializer.SerializeAsync(A.Fake<TextWriter>(), null);
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("data");
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public async Task Should_Serialize_Data(bool withHeader)
            {
                var sampleData = CreateSampleDataWithNames();

                _serializer.AddField(nameof(SampleDataDummy.Name), data => data.Name);

                string actual;

                using (var writer = new StringWriter())
                {
                    await _serializer.SerializeAsync(writer, sampleData, withHeader);
                    actual = writer.ToString();
                }

                var expected = GetExpectedOutputForDataWithNames(sampleData, withHeader);

                expected.Should().Be(actual);
            }
        }

        private IReadOnlyCollection<SampleDataDummy> CreateSampleDataWithNames()
        {
            return CreateMany<string>()
                .Select(value => new SampleDataDummy { Name = value })
                .AsReadOnlyCollection();
        }

        private static string GetExpectedOutputForDataWithNames(IEnumerable<SampleDataDummy> sampleData, bool includeHeader)
        {
            var sb = new StringBuilder();

            if (includeHeader)
            {
                sb.AppendLine(nameof(SampleDataDummy.Name));
            }

            foreach (var data in sampleData)
            {
                sb.AppendLine(data.Name);
            }

            return sb.ToString();
        }
    }
}
