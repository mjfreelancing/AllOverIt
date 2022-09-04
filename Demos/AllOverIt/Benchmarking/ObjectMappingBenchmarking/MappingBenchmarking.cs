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
    // BEFORE
    |                                        Method |       Runtime |        Mean |     Error |    StdDev | Allocated |
    |---------------------------------------------- |-------------- |------------:|----------:|----------:|----------:|
    |          AutoMapper_SimpleSource_SimpleTarget |      .NET 5.0 |    69.81 ns |  0.766 ns |  1.300 ns |      40 B |
    | ObjectMapper_SimpleSource_Create_SimpleTarget |      .NET 5.0 |   804.67 ns |  9.340 ns |  8.736 ns |     280 B |
    | ObjectMapper_SimpleSource_CopyTo_SimpleTarget |      .NET 5.0 |   728.88 ns |  8.419 ns |  7.463 ns |     240 B |
    | StaticMethod_SimpleSource_Create_SimpleTarget |      .NET 5.0 | 2,933.67 ns | 38.996 ns | 36.477 ns |   2,864 B |

    |          AutoMapper_SimpleSource_SimpleTarget |      .NET 6.0 |    72.40 ns |  1.102 ns |  0.977 ns |      40 B |
    | ObjectMapper_SimpleSource_Create_SimpleTarget |      .NET 6.0 |   617.13 ns |  7.928 ns |  7.028 ns |      88 B |
    | ObjectMapper_SimpleSource_CopyTo_SimpleTarget |      .NET 6.0 |   601.85 ns |  4.908 ns |  4.351 ns |      48 B |
    | StaticMethod_SimpleSource_Create_SimpleTarget |      .NET 6.0 | 2,567.73 ns | 26.555 ns | 23.540 ns |   2,608 B |

    |          AutoMapper_SimpleSource_SimpleTarget | .NET Core 3.1 |   109.66 ns |  0.946 ns |  0.839 ns |      40 B |
    | ObjectMapper_SimpleSource_Create_SimpleTarget | .NET Core 3.1 |   821.65 ns |  5.840 ns |  5.177 ns |     280 B |
    | ObjectMapper_SimpleSource_CopyTo_SimpleTarget | .NET Core 3.1 |   784.03 ns |  6.429 ns |  6.014 ns |     240 B |
    | StaticMethod_SimpleSource_Create_SimpleTarget | .NET Core 3.1 | 3,750.42 ns | 32.529 ns | 30.428 ns |   2,832 B |

    *******************************************************************************************************************

    // AFTER
    |                                        Method |       Runtime |        Mean |     Error |    StdDev | Allocated |
    |---------------------------------------------- |-------------- |------------:|----------:|----------:|----------:|
    |          AutoMapper_SimpleSource_SimpleTarget |      .NET 5.0 |    71.69 ns |  0.727 ns |  1.641 ns |      40 B |
    | ObjectMapper_SimpleSource_Create_SimpleTarget |      .NET 5.0 |   139.29 ns |  2.806 ns |  3.119 ns |      88 B |
    | ObjectMapper_SimpleSource_CopyTo_SimpleTarget |      .NET 5.0 |   101.99 ns |  1.964 ns |  1.837 ns |      48 B |
    | StaticMethod_SimpleSource_Create_SimpleTarget |      .NET 5.0 | 2,916.50 ns | 54.468 ns | 55.935 ns |   2,864 B |

    |          AutoMapper_SimpleSource_SimpleTarget |      .NET 6.0 |    72.09 ns |  1.428 ns |  1.467 ns |      40 B |
    | ObjectMapper_SimpleSource_Create_SimpleTarget |      .NET 6.0 |   112.80 ns |  2.212 ns |  2.173 ns |      88 B |
    | ObjectMapper_SimpleSource_CopyTo_SimpleTarget |      .NET 6.0 |   102.75 ns |  1.983 ns |  1.948 ns |      48 B |
    | StaticMethod_SimpleSource_Create_SimpleTarget |      .NET 6.0 | 2,529.94 ns | 50.301 ns | 49.402 ns |   2,608 B |

    |          AutoMapper_SimpleSource_SimpleTarget | .NET Core 3.1 |   106.59 ns |  1.692 ns |  1.583 ns |      40 B |
    | ObjectMapper_SimpleSource_Create_SimpleTarget | .NET Core 3.1 |   139.07 ns |  2.701 ns |  2.653 ns |      88 B |
    | ObjectMapper_SimpleSource_CopyTo_SimpleTarget | .NET Core 3.1 |   101.33 ns |  1.705 ns |  1.595 ns |      48 B |
    | StaticMethod_SimpleSource_Create_SimpleTarget | .NET Core 3.1 | 3,601.60 ns | 68.513 ns | 64.087 ns |   2,832 B |
    */

    [MemoryDiagnoser(false)]
    //[SimpleJob(RuntimeMoniker.NetCoreApp31)]
    //[SimpleJob(RuntimeMoniker.Net50)]
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
        [Benchmark]     // for speed comparison
        public void AutoMapper_New_Mapper()
        {
            var mapperConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<SimpleSource, SimpleTarget>();
            });

            var autoMapper = new Mapper(mapperConfig);

            _ = _autoMapper.Map<SimpleTarget>(_simpleSource);
        }
        
        [Benchmark]
        public void AutoMapper_Create_Target()
        {
            _ = _autoMapper.Map<SimpleTarget>(_simpleSource);
        }
#endif

        [Benchmark]
        public void ObjectMapper_Create_Target()
        {
            _ = _objectMapper.Map<SimpleTarget>(_simpleSource);
        }

        [Benchmark]     // for speed comparison
        public void ObjectMapper_CopyTo_Target_New_Mapper()
        {
            var objectMapper = new ObjectMapper();
            objectMapper.Configure<SimpleSource, SimpleTarget>();

            _ = objectMapper.Map(_simpleSource, _simpleTarget);
        }

        [Benchmark]
        public void ObjectMapper_CopyTo_Target()
        {
            _ = _objectMapper.Map(_simpleSource, _simpleTarget);
        }
    }
}
