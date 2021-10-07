using AllOverIt.Helpers;

namespace SerializeObjectProperties
{
    internal sealed class ComplexObjectFilter : ObjectPropertyFilter
    {
        public override bool OnIncludeProperty()
        {
            // Excludes the array of numbers
            return !Name.EndsWith(".Values");
        }

        public override bool OnIncludeValue(ref string value)
        {
            // Reformat the timestamp property
            if (Name.EndsWith(".Timestamp"))
            {
                value = $"[{value}]";
            }

            return true;
        }
    }
}