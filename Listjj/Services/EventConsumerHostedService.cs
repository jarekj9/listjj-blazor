using Listjj.Consumers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Listjj.Services
{
    /// <summary>
    /// Hosted service that manages the lifecycle of the AddItemEventConsumer.
    /// Starts consuming events when the application starts.
    /// </summary>
    public class EventConsumerHostedService : IHostedService
    {
        private readonly AddItemEventConsumer _addEventConsumer;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EventConsumerHostedService> _logger;

        public EventConsumerHostedService(
            AddItemEventConsumer addEventConsumer,
            IConfiguration configuration,
            ILogger<EventConsumerHostedService> logger)
        {
            _addEventConsumer = addEventConsumer;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var queueName = _configuration["AzureServiceBus:QueueName"]
                    ?? throw new InvalidOperationException("AzureServiceBus:QueueName not configured");

                _logger.LogInformation("Starting event consumer for queue: {QueueName}", queueName);
                await _addEventConsumer.StartAsync(queueName);
                _logger.LogInformation("Event consumer started successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start event consumer");
                throw;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping event consumer");
            await Task.CompletedTask;
        }
    }
}
