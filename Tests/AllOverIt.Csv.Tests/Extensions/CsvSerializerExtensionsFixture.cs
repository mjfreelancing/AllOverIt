using AllOverIt.Csv.Extensions;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllOverIt.Csv.Exceptions;
using Xunit;

namespace AllOverIt.Csv.Tests.Extensions
{
    public class CsvSerializerExtensionsFixture : FixtureBase
    {
        private sealed class SampleDataDummy
        {
            public class ChildDummy
            {
                public string Name { get; set; }
                public int Value { get; set; }
                public bool Active { get; set; }
            }

            public IDictionary<string, double> KeyValues { get; set; }
            public ChildDummy Child { get; set; }
            public IReadOnlyCollection<ChildDummy> Children { get; set; }
        }

        private readonly CsvSerializer<SampleDataDummy> _serializer = new();
        private readonly IReadOnlyCollection<SampleDataDummy> _sampleData;

        public CsvSerializerExtensionsFixture()
        {
            var duplicateKeys = CreateMany<string>();

            _sampleData = new List<SampleDataDummy>()
            {
                new SampleDataDummy
                {
                    KeyValues = CreateMany<KeyValuePair<string, double>>()
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),

                    Child = Create<SampleDataDummy.ChildDummy>(),
                    Children = CreateMany<SampleDataDummy.ChildDummy>()
                },
                new SampleDataDummy
                {
                    KeyValues = duplicateKeys
                        .Select(key => new KeyValuePair<string, double>(key, Create<double>()))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),

                    Child = Create<SampleDataDummy.ChildDummy>(),
                    Children = CreateMany<SampleDataDummy.ChildDummy>()
                },
                new SampleDataDummy
                {
                    KeyValues = CreateMany<KeyValuePair<string, double>>()
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),

                    Child = Create<SampleDataDummy.ChildDummy>(),
                    Children = CreateMany<SampleDataDummy.ChildDummy>()
                },
                new SampleDataDummy
                {
                    KeyValues = duplicateKeys
                        .Select(key => new KeyValuePair<string, double>(key, Create<double>()))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),

                    Child = Create<SampleDataDummy.ChildDummy>(),
                    Children = CreateMany<SampleDataDummy.ChildDummy>()
                },
                new SampleDataDummy
                {
                    KeyValues = CreateMany<KeyValuePair<string, double>>()
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),

                    Child = Create<SampleDataDummy.ChildDummy>(),
                    Children = CreateMany<SampleDataDummy.ChildDummy>()
                },
            };
        }

        public class AddDynamicFields_Field : CsvSerializerExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Serializer_Null()
            {
                Invoking(() =>
                    {
                        CsvSerializerExtensions.AddDynamicFields<SampleDataDummy, IDictionary<string, double>>(null, _sampleData, _ => null, _ => null, (_, _) => null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("serializer");
            }

            [Fact]
            public void Should_Throw_When_Data_Null()
            {
                Invoking(() =>
                    {
                        CsvSerializerExtensions.AddDynamicFields<SampleDataDummy, IDictionary<string, double>>(_serializer, null, _ => null, _ => null, (_, _) => null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("data");
            }

            [Fact]
            public void Should_Throw_When_Field_Selector_Null()
            {
                Invoking(() =>
                    {
                        CsvSerializerExtensions.AddDynamicFields<SampleDataDummy, IDictionary<string, double>>(_serializer, _sampleData, null, _ => null, (_, _) => null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("fieldSelector");
            }

            [Fact]
            public void Should_Throw_When_Header_Name_Resolver_Null()
            {
                Invoking(() =>
                    {
                        CsvSerializerExtensions.AddDynamicFields<SampleDataDummy, IDictionary<string, double>>(_serializer, _sampleData, _ => null, null, (_, _) => null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("headerNameResolver");
            }

            [Fact]
            public void Should_Throw_When_Value_Resolver_Null()
            {
                Invoking(() =>
                    {
                        CsvSerializerExtensions.AddDynamicFields<SampleDataDummy, IDictionary<string, double>>(_serializer, _sampleData, _ => null, _ => null, null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("valueResolver");
            }

            [Fact]
            public async Task Should_Export_Dynamic_Columns_From_Dictionary()
            {
                CsvSerializerExtensions.AddDynamicFields<SampleDataDummy, IDictionary<string, double>>(
                    _serializer,
                    _sampleData,
                    data => data.KeyValues,     // The property
                    data => data.Keys,          // All keys in the property
                    (data, identifier) =>
                    {
                        if (data.TryGetValue(identifier, out var value))
                        {
                            return value;
                        }

                        return null;
                    }
                );

                // Get the actual output
                string actual;

                using (var writer = new StringWriter())
                {
                    await _serializer.SerializeAsync(writer, _sampleData);
                    actual = writer.ToString();
                }

                // Build the expected output
                var sb = new StringBuilder();

                var headerNames = _sampleData
                    .SelectMany(item => item.KeyValues)
                    .Select(item => item.Key)
                    .Distinct()
                    .AsReadOnlyCollection();

                sb.AppendLine(string.Join(",", headerNames));

                foreach (var data in _sampleData)
                {
                    var cellValues = new List<string>();

                    foreach (var headerName in headerNames)
                    {
                        var cellValue = data.KeyValues.TryGetValue(headerName, out var value)
                            ? $"{value}"
                            : string.Empty;

                        cellValues.Add(cellValue);
                    }

                    sb.AppendLine(string.Join(",", cellValues));
                }

                var expected = sb.ToString();

                expected.Should().Be(actual);
            }

            [Fact]
            public async Task Should_Export_Dynamic_Columns_From_Object()
            {
                CsvSerializerExtensions.AddDynamicFields<SampleDataDummy, SampleDataDummy.ChildDummy>(
                    _serializer,
                    _sampleData,
                    data => data.Child,                                 // The property
                    data => new []                                      // The dynamic columns
                        {
                            nameof(SampleDataDummy.ChildDummy.Name),
                            nameof(SampleDataDummy.ChildDummy.Value)
                        },
                    (data, identifier) => identifier == nameof(SampleDataDummy.ChildDummy.Name)
                        ? data.Name
                        : data.Value);

                // Get the actual output
                string actual;

                using (var writer = new StringWriter())
                {
                    await _serializer.SerializeAsync(writer, _sampleData);
                    actual = writer.ToString();
                }

                // Build the expected output
                var sb = new StringBuilder();
                sb.AppendLine($"{nameof(SampleDataDummy.ChildDummy.Name)},{nameof(SampleDataDummy.ChildDummy.Value)}");

                foreach (var data in _sampleData)
                {
                    sb.AppendLine($"{data.Child.Name},{data.Child.Value}");
                }

                var expected = sb.ToString();

                expected.Should().Be(actual);
            }
        }

        public class AddDynamicFields_Field_FieldId : CsvSerializerExtensionsFixture
        {
                [Fact]
            public void Should_Throw_When_Serializer_Null()
            {
                Invoking(() =>
                    {
                        CsvSerializerExtensions.AddDynamicFields<SampleDataDummy, IReadOnlyCollection<SampleDataDummy.ChildDummy>, int>(
                            null,
                            _sampleData,
                            data => data.Children,
                            field => new FieldIdentifier<int>[]{},
                            (field, identifier) => new object[]{}
                        );
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("serializer");
            }

            [Fact]
            public void Should_Throw_When_Data_Null()
            {
                Invoking(() =>
                    {
                        CsvSerializerExtensions.AddDynamicFields<SampleDataDummy, IReadOnlyCollection<SampleDataDummy.ChildDummy>, int>(
                            _serializer,
                            null,
                            data => data.Children,
                            field => new FieldIdentifier<int>[] { },
                            (field, identifier) => new object[] { }
                        );
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("data");
            }

            [Fact]
            public void Should_Throw_When_Field_Selector_Null()
            {
                Invoking(() =>
                    {
                        CsvSerializerExtensions.AddDynamicFields<SampleDataDummy, IReadOnlyCollection<SampleDataDummy.ChildDummy>, int>(
                            _serializer,
                            _sampleData,
                            null,
                            field => new FieldIdentifier<int>[] { },
                            (field, identifier) => new object[] { }
                        );
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("fieldSelector");
            }

            [Fact]
            public void Should_Throw_When_Field_Identifiers_Null()
            {
                Invoking(() =>
                    {
                        CsvSerializerExtensions.AddDynamicFields<SampleDataDummy, IReadOnlyCollection<SampleDataDummy.ChildDummy>, int>(
                            _serializer,
                            _sampleData,
                            data => data.Children,
                            null,
                            (field, identifier) => new object[] { }
                        );
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("fieldIdentifiers");
            }

            [Fact]
            public void Should_Throw_When_Values_Resolver_Null()
            {
                Invoking(() =>
                    {
                        CsvSerializerExtensions.AddDynamicFields<SampleDataDummy, IReadOnlyCollection<SampleDataDummy.ChildDummy>, int>(
                            _serializer,
                            _sampleData,
                            data => data.Children,
                            field => new FieldIdentifier<int>[] { },
                            null
                        );
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("valuesResolver");
            }

            [Fact]
            public async Task Should_Export_Dynamic_Columns_From_Object()
            {
                CsvSerializerExtensions.AddDynamicFields<SampleDataDummy, IReadOnlyCollection<SampleDataDummy.ChildDummy>, KeyValuePair<string, int>>(
                    _serializer,
                    _sampleData,
                    data => data.Children,
                    field => 
                    {
                        return field
                            .Select(child =>
                            {
                                return new FieldIdentifier<KeyValuePair<string, int>>
                                {
                                    // Used as a lookup key
                                    Id = new KeyValuePair<string, int>(child.Name, child.Value),

                                    // One or more columns to output
                                    Names = new[]
                                    {
                                        $"{child.Name}-{child.Value}"
                                    }
                                };
                            });
                    },
                    (item, headerId) =>
                    {
                        var (name, value) = headerId.Id;

                        var child = item.SingleOrDefault(data => data.Name == name && data.Value == value);

                        return child == null
                            ? null
                            : new object[]
                            {
                                child.Active
                            };
                    }
                );

                // Get the actual output
                string actual;

                using (var writer = new StringWriter())
                {
                    await _serializer.SerializeAsync(writer, _sampleData);
                    actual = writer.ToString();
                }

                // Build the expected output
                var sb = new StringBuilder();

                var headerNames = _sampleData
                    .SelectMany(item => item.Children)
                    .SelectAsReadOnlyCollection(item => $"{item.Name}-{item.Value}");

                sb.AppendLine(string.Join(",", headerNames));

                foreach (var data in _sampleData)
                {
                    var cellValues = new List<string>();

                    foreach (var headerName in headerNames)
                    {
                        var child = data.Children.SingleOrDefault(item => headerName == $"{item.Name}-{item.Value}");

                        var cellValue = child != null
                            ? $"{child.Active}"
                            : string.Empty;

                        cellValues.Add(cellValue);
                    }

                    sb.AppendLine(string.Join(",", cellValues));
                }

                var expected = sb.ToString();

                expected.Should().Be(actual);
            }

            [Fact]
            public async Task Should_Throw_When_Column_Counts_Mismatch()
            {
                CsvSerializerExtensions.AddDynamicFields<SampleDataDummy, IReadOnlyCollection<SampleDataDummy.ChildDummy>, KeyValuePair<string, int>>(
                    _serializer,
                    _sampleData,
                    data => data.Children,
                    field =>
                    {
                        return field
                            .Select(child =>
                            {
                                return new FieldIdentifier<KeyValuePair<string, int>>
                                {
                                    // Used as a lookup key
                                    Id = new KeyValuePair<string, int>(child.Name, child.Value),

                                    // One or more columns to output
                                    Names = new[]
                                    {
                                        // Returning two columns from here but only a single value below
                                        child.Name,
                                        $"{child.Value}"
                                    }
                                };
                            });
                    },
                    (item, headerId) =>
                    {
                        var (name, value) = headerId.Id;

                        var child = item.SingleOrDefault(data => data.Name == name && data.Value == value);

                        return child == null
                            ? null
                            : new object[]
                            {
                                // Only one value here, but two columns are defined above
                                child.Active
                            };
                    }
                );

                await Invoking(async () =>
                    {
                        using (var writer = new StringWriter())
                        {
                            await _serializer.SerializeAsync(writer, _sampleData);
                        }
                    })
                    .Should()
                    .ThrowAsync<CsvExportException>()
                    .WithMessage("Column count mismatch. Expected 2, found 1.");
            }
        }
    }
}