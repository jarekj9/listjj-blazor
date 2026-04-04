using Listjj.Infrastructure.DTOs;
using Listjj.Infrastructure.Events;
using Listjj.Infrastructure.Messaging;
using Listjj.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Listjj.Consumers
{

    /*
     Example test message via Azure UI:
        {
          "id": "550e8400-e29b-41d4-a716-446655440000",
          "creationDate": "2026-04-04T10:30:00Z",
          "name": "Test Item",
          "description": "This is a test item from Azure portal",
          "value": 99.99,
          "categoryId": "62ba500e-9afe-4ff4-9302-c4c8a5be0809",
          "tags": ["test", "portal", "demo"],
          "userId": "6faa546c-2b65-4a0f-8dd4-91a5500ad527"
        }
     */
    public class AddItemEventConsumer
    {
        private readonly IMessageHandler _messageHandler;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<AddItemEventConsumer> _logger;

        public AddItemEventConsumer(
            IServiceScopeFactory serviceScopeFactory,
            IMessageHandler messageHandler,
            ILogger<AddItemEventConsumer> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _messageHandler = messageHandler;
            _logger = logger;
        }

        /// <summary>
        /// Start consuming events from the specified queue.
        /// </summary>
        /// <param name="queueName">Queue or topic name to consume from</param>
        public async Task StartAsync(string queueName)
        {
            await _messageHandler.SubscribeAsync(queueName, ProcessMessageAsync);
        }

        /// <summary>
        /// Process incoming message from the queue.
        /// Deserializes JSON to AddItemEvent and processes it.
        /// </summary>
        private async Task ProcessMessageAsync(string messageBody)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var addItemEvent = JsonSerializer.Deserialize<AddItemEvent>(messageBody, options);

                if (addItemEvent == null)
                {
                    _logger.LogWarning("Failed to deserialize message to AddItemEvent");
                    return;
                }

                _logger.LogInformation(
                    "Consumed AddItemEvent with Id: {EventId}, Name: {ItemName}, CategoryId: {CategoryId}",
                    addItemEvent.Id, addItemEvent.Name, addItemEvent.CategoryId);

                await ProcessEventAsync(addItemEvent);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize AddItemEvent from message body");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing AddItemEvent");
            }
        }

        /// <summary>
        /// Handle the actual business logic for processing the AddItemEvent.
        /// Creates a new ListItem in the database.
        /// </summary>
        private async Task ProcessEventAsync(AddItemEvent addItemEvent)
        {
            try
            {
                var request = new ListItemAddOrUpdateRequest
                {
                    Id = Guid.Empty,
                    Name = addItemEvent.Name,
                    Description = addItemEvent.Description,
                    CategoryId = addItemEvent.CategoryId,
                    Tags = addItemEvent.Tags ?? new List<string>(),
                    Active = true,
                    UserId = addItemEvent.UserId
                };

                using var scope = _serviceScopeFactory.CreateScope();
                var itemService = scope.ServiceProvider.GetRequiredService<IItemService>();
                var item = await itemService.AddOrUpdateItemAsync(request);

                _logger.LogInformation(
                    "Successfully created ListItem from event. ItemId: {ItemId}, Name: {ItemName}, CategoryId: {CategoryId}, UserId: {UserId}",
                    item.Id, item.Name, item.CategoryId, item.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process AddItemEvent: {EventId}", addItemEvent.Id);
                throw;
            }
        }
    }
}
