#define AUTOMAPPER
using AllOverIt.Mapping;
using AllOverIt.Mapping.Extensions;
using AutoMapper;
using BenchmarkDotNet.Attributes;
using System;
using BenchmarkDotNet.Jobs;

namespace ObjectMappingBenchmarking
{
    /*
    |                                        Method |       Mean |    Error |   StdDev | Allocated |
    |---------------------------------------------- |-----------:|---------:|---------:|----------:|
    |          AutoMapper_SimpleSource_SimpleTarget |   222.2 ns | 19.19 ns | 54.14 ns |      40 B |
    | StaticMethod_SimpleSource_Create_SimpleTarget | 4,272.8 ns | 75.40 ns | 66.84 ns |   2,544 B |
    | ObjectMapper_SimpleSource_Create_SimpleTarget |   961.1 ns | 18.63 ns | 23.57 ns |      88 B |
    | ObjectMapper_SimpleSource_CopyTo_SimpleTarget |   908.2 ns | 11.79 ns | 10.45 ns |      48 B |
    */

    [MemoryDiagnoser(false)]
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [SimpleJob(RuntimeMoniker.Net50)]
    [SimpleJob(RuntimeMoniker.Net60)]
    public class MappingTests
    {
#if AUTOMAPPER
        private readonly IMapper _autoMapper;
#endif

        private readonly IObjectMapper _objectMapper;

        private readonly SimpleSource _simpleSource;
        private readonly SimpleTarget _simpleTarget;

        public MappingTests()
        {
#if AUTOMAPPER
            var autoMapperConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<SimpleSource, SimpleTarget>();
            });

            _autoMapper = new Mapper(autoMapperConfig);
#endif

            _objectMapper = new ObjectMapper();

            _objectMapper.Configure<SimpleSource, SimpleTarget>();

            _simpleSource = new SimpleSource
            {
                Prop1 = 1,
                Prop2 = "Some Text",
                TimestampUtc = DateTime.UtcNow
            };

            _simpleTarget = new SimpleTarget();
        }

#if AUTOMAPPER
        [Benchmark]
        public void AutoMapper_SimpleSource_SimpleTarget()
        {
            _ = _autoMapper.Map<SimpleTarget>(_simpleSource);
        }
#endif

        [Benchmark]
        public void StaticMethod_SimpleSource_Create_SimpleTarget()
        {
            _ = _simpleSource.MapTo<SimpleTarget>();
        }

        [Benchmark]
        public void ObjectMapper_SimpleSource_Create_SimpleTarget()
        {
            _ = _objectMapper.Map<SimpleTarget>(_simpleSource);
        }

        [Benchmark]
        public void ObjectMapper_SimpleSource_CopyTo_SimpleTarget()
        {
            _ = _objectMapper.Map(_simpleSource, _simpleTarget);
        }
    }
}
