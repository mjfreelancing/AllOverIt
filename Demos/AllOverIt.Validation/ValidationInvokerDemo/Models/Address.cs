namespace ValidationInvokerDemo.Models
{
    public sealed class Address
    {
        public int Number { get; set; }
        public string Street { get; set; } = string.Empty;
        public string Suburb { get; set; } = string.Empty;
        public int Postcode { get; set; }
    }
}