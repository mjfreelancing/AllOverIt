using System;

namespace AllOverIt.Csv
{
    public interface IExportFieldResolver<in TCsvData>
    {
        string HeaderName { get; }
        Func<TCsvData, object> ValueResolver { get; }
    }
}
