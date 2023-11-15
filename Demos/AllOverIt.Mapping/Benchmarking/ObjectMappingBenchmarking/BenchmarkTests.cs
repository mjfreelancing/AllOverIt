//#define AUTOMAPPER
using AllOverIt.Mapping;
using BenchmarkDotNet.Attributes;
using System;

#if AUTOMAPPER
    using AutoMapper;
#endif

namespace ObjectMappingBenchmarking
{
    /*
    // 07/12/2022

    |                                     Method |         Mean |   Gen0 |   Gen1 | Allocated |
    |------------------------------------------- |-------------:|-------:|-------:|----------:|
    | ObjectMapper_New_Mapper_Explicit_Configure | 773,364.1 ns | 3.9063 | 1.9531 |   34451 B |
    | ObjectMapper_New_Mapper_Implicit_Configure | 569,549.5 ns | 4.8828 | 3.9063 |   34372 B |
    |   ObjectMapper_PreConfigured_Create_Target |     279.6 ns | 0.0367 |      - |     232 B |
    |   ObjectMapper_PreConfigured_CopyTo_Target |     264.8 ns | 0.0305 |      - |     192 B |
     */

    [MemoryDiagnoser(true)]
    [HideColumns("Error", "StdDev", "Median")]
    //[SimpleJob(RuntimeMoniker.Net60)]
    //[SimpleJob(RuntimeMoniker.Net70)]
    public class MappingTests
    {
#if AUTOMAPPER
        private readonly IMapper _autoMapper;
#endif

        private readonly ObjectMapper _objectMapper;

        private static readonly SimpleSource SimpleSource;
        private static readonly SimpleTarget SimpleTarget;

        static MappingTests()
        {
            SimpleSource = new SimpleSource
            {
                Prop1 = 1,
                Prop2 = "Some Text",
                TimestampUtc = DateTime.UtcNow
            };

            SimpleTarget = new SimpleTarget();
        }

        public MappingTests()
        {
#if AUTOMAPPER
            var autoMapperConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<SimpleSource, SimpleTarget>();
            });

            _autoMapper = new Mapper(autoMapperConfig);
#endif

            var mapperConfiguration = new ObjectMapperConfiguration();

            mapperConfiguration.Configure<SimpleSource, SimpleTarget>();

            _objectMapper = new ObjectMapper(mapperConfiguration);
        }

#if AUTOMAPPER
        [Benchmark]     // for speed comparison
        public void AutoMapper_New_Mapper()
        {
            var mapperConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<SimpleSource, SimpleTarget>();
            });

            var autoMapper = new Mapper(mapperConfig);

            _ = _autoMapper.Map<SimpleTarget>(SimpleSource);
        }

        [Benchmark]
        public void AutoMapper_Create_Target()
        {
            _ = _autoMapper.Map<SimpleTarget>(SimpleSource);
        }

        [Benchmark]
        public void AutoMapper_CopyTo_Target()
        {
            _ = _autoMapper.Map(SimpleSource, SimpleTarget);
        }
#endif

        [Benchmark]     // for speed comparison
        public void ObjectMapper_New_Mapper_Explicit_Configure()
        {
            var mapperConfiguration = new ObjectMapperConfiguration();

            mapperConfiguration.Configure<SimpleSource, SimpleTarget>();       // not really required since it will happen implicitly

            var objectMapper = new ObjectMapper(mapperConfiguration);

            _ = objectMapper.Map(SimpleSource, SimpleTarget);
        }

        [Benchmark]     // for speed comparison
        public void ObjectMapper_New_Mapper_Implicit_Configure()
        {
            var objectMapper = new ObjectMapper();
            // not mapping => objectMapper.Configure<SimpleSource, SimpleTarget>();

            _ = objectMapper.Map(SimpleSource, SimpleTarget);
        }

        [Benchmark]
        public void ObjectMapper_PreConfigured_Create_Target()
        {
            _ = _objectMapper.Map<SimpleTarget>(SimpleSource);
        }

        [Benchmark]
        public void ObjectMapper_PreConfigured_CopyTo_Target()
        {
            _ = _objectMapper.Map(SimpleSource, SimpleTarget);
        }
    }
}
