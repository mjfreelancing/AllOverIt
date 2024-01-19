using System.Collections.Generic;

namespace DtoMappingDemo
{
    public class ChildSourceType
    {
        public int Prop1 { get; set; }
        public IEnumerable<ChildChildSourceType> Prop2a { get; set; }
        public IEnumerable<ChildChildSourceType> Prop2b { get; set; }
    }
}