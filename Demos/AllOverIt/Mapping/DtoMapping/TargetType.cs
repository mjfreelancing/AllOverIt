using System.Collections.Generic;

namespace DtoMapping
{
    public class TargetType
    {
        public int Prop1 { get; set; }
        public IReadOnlyCollection<string> Prop3 { get; set; }
        public int Prop4 { get; private set; }      // Is private on the source
        public int Prop5b { get; set; }
        public int Prop6 { get; set; }
        public IReadOnlyCollection<string> Prop7 { get; set; }
    }
}