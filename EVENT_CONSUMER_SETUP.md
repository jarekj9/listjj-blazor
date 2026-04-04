# Azure Service Bus Event Consumer Setup Guide

This document explains how to configure and use the event consumer infrastructure for Azure Service Bus.

## Architecture

The solution uses a **layered separation of concerns** approach:

1. **`IMessageHandler`** - Abstract interface for message queue handling
2. **`AzureServiceBusMessageHandler`** - Concrete implementation using Azure Service Bus SDK
3. **`AddItemEventConsumer`** - Business logic for consuming `AddItemEvent` events
4. **`EventConsumerHostedService`** - Lifecycle management of the consumer

This design allows you to easily swap implementations (e.g., replace Azure Service Bus with RabbitMQ) without changing consumer code.

## Configuration

### 1. appsettings.json

Add the following configuration sections to your `appsettings.json`:

```json
{
  "AzureServiceBus": {
    "Namespace": "your-namespace.servicebus.windows.net",
    "QueueName": "add-item-events"
  },
  "ServicePrincipal": {
    "TenantId": "6ffa26d2-2615-461f-9068-458f373c2bb9",
    "ClientId": "002902aa-eaaf-4529-9a05-b323b2150711",
    "ClientSecret": "your-secret-value"
  }
}
```

### 2. Azure Service Bus Setup

- Create an Azure Service Bus namespace
- Create a queue named `add-item-events` (or use your preferred name in config)

### 3. Service Principal Configuration

Since you're running on-premises and already have an app registration for Microsoft login, you'll need to add a **Client Secret** to that same app registration:

1. Go to Azure Portal → App Registrations → Select your app (ID: `002902aa-eaaf-4529-9a05-b323b2150711`)
2. Navigate to **Certificates & secrets**
3. Click **New client secret**
4. Create a secret and copy the value to `ServicePrincipal:ClientSecret`

### 4. Azure RBAC Permissions

The service principal needs these permissions on the Service Bus namespace:

- **Azure Service Bus Data Owner** - for consuming and managing messages
- OR **Azure Service Bus Data Receiver** - minimal permissions for receiving only

Assign the role at the Service Bus namespace level.

## Usage

### Starting the Consumer

The consumer automatically starts when the application boots via `EventConsumerHostedService`. No manual intervention needed.

### Processing Events

When messages arrive in the queue:

1. `EventConsumerHostedService` calls `AddItemEventConsumer.StartAsync(queueName)`
2. `AddItemEventConsumer` subscribes to the queue using `IMessageHandler`
3. Messages are received and deserialized to `AddItemEvent`
4. `ProcessEventAsync` handles the business logic

### Integrating with Your Repository

In `AddItemEventConsumer.ProcessEventAsync()`, inject `IUnitOfWork` and create the `ListItem`:

```csharp
public class AddItemEventConsumer
{
    private readonly IUnitOfWork _unitOfWork;

    public AddItemEventConsumer(IMessageHandler messageHandler, IUnitOfWork unitOfWork, ILogger<AddItemEventConsumer> logger)
    {
        _unitOfWork = unitOfWork;
        // ...
    }

    private async Task ProcessEventAsync(AddItemEvent addItemEvent)
    {
        var newItem = new ListItem
        {
            Name = addItemEvent.Name,
            Description = addItemEvent.Description,
            Value = addItemEvent.Value,
            CategoryId = addItemEvent.CategoryId,
            Tags = string.Join(",", addItemEvent.Tags),
            Active = true,
            Created = DateTime.UtcNow,
            Modified = DateTime.UtcNow
        };

        await _unitOfWork.ListItems.Add(newItem);
        await _unitOfWork.SaveChangesAsync();
    }
}
```

## Adding Alternative Queue Implementations

To add RabbitMQ support, create a new implementation of `IMessageHandler`:

```csharp
public class RabbitMqMessageHandler : IMessageHandler
{
    public async Task SubscribeAsync(string queueName, Func<string, Task> handler)
    {
        // RabbitMQ implementation
    }

    public async Task CloseAsync()
    {
        // Cleanup
    }
}
```

Then register it in `Program.cs`:

```csharp
// For RabbitMQ
builder.Services.AddSingleton<IMessageHandler, RabbitMqMessageHandler>();

// Or use conditional registration based on configuration
var messagingType = builder.Configuration["Messaging:Type"]; // "AzureServiceBus" or "RabbitMq"
if (messagingType == "RabbitMq")
{
    builder.Services.AddSingleton<IMessageHandler, RabbitMqMessageHandler>();
}
else
{
    builder.Services.AddSingleton<IMessageHandler, AzureServiceBusMessageHandler>();
}
```

## NuGet Dependencies

The Azure Service Bus implementation requires:

```
Azure.Identity
Azure.Messaging.ServiceBus
```

These should already be available in your .NET 10 project. If not, install them via:

```
dotnet add package Azure.Identity
dotnet add package Azure.Messaging.ServiceBus
```

## Troubleshooting

### Connection Issues

- Verify service principal has correct permissions on Service Bus namespace
- Check that `ServiceBus:Namespace`, `ServicePrincipal:*` configurations are correct
- Ensure the service principal secret hasn't expired

### Messages Not Being Processed

- Check application logs in `EventConsumerHostedService`
- Verify queue name matches between configuration and Azure Portal
- Ensure messages are being published to the correct queue

### Security

- Store `ServicePrincipal:ClientSecret` in Azure Key Vault or secure configuration, **not** in source control
- Use environment variables or user secrets in development:
  ```
  dotnet user-secrets set "ServicePrincipal:ClientSecret" "your-secret"
  ```
