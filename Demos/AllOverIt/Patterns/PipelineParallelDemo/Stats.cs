namespace PipelineParallelDemo
{
    internal sealed class Stats
    {
        public double Min { get; }
        public double Max { get; }
        public double Avg { get; }

        public Stats(double min, double max, double avg)
        {
            Min = min;
            Max = max;
            Avg = avg;
        }

        public override string ToString()
        {
            return $"Min = {Min}, Max = {Max}, Avg = {Avg}";
        }
    }
}