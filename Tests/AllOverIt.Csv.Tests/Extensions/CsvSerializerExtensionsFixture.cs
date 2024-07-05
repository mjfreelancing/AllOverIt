using AllOverIt.Csv.Exceptions;
using AllOverIt.Csv.Extensions;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System.Text;

namespace AllOverIt.Csv.Tests.Extensions
{
    public class CsvSerializerExtensionsFixture : FixtureBase
    {
        private sealed class DummySampleData
        {
            public class DummyChild
            {
                public string Name { get; set; }
                public int Value { get; set; }
                public bool Active { get; set; }
            }

            public IDictionary<string, double> KeyValues { get; set; }
            public DummyChild Child { get; set; }
            public IReadOnlyCollection<DummyChild> Children { get; set; }
        }

        private readonly CsvSerializer<DummySampleData> _serializer = new();
        private readonly IReadOnlyCollection<DummySampleData> _sampleData;

        public CsvSerializerExtensionsFixture()
        {
            var duplicateKeys = CreateMany<string>();

            _sampleData = new List<DummySampleData>()
            {
                new DummySampleData
                {
                    KeyValues = CreateMany<KeyValuePair<string, double>>()
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),

                    Child = Create<DummySampleData.DummyChild>(),
                    Children = CreateMany<DummySampleData.DummyChild>()
                },
                new DummySampleData
                {
                    KeyValues = duplicateKeys
                        .Select(key => new KeyValuePair<string, double>(key, Create<double>()))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),

                    Child = Create<DummySampleData.DummyChild>(),
                    Children = CreateMany<DummySampleData.DummyChild>()
                },
                new DummySampleData
                {
                    KeyValues = CreateMany<KeyValuePair<string, double>>()
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),

                    Child = Create<DummySampleData.DummyChild>(),
                    Children = CreateMany<DummySampleData.DummyChild>()
                },
                new DummySampleData
                {
                    KeyValues = duplicateKeys
                        .Select(key => new KeyValuePair<string, double>(key, Create<double>()))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),

                    Child = Create<DummySampleData.DummyChild>(),
                    Children = CreateMany<DummySampleData.DummyChild>()
                },
                new DummySampleData
                {
                    KeyValues = CreateMany<KeyValuePair<string, double>>()
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),

                    Child = Create<DummySampleData.DummyChild>(),
                    Children = CreateMany<DummySampleData.DummyChild>()
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
                        CsvSerializerExtensions.AddDynamicFields<DummySampleData, IDictionary<string, double>>(null, _sampleData, _ => null, _ => null, (_, _) => null);
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
                        CsvSerializerExtensions.AddDynamicFields<DummySampleData, IDictionary<string, double>>(_serializer, null, _ => null, _ => null, (_, _) => null);
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
                        CsvSerializerExtensions.AddDynamicFields<DummySampleData, IDictionary<string, double>>(_serializer, _sampleData, null, _ => null, (_, _) => null);
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
                        CsvSerializerExtensions.AddDynamicFields<DummySampleData, IDictionary<string, double>>(_serializer, _sampleData, _ => null, null, (_, _) => null);
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
                        CsvSerializerExtensions.AddDynamicFields<DummySampleData, IDictionary<string, double>>(_serializer, _sampleData, _ => null, _ => null, null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("valueResolver");
            }

            [Fact]
            public async Task Should_Export_Dynamic_Columns_From_Dictionary()
            {
                CsvSerializerExtensions.AddDynamicFields<DummySampleData, IDictionary<string, double>>(
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
                CsvSerializerExtensions.AddDynamicFields<DummySampleData, DummySampleData.DummyChild>(
                    _serializer,
                    _sampleData,
                    data => data.Child,                                 // The property
                    data => new[]                                      // The dynamic columns
                        {
                            nameof(DummySampleData.DummyChild.Name),
                            nameof(DummySampleData.DummyChild.Value)
                        },
                    (data, identifier) => identifier == nameof(DummySampleData.DummyChild.Name)
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
                sb.AppendLine($"{nameof(DummySampleData.DummyChild.Name)},{nameof(DummySampleData.DummyChild.Value)}");

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
                        CsvSerializerExtensions.AddDynamicFields<DummySampleData, IReadOnlyCollection<DummySampleData.DummyChild>, int>(
                            null,
                            _sampleData,
                            data => data.Children,
                            field => Array.Empty<FieldIdentifier<int>>(),
                            (field, identifier) => Array.Empty<object>()
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
                        CsvSerializerExtensions.AddDynamicFields<DummySampleData, IReadOnlyCollection<DummySampleData.DummyChild>, int>(
                            _serializer,
                            null,
                            data => data.Children,
                            field => Array.Empty<FieldIdentifier<int>>(),
                            (field, identifier) => Array.Empty<object>()
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
                        CsvSerializerExtensions.AddDynamicFields<DummySampleData, IReadOnlyCollection<DummySampleData.DummyChild>, int>(
                            _serializer,
                            _sampleData,
                            null,
                            field => Array.Empty<FieldIdentifier<int>>(),
                            (field, identifier) => Array.Empty<object>()
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
                        CsvSerializerExtensions.AddDynamicFields<DummySampleData, IReadOnlyCollection<DummySampleData.DummyChild>, int>(
                            _serializer,
                            _sampleData,
                            data => data.Children,
                            null,
                            (field, identifier) => Array.Empty<object>()
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
                        CsvSerializerExtensions.AddDynamicFields<DummySampleData, IReadOnlyCollection<DummySampleData.DummyChild>, int>(
                            _serializer,
                            _sampleData,
                            data => data.Children,
                            field => Array.Empty<FieldIdentifier<int>>(),
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
                CsvSerializerExtensions.AddDynamicFields<DummySampleData, IReadOnlyCollection<DummySampleData.DummyChild>, KeyValuePair<string, int>>(
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
                    .SelectToReadOnlyCollection(item => $"{item.Name}-{item.Value}");

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
                CsvSerializerExtensions.AddDynamicFields<DummySampleData, IReadOnlyCollection<DummySampleData.DummyChild>, KeyValuePair<string, int>>(
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
                    .ThrowAsync<CsvSerializerException>()
                    .WithMessage("Column count mismatch. Expected 2, found 1.");
            }
        }
    }
}