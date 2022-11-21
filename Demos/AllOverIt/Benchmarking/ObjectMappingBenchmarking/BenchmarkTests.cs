#define AUTOMAPPER
using AllOverIt.Mapping;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System;

#if AUTOMAPPER
    using AutoMapper;
#endif

namespace ObjectMappingBenchmarking
{
    /*
    |                                     Method |          Mean |         Error |        StdDev |        Median | Allocated |
    |------------------------------------------- |--------------:|--------------:|--------------:|--------------:|----------:|
    |                      AutoMapper_New_Mapper | 107,214.25 ns |  2,129.132 ns |  4,934.587 ns | 105,570.74 ns |  43,366 B |
    | ObjectMapper_New_Mapper_Explicit_Configure | 669,252.49 ns | 12,229.540 ns | 12,558.837 ns | 671,961.04 ns |  33,114 B |
    | ObjectMapper_New_Mapper_Implicit_Configure | 668,595.91 ns | 13,110.411 ns | 15,097.968 ns | 669,269.04 ns |  33,114 B |
    |                   AutoMapper_Create_Target |      74.85 ns |      1.466 ns |      1.630 ns |      75.19 ns |      40 B |
    |                 ObjectMapper_Create_Target |     241.69 ns |      4.485 ns |      4.195 ns |     242.39 ns |      88 B |
    |                 ObjectMapper_CopyTo_Target |     238.05 ns |      4.116 ns |      3.850 ns |     237.60 ns |      48 B |


    |                                     Method |          Mean |         Error |        StdDev |        Median | Allocated |
    |------------------------------------------- |--------------:|--------------:|--------------:|--------------:|----------:|
    |                      AutoMapper_New_Mapper | 127,781.94 ns |  9,108.572 ns | 26,856.830 ns | 110,726.37 ns |  43,366 B |
    |                   AutoMapper_Create_Target |      77.25 ns |      1.538 ns |      1.646 ns |      76.69 ns |      40 B |
    |                   AutoMapper_CopyTo_Target |      73.76 ns |      1.405 ns |      1.246 ns |      73.16 ns |         - |
    | ObjectMapper_New_Mapper_Explicit_Configure | 677,466.63 ns | 11,923.769 ns | 11,153.501 ns | 678,157.76 ns |  33,170 B |
    | ObjectMapper_New_Mapper_Implicit_Configure | 678,204.01 ns | 12,919.114 ns | 13,823.307 ns | 674,865.33 ns |  33,170 B |
    |   ObjectMapper_PreConfigured_Create_Target |     316.85 ns |      5.596 ns |      4.369 ns |     317.21 ns |     128 B |
    |   ObjectMapper_PreConfigured_CopyTo_Target |     300.11 ns |      5.766 ns |      4.815 ns |     299.16 ns |      88 B |    
     */

    [MemoryDiagnoser(true)]
    [HideColumns("Error", "StdDev", "Median")]
    //[SimpleJob(RuntimeMoniker.NetCoreApp31)]
    //[SimpleJob(RuntimeMoniker.Net50)]
    [SimpleJob(RuntimeMoniker.Net60)]
    public class MappingTests
    {
#if AUTOMAPPER
        private readonly IMapper _autoMapper;
#endif

        private readonly IObjectMapper _objectMapper;

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
