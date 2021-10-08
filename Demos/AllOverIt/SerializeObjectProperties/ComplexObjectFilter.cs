﻿using AllOverIt.Helpers;

namespace SerializeObjectProperties
{
    internal sealed class ComplexObjectFilter : ObjectPropertyFilter
    {
        public override bool OnIncludeProperty()
        {
            var t = this;

            // Excludes the array of numbers
            return !Path.EndsWith(".Values");
        }

        public override bool OnIncludeValue(ref string value)
        {
            // Reformat the timestamp property
            if (Path.EndsWith(".Timestamp"))
            {
                var name = Path;


                value = $"[{value}]";
            }

            return true;
        }
    }
}