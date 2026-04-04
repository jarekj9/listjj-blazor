using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Listjj.Infrastructure.Services.Credentials;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Listjj.Infrastructure.Messaging
{
    /// <summary>
    /// Azure Service Bus implementation of IMessageHandler.
    /// Authenticates using ICredentialProvider for credential management.
    /// Automatically creates queues if they don't exist.
    /// </summary>
    public class AzureServiceBusMessageHandler : IMessageHandler, IAsyncDisposable
    {
        private readonly string _serviceBusNamespace;
        private readonly ICredentialProvider _credentialProvider;
        private readonly ILogger<AzureServiceBusMessageHandler> _logger;
        private ServiceBusClient _serviceBusClient;
        private ServiceBusAdministrationClient _administrationClient;

        public AzureServiceBusMessageHandler(
            IConfiguration configuration,
            ICredentialProvider credentialProvider,
            ILogger<AzureServiceBusMessageHandler> logger)
        {
            _logger = logger;
            _credentialProvider = credentialProvider;

            // Read Azure Service Bus settings
            var azureConfig = configuration.GetSection("AzureServiceBus");
            _serviceBusNamespace = azureConfig["Namespace"] ?? throw new InvalidOperationException("AzureServiceBus:Namespace not configured");

            InitializeClients();
        }

        private void InitializeClients()
        {
            try
            {
                var credential = _credentialProvider.GetCredential();
                _serviceBusClient = new ServiceBusClient(_serviceBusNamespace, credential);
                _administrationClient = new ServiceBusAdministrationClient(_serviceBusNamespace, credential);
                _logger.LogInformation("Azure Service Bus clients initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Azure Service Bus clients");
                throw;
            }
        }

        /// <summary>
        /// Ensure the queue exists, create it if not.
        /// </summary>
        private async Task EnsureQueueExistsAsync(string queueName)
        {
            try
            {
                var queueExists = await _administrationClient.QueueExistsAsync(queueName);
                
                if (!queueExists)
                {
                    _logger.LogInformation("Queue '{QueueName}' does not exist. Creating...", queueName);
                    var options = new CreateQueueOptions(queueName)
                    {
                        DefaultMessageTimeToLive = TimeSpan.FromDays(14),
                        LockDuration = TimeSpan.FromMinutes(1),
                        MaxDeliveryCount = 10,
                        DeadLetteringOnMessageExpiration = true
                    };
                    
                    await _administrationClient.CreateQueueAsync(options);
                    _logger.LogInformation("Queue '{QueueName}' created successfully", queueName);
                }
                else
                {
                    _logger.LogInformation("Queue '{QueueName}' already exists", queueName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to ensure queue '{QueueName}' exists", queueName);
                throw;
            }
        }

        public async Task SubscribeAsync(string queueName, Func<string, Task> handler)
        {
            try
            {
                _logger.LogInformation("Subscribing to queue: {QueueName}", queueName);

                await EnsureQueueExistsAsync(queueName);

                var processor = _serviceBusClient.CreateProcessor(queueName, new ServiceBusProcessorOptions
                {
                    AutoCompleteMessages = false,
                    MaxConcurrentCalls = 1
                });

                processor.ProcessMessageAsync += async args =>
                {
                    try
                    {
                        var messageBody = args.Message.Body.ToString();
                        await handler(messageBody);
                        await args.CompleteMessageAsync(args.Message);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing message from queue: {QueueName}", queueName);
                        await args.AbandonMessageAsync(args.Message);
                    }
                };

                processor.ProcessErrorAsync += args =>
                {
                    _logger.LogError(args.Exception, "Error in Service Bus processor for queue: {QueueName}", queueName);
                    return Task.CompletedTask;
                };

                await processor.StartProcessingAsync();
                _logger.LogInformation("Successfully subscribed to queue: {QueueName}", queueName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to subscribe to queue: {QueueName}", queueName);
                throw;
            }
        }

        public async Task CloseAsync()
        {
            if (_serviceBusClient != null)
            {
                await _serviceBusClient.DisposeAsync();
            }
        }

        public async ValueTask DisposeAsync()
        {
            await CloseAsync();
        }
    }
}
