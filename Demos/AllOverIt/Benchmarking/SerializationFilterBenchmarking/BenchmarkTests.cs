using AllOverIt.Extensions;
using AllOverIt.Formatters.Objects;
using BenchmarkDotNet.Attributes;

namespace SerializationFilterBenchmarking
{
    [MemoryDiagnoser]
    public class BenchmarkTests
    {
        private readonly ComplexObject _complexObject;
        private readonly ObjectPropertySerializer _serializer;

        public BenchmarkTests()
        {
            _complexObject = new ComplexObject
            {
                Items =
                [
                    new()
                    {
                        Name = "Name 1",
                        Factor = 1.1,
                        Data = new ComplexObject.Item.ItemData
                        {
                            Timestamp = DateTime.Now,
                            Values = Enumerable.Range(1, 5).SelectToReadOnlyCollection(value => value)
                        }
                    },
                    new()
                    {
                        Name = "Name 2",
                        Factor = 2.2,
                        Data = new ComplexObject.Item.ItemData
                        {
                            Timestamp = DateTime.Now,
                            Values = Enumerable.Range(11, 5).SelectToReadOnlyCollection(value => value)
                        }
                    },
                    new()
                    {
                        Name = "Name 3",
                        Factor = 3.3,
                        Data = new ComplexObject.Item.ItemData
                        {
                            Timestamp = DateTime.Now,
                            Values = Enumerable.Range(21, 5).SelectToReadOnlyCollection(value => value)
                        }
                    },
                ]
            };

            var filter = new ComplexObjectFilter();
            _serializer = new ObjectPropertySerializer(filter);
        }

        [Benchmark]
        public void CheckPerformance()
        {
            for (var i = 1; i <= 100; i++)
            {
                _ = _serializer.SerializeToDictionary(_complexObject);
            }
        }
    }
}
