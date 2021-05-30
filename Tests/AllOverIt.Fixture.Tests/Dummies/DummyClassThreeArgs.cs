namespace AllOverIt.Fixture.Tests.Dummies
{
    internal class DummyClassThreeArgs
    {
        public int Value1 { get; }
        public double Value2 { get; }
        public DummyEnum Value3 { get; }

        public DummyClassThreeArgs(int value1, double value2, DummyEnum value3)
        {
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
        }
    }
}