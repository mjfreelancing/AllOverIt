using System.Collections.Generic;

namespace DtoMapping
{
    public class SourceType
    {
        //public int Prop1 { get; set; }
        //public bool Prop2 { get; set; }
        public IEnumerable<string> Prop3 { get; set; }
        //public IEnumerable<string> Prop3b { get; set; }
        //private int Prop4 { get; }      // Is public on the target, but has a private setter
        //public int Prop5a { get; set; }
        //public IEnumerable<string> Prop7 { get; set; }
        public ChildSourceType Prop8 { get; set; } = new();
        public ChildSourceType Prop9 { get; set; } = new();

        public SourceType(int prop4)
        {
            //Prop4 = prop4;
        }

        //public int GetProp4()
        //{
        //    return Prop4;
        //}
    }
}