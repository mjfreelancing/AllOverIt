using ChainOfResponsibilityDemo.Handlers;

namespace ChainOfResponsibilityDemo
{
    class Program
    {
        static void Main()
        {
            var message = new QueueMessage();
            var broker = new QueueBroker();

            // Handle exceptions using Chain Of Responsibility

            // Method 1 : Using QueueBrokerExceptionHandler1 implemented as a ChainOfResponsibilityComposer<QueueMessageHandlerState, QueueMessageHandlerState>
            // var exceptionHandler = new QueueBrokerExceptionHandler1();

            // Method 2 : Using QueueBrokerExceptionHandler2 implemented in terms of chained Then() calls
            var exceptionHandler = new QueueBrokerExceptionHandler2();

            try
            {
                Console.WriteLine("Test 1: Null payload");

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
                Console.WriteLine("Test 2: Empty String Payload");

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
                Console.WriteLine("Test 3: Unhandled exception");

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
