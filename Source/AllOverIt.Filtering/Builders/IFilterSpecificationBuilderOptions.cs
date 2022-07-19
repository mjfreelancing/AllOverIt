using System;

namespace AllOverIt.Filtering.Builders
{
    public interface IFilterSpecificationBuilderOptions
    {
        bool UseParameterizedQueries { get; }
        StringComparison? StringComparison { get; }
    }
}