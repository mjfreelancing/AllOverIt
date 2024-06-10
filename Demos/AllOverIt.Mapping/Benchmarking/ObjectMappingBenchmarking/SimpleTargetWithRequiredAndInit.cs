namespace ObjectMappingBenchmarking
{
    public class SimpleTargetWithRequiredAndInit
    {
        public required int Prop1 { get; init; }
        public required string Prop2 { get; init; }
        public required DateTime TimestampUtc { get; init; }
    }
}