using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AllOverIt.Csv
{
    public interface IDataSerializer<TCsvData>
    {
        void AddField(string headerName, Func<TCsvData, object> valueResolver);

        Task Serialize(TextWriter writer, IEnumerable<TCsvData> data, bool includeHeader = true);
    }
}
