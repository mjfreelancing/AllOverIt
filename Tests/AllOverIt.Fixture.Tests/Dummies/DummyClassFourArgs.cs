namespace AllOverIt.Fixture.Tests.Dummies
{
    internal class DummyClassFourArgs
    {
        public int Value1 { get; }
        public double Value2 { get; }
        public DummyEnum Value3 { get; }
        public float Value4 { get; }

        public DummyClassFourArgs(int value1, double value2, DummyEnum value3, float value4)
        {
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
            Value4 = value4;
        }
    }
}