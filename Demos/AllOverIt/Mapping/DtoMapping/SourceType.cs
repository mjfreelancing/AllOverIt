using System.Collections.Generic;

namespace DtoMapping
{
    public class SourceType
    {
        private int Prop4 { get; }

        public int Prop1 { get; set; }
        public bool Prop2 { get; set; }
        public IEnumerable<string> Prop3 { get; set; }

        public SourceType()
        {
            Prop4 = 0;
        }

        public SourceType(int prop4)
        {
            Prop4 = prop4;
        }
    }
}