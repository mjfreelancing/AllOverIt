﻿using System;
using System.Collections.Generic;

namespace AllOverIt.Helpers
{
    public abstract class ObjectPropertyFilter
    {
        public Type Type { get; internal set; }
        public string Path { get; internal set; }
        public IReadOnlyCollection<object> Parents { get; internal set; }

        public virtual bool OnIncludeProperty()
        {
            return true;
        }

        public virtual bool OnIncludeValue(ref string value)
        {
            return true;
        }

        //public virtual ref string OnFormatValue(ref string value)
        //{
        //    value = "";

        //    return ref value;
        //}
    }
}