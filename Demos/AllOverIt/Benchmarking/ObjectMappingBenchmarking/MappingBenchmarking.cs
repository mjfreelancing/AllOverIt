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
    |          AutoMapper_SimpleSource_SimpleTarget |  11.77 us | 0.108 us | 0.084 us |  0.9460 |      4 KB |
    | StaticMethod_SimpleSource_Create_SimpleTarget | 486.34 us | 7.561 us | 7.073 us | 81.5430 |    334 KB |
    | ObjectMapper_SimpleSource_Create_SimpleTarget | 248.86 us | 4.238 us | 3.964 us | 37.3535 |    153 KB |
    | ObjectMapper_SimpleSource_CopyTo_SimpleTarget | 241.13 us | 3.766 us | 3.338 us | 36.3770 |    149 KB |
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
