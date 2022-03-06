#define AUTOMAPPER
using AllOverIt.Mapping;
using AllOverIt.Mapping.Extensions;
using AutoMapper;
using BenchmarkDotNet.Attributes;
using System;

namespace ObjectMappingBenchmarking
{
    /*
    |                                        Method |      Mean |    Error |   StdDev |   Gen 0 | Allocated |
    |---------------------------------------------- |----------:|---------:|---------:|--------:|----------:|
    |          AutoMapper_SimpleSource_SimpleTarget |  11.88 us | 0.158 us | 0.148 us |  0.9460 |      4 KB |
    | StaticMethod_SimpleSource_Create_SimpleTarget | 479.94 us | 7.457 us | 6.976 us | 81.5430 |    334 KB |
    | ObjectMapper_SimpleSource_Create_SimpleTarget | 106.30 us | 1.861 us | 1.650 us |  7.8125 |     32 KB |
    | ObjectMapper_SimpleSource_CopyTo_SimpleTarget | 102.02 us | 1.819 us | 1.786 us |  6.8359 |     28 KB |
    */


    [MemoryDiagnoser]
    public class MappingTests
    {
        private const int LoopCount = 100;

#if AUTOMAPPER
        private readonly IMapper _autoMapper;
#endif

        // ObjectMapper
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

            // ObjectMapper
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
            for (var i = 0; i < LoopCount; i++)
            {
                _ = _autoMapper.Map<SimpleTarget>(_simpleSource);
            }
        }
#endif

        [Benchmark]
        public void StaticMethod_SimpleSource_Create_SimpleTarget()
        {
            for (var i = 0; i < LoopCount; i++)
            {
                _ = _simpleSource.MapTo<SimpleTarget>();
            }
        }

        [Benchmark]
        public void ObjectMapper_SimpleSource_Create_SimpleTarget()
        {
            for (var i = 0; i < LoopCount; i++)
            {
                _ = _objectMapper.Map<SimpleTarget>(_simpleSource);
            }
        }

        [Benchmark]
        public void ObjectMapper_SimpleSource_CopyTo_SimpleTarget()
        {
            for (var i = 0; i < LoopCount; i++)
            {
                _ = _objectMapper.Map(_simpleSource, _simpleTarget);
            }
        }
    }
}
