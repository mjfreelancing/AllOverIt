﻿namespace AllOverIt.Filtering.Filters
{
    public interface IStartsWith : IStringFilterOperation
    {
        string Value { get; }
    }
}