using AllOverIt.Mapping;
using AllOverIt.Mapping.Extensions;
using AutoMapper;
using BenchmarkDotNet.Attributes;
using System;

namespace ObjectMappingBenchmarking
{
    public class SimpleSource
    {
        public int Prop1 { get; set; }
        public string Prop2 { get; set; }
        public DateTime TimestampUtc { get; set; }
    }

    public class SimpleTarget
    {
        public int Prop1 { get; set; }
        public string Prop2 { get; set; }
        public DateTime TimestampUtc { get; set; }
    }


    [MemoryDiagnoser]
    public class MappingTests
    {
        private const int _loopCount = 10;

        // AutoMapper
        private readonly IMapper _autoMapper;

        // ObjectMapper
        private readonly IObjectMapper _objectMapper;

        private readonly SimpleSource _simpleSource;
        private readonly SimpleTarget _simpleTarget;

        public MappingTests()
        {
            // AutoMapper
            var autoMapperConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<SimpleSource, SimpleTarget>();
            });

            _autoMapper = new Mapper(autoMapperConfig);

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

        [Benchmark]
        public void AutoMapper_SimpleSource_SimpleTarget()
        {
            for (var i = 0; i < _loopCount; i++)
            {
                _ = _autoMapper.Map<SimpleTarget>(_simpleSource);
            }
        }

        [Benchmark]
        public void StaticMethod_SimpleSource_Create_SimpleTarget()
        {
            for (var i = 0; i < _loopCount; i++)
            {
                _ = _simpleSource.MapTo<SimpleTarget>();
            }
        }

        [Benchmark]
        public void ObjectMapper_SimpleSource_Create_SimpleTarget()
        {
            for (var i = 0; i < _loopCount; i++)
            {
                _ = _objectMapper.Map<SimpleTarget>(_simpleSource);
            }
        }

        [Benchmark]
        public void ObjectMapper_SimpleSource_CopyTo_SimpleTarget()
        {
            for (var i = 0; i < _loopCount; i++)
            {
                _ = _objectMapper.Map(_simpleSource, _simpleTarget);
            }
        }
    }
}
