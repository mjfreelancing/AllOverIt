using AllOverIt.Helpers;

namespace SerializationFilterBenchmarking
{
    internal sealed class ComplexObjectFilter : ObjectPropertyFilter
    {
        public override bool OnIncludeProperty()
        {
            return !Path.EndsWith(".Values");
        }

        public override bool OnIncludeValue(ref string value)
        {
            // Reformat the timestamp property
            if (Path.EndsWith(".Timestamp"))
            {
                value = $"[{value}]";
            }

            return true;
        }
    }
}
