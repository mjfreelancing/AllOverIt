﻿using ChainOfResponsibilityAsyncDemo.Handlers;

namespace ChainOfResponsibilityAsyncDemo
{
    class Program
    {
        static async Task Main()
        {
            using var cts = new CancellationTokenSource();

            var message = new QueueMessage();
            var broker = new QueueBroker();

            // handles exceptions using Chain Of Responsibility
            var exceptionHandler = new QueueBrokerExceptionHandler();

            try
            {
                Console.WriteLine("Test 1: Null payload");

                // test null payload exception handling
                broker.Send(message);
            }
            catch (Exception exception)
            {
                await exceptionHandler.HandleAsync(message, broker, exception, cts.Token);
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
                await exceptionHandler.HandleAsync(message, broker, exception, cts.Token);
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
                await exceptionHandler.HandleAsync(message, broker, exception, cts.Token);
            }

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}
