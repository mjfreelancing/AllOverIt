using AllOverIt.Helpers;

namespace SerializeObjectProperties
{
    internal sealed class ComplexObjectFilter : ObjectPropertyFilter
    {
        public override bool OnIncludeProperty()
        {
            return !Name.EndsWith(".Points");
        }

        public override bool OnIncludeValue(ref string value)
        {
            return Name != "Points";
        }
    }
}