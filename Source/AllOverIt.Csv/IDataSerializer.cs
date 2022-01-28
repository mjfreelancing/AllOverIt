using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AllOverIt.Csv
{
    public interface IDataSerializer<TCsvData>
    {
        void AddField(string headerName, Func<TCsvData, object> valueResolver);
        void AddFields(IEnumerable<string> headerNames, Func<TCsvData, IEnumerable<object>> valuesResolver);

        Task SerializeAsync(TextWriter writer, IEnumerable<TCsvData> data, bool includeHeader = true);
    }
}
