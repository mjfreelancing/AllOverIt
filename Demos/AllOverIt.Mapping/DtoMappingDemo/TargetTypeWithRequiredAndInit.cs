namespace DtoMappingDemo
{
    public class TargetTypeWithRequiredAndInit
    {
        public required int Prop1 { get; set; }
        public required bool Prop2 { get; set; }
        public required IEnumerable<string> Prop3 { get; set; }
        public required IEnumerable<string> Prop3b { get; set; }
        public required int Prop4 { get; set; }     // Is private on the source
        public required int Prop5a { get; set; }
        public required IEnumerable<string> Prop7 { get; set; }
        public required ChildSourceType Prop8 { get; set; }
        public required ChildSourceType Prop9 { get; set; }
    }
}