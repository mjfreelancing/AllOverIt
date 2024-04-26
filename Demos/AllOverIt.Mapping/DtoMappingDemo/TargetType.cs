using System.Collections.ObjectModel;

namespace DtoMappingDemo
{
    public class TargetType
    {
        public int Prop1 { get; set; }
        public IReadOnlyCollection<string> Prop3 { get; set; }
        public ObservableCollection<string> Prop3b { get; set; }
        public int Prop4 { get; private set; }      // Is private on the source
        public int Prop5b { get; set; }
        public int Prop6 { get; set; }
        public IReadOnlyCollection<string> Prop7 { get; set; }
        public ChildTargetType Prop8 { get; set; }
        public ChildTargetType Prop8a { get; set; }
    }
}