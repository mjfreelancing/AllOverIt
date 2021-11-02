using AllOverIt.Assertion;
using System;

namespace ChainOfResponsibilityDemo
{
    public sealed class QueueBroker
    {
        public void Send(QueueMessage message)
        {
            // emulate various exceptions

            // throws ThrowArgumentNullException when null and ArgumentException when whitespace
            // and will be processed by NullMessageExceptionHandler / EmptyMessageExceptionHandler
            _ = message.Payload.WhenNotNullOrEmpty(nameof(message));

            // this will be processed by UnhandledExceptionHandler
            throw new InvalidOperationException();
        }

        public void Abandon()
        {
            Console.WriteLine("Abandon message");
        }

        public void Deadletter()
        {
            Console.WriteLine("Deadletter message");
        }
    }
}