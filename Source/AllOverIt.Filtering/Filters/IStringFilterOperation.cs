﻿namespace AllOverIt.Filtering.Filters
{
    /// <summary>Represents a basic filter operation for a string value. Basic operations include those
    /// that can compare one value to another.</summary>
    public interface IStringFilterOperation
    {
        string Value { get; }
    }
}