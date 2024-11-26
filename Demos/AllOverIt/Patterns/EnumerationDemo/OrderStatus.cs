using AllOverIt.Patterns.Enumeration;
using System.Runtime.CompilerServices;

namespace EnumerationDemo
{
    internal sealed class OrderStatus : EnrichedEnum<OrderStatus>
    {
        public static readonly OrderStatus Quoted = new(0);                                  // Defaults to Quoted
        public static readonly OrderStatus Packed = new(1);                                  // Defaults to Packed
        public static readonly OrderStatus Dispatched = new(2);                              // Defaults to Dispatched
        public static readonly OrderStatus Delivered = new(3);                               // Defaults to Delivered
        public static readonly OrderStatus ClientAccepted = new(4, "Client_Accepted");       // A custom name

        public OrderStatus(int value, [CallerMemberName] string name = "")
            : base(value, name)
        {
        }
    }
}
