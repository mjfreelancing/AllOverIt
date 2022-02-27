using System.Collections.Generic;

namespace DtoMapping
{
    public class SourceType
    {
        public int Prop1 { get; set; }
        public bool Prop2 { get; set; }
        public IEnumerable<string> Prop3 { get; set; }
        private int Prop4 { get; }      // Is public on the target, but has a private setter

        public SourceType(int prop4)
        {
            Prop4 = prop4;
        }

        public int GetProp4()
        {
            return Prop4;
        }
    }
}