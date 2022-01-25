using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AllOverIt.Csv
{
    public interface IDataSerializer<TCsvData>
    {
        void AddFixedField(string headerName, Func<TCsvData, object> valueResolver);

        void AddDynamicFields<TField>(IEnumerable<TCsvData> data, Func<TCsvData, TField> fieldSelector,
            Func<TField, IEnumerable<string>> headerName, Func<TField, string, object> valueResolver);

        void AddDynamicFields<TField, THeaderId>(IEnumerable<TCsvData> data, Func<TCsvData, TField> fieldSelector,
            Func<TField, IEnumerable<(THeaderId, string)>> headerIdentifier, Func<TField, (THeaderId, string), object> valueResolver);

        Task Serialize(TextWriter writer, IEnumerable<TCsvData> data, bool includeHeader = true);
    }
}
