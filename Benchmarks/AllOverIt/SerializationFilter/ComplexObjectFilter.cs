﻿using AllOverIt.Formatters.Objects;

namespace SerializationFilterBenchmark
{
    internal sealed class ComplexObjectFilter : ObjectPropertyFilter, IFormattableObjectPropertyFilter
    {
        public override bool OnIncludeProperty()
        {
            return !Path!.EndsWith(".Values");
        }

        public string OnFormatValue(string value)
        {
            return Path!.EndsWith(".Timestamp")
                ? $"[{value}]"
                : value;
        }
    }
}
