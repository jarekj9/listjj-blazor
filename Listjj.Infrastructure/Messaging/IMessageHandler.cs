namespace Listjj.Infrastructure.Messaging
{
    /// <summary>
    /// Abstraction for message queue handling. Allows plugging different queue implementations
    /// (Azure Service Bus, RabbitMQ, etc.) without changing consumer code.
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Subscribe to a specific queue/topic with a handler callback
        /// </summary>
        /// <param name="queueName">Queue or topic name</param>
        /// <param name="handler">Callback function to process messages</param>
        /// <returns></returns>
        Task SubscribeAsync(string queueName, Func<string, Task> handler);

        /// <summary>
        /// Close the connection to the message handler
        /// </summary>
        Task CloseAsync();
    }
}
