using System;

namespace AllOverIt.Csv
{
    public interface IExportFieldResolver<in TExportData>
    {
        public string HeaderName { get; }
        public Func<TExportData, object> ValueResolver { get; }
    }
}
