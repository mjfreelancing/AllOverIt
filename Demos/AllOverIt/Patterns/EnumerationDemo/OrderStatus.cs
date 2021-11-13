using AllOverIt.Patterns.Enumeration;
using System.Runtime.CompilerServices;

namespace EnumerationDemo
{
    internal sealed class OrderStatus : EnrichedEnum<OrderStatus>
    {
        public static OrderStatus Quoted = new(0);                                  // Defaults to Quoted
        public static OrderStatus Packed = new(1);                                  // Defaults to Packed
        public static OrderStatus Dispatched = new(2);                              // Defaults to Dispatched
        public static OrderStatus Delivered = new(3);                               // Defaults to Delivered
        public static OrderStatus ClientAccepted = new(4, "Client_Accepted");       // A custom name

        public OrderStatus(int value, [CallerMemberName] string name = null)
            : base(value, name)
        {
        }
    }
}
