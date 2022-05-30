using System;

namespace EnumerationDemo
{
    internal class Program
    {
        static void Main()
        {
            Console.WriteLine($"{nameof(OrderStatus.Quoted)} has a value of {OrderStatus.Quoted.Value} and a name of '{OrderStatus.Quoted.Name}'");
            Console.WriteLine($"{nameof(OrderStatus.Packed)} has a value of {OrderStatus.Packed.Value} and a name of '{OrderStatus.Packed.Name}'");
            Console.WriteLine($"{nameof(OrderStatus.Dispatched)} has a value of {OrderStatus.Dispatched.Value} and a name of '{OrderStatus.Dispatched.Name}'");
            Console.WriteLine($"{nameof(OrderStatus.Delivered)} has a value of {OrderStatus.Delivered.Value} and a name of '{OrderStatus.Delivered.Name}'");
            Console.WriteLine($"{nameof(OrderStatus.ClientAccepted)} has a value of {OrderStatus.ClientAccepted.Value} and a name of '{OrderStatus.ClientAccepted.Name}'");

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}
