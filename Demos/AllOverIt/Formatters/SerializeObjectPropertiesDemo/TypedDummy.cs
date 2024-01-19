namespace SerializeObjectPropertiesDemo
{
    internal class TypedDummy<TType>
    {
        public TType Dummy { get; set; }
        public int? Value1 { get; set; }
        public int Value2 { get; set; }
    }
}