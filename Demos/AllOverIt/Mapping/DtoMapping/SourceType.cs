using System.Collections.Generic;

namespace DtoMapping
{
    public class SourceType
    {
        public int Prop1 { get; set; }
        public bool Prop2 { get; set; }
        public IEnumerable<string> Prop3 { get; set; }
    }
}