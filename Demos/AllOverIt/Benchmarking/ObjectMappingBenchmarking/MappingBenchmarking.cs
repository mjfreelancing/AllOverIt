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
    |          AutoMapper_SimpleSource_SimpleTarget |  10.74 us | 0.098 us | 0.087 us |  0.9460 |      4 KB |
    | StaticMethod_SimpleSource_Create_SimpleTarget | 367.74 us | 7.249 us | 9.167 us | 59.0820 |    242 KB |
    | ObjectMapper_SimpleSource_Create_SimpleTarget | 101.96 us | 1.585 us | 1.482 us |  6.5918 |     27 KB |
    | ObjectMapper_SimpleSource_CopyTo_SimpleTarget |  98.61 us | 1.407 us | 1.247 us |  5.7373 |     23 KB |
    */

    [MemoryDiagnoser]
    public class MappingTests
    {
        private const int LoopCount = 100;

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
