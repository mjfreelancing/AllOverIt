namespace DtoMappingDemo
{
    public class ChildTargetType
    {
        public int Prop1 { get; set; }
        public IEnumerable<ChildChildSourceType>? Prop2a { get; set; }
        public ChildChildTargetType[] Prop2b { get; set; } = [];
    }
}