﻿namespace AllOverIt.Filtering.Filters
{
    public interface ILessThan<TProperty> : IFilterOperation
    {
        TProperty Value { get; }
    }
}