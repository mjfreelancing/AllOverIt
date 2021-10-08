﻿using AllOverIt.Helpers;

namespace SerializeObjectProperties
{
    internal sealed class ComplexObjectFilter : FormattableObjectPropertyFilter
    {
        public override bool OnIncludeProperty()
        {
            return !Path.EndsWith(".Values");
        }

        public override string OnFormatValue(string value)
        {
            return Path.EndsWith(".Timestamp")
                ? $"[{value}]"
                : value;
        }
    }
}