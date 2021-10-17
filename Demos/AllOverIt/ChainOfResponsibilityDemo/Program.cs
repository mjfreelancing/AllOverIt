using ChainOfResponsibilityDemo.Handlers;
using System;

namespace ChainOfResponsibilityDemo
{
    class Program
    {
        static void Main()
        {
            var message = new QueueMessage();
            var broker = new QueueBroker();

            // handles exceptions using Chain Of Responsibility
            var exceptionHandler = new QueueBrokerExceptionHandler();

            try
            {
                // test null payload exception handling
                broker.Send(message);
            }
            catch (Exception exception)
            {
                exceptionHandler.Handle(message, broker, exception);
            }

            Console.WriteLine();

            try
            {
                // test empty payload exception handling
                message.Payload = string.Empty;
                broker.Send(message);
            }
            catch (Exception exception)
            {
                exceptionHandler.Handle(message, broker, exception);
            }

            Console.WriteLine();

            try
            {
                // test unhandled exception handling
                message.Payload = "A bad payload";
                broker.Send(message);
            }
            catch (Exception exception)
            {
                exceptionHandler.Handle(message, broker, exception);
            }

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}
