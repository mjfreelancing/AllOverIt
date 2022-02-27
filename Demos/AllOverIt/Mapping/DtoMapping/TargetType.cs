using System.Collections.Generic;

namespace DtoMapping
{
    public class TargetType
    {
        public int Prop1 { get; set; }
        public IEnumerable<string> Prop3 { get; set; }
        public int Prop4 { get; private set; }      // Is private on the source
    }
}