using System.Collections.Generic;

namespace AllOverIt.Csv
{
    public interface IExportFieldResolver<in TCsvData>
    {
        IEnumerable<string> HeaderNames { get; }
        IEnumerable<object> GetValues(TCsvData data);
    }
}
