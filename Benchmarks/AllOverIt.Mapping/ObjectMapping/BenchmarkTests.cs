using AllOverIt.Mapping;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace ObjectMappingBenchmark
{
    /*
    | Method                                       | Mean     | Gen0   | Allocated |
    |--------------------------------------------- |---------:|-------:|----------:|
    | Existing_Mapper_No_Configure_New_Target      | 74.77 ns | 0.0204 |     256 B |
    | Existing_Mapper_No_Configure_Existing_Target | 60.51 ns | 0.0120 |     152 B |
    | Existing_Mapper_Configured_New_Target        | 71.11 ns | 0.0204 |     256 B |
    | Existing_Mapper_Configured_Existing_Target   | 59.72 ns | 0.0121 |     152 B |
     */

    [MemoryDiagnoser(true)]
    [HideColumns("Error", "StdDev", "Median")]
    [SimpleJob(RuntimeMoniker.Net80)]
    public class MappingTests
    {
        private static readonly ObjectMapper Mapper1 = new();
        private static readonly ObjectMapper Mapper2;
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

            var mapperConfiguration = new ObjectMapperConfiguration(options =>
            {
                // The Register() methods are optional.
                // * May help with the time it takes to create the first instance.
                // * Provides a way to set default properties not on the source.
                // * Provides a way of constructing via DI.

                // Examples:
                // options.Register<SimpleTarget>(() => new SimpleTarget());
                // options.Register<SimpleTargetWithRequiredAndInit>(() => new { Prop1 = 10 });
            });

            mapperConfiguration.Configure<SimpleSource, SimpleTarget>();
            mapperConfiguration.Configure<SimpleSource, SimpleTargetWithRequiredAndInit>();

            Mapper2 = new ObjectMapper(mapperConfiguration);
        }

        // ==

        [Benchmark]
        public void Existing_Mapper_No_Configure_New_Target()
        {
            _ = Mapper1.Map<SimpleTarget>(SimpleSource);
        }

        [Benchmark]
        public void Existing_Mapper_No_Configure_Existing_Target()
        {
            _ = Mapper1.Map(SimpleSource, SimpleTarget);
        }

        // ==

        [Benchmark]
        public void Existing_Mapper_Configured_New_Target()
        {
            _ = Mapper2.Map<SimpleTarget>(SimpleSource);
        }

        [Benchmark]
        public void Existing_Mapper_Configured_Existing_Target()
        {
            _ = Mapper2.Map(SimpleSource, SimpleTarget);
        }
    }
}
