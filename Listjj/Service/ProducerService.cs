using MassTransit;
using Listjj.Abstract;
using Listjj.Infrastructure.Events;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace Listjj.Service
{
    public class ProducerService : IProducerService
    {
        private readonly ISendEndpointProvider _endpointProvider;
        private readonly ILogger<ProducerService> _logger;

        public ProducerService(ISendEndpointProvider endpointProvider, ILogger<ProducerService> logger)
        {
            _endpointProvider = endpointProvider;
            _logger = logger;
        }

        public async Task SendToTenant<TEvent>(TEvent @event, string tenantCode) where TEvent : BaseEvent
        {
            var eventName = @event.GetType().Name;
            // queue Created in RabbitMQ will be: {tenantCode}_{eventName}, not "queue:{tenantCode}_{eventName}" !!!
            var endpointUri = new Uri($"queue:{tenantCode}_{eventName}"); 
            var endpoint = await _endpointProvider.GetSendEndpoint(endpointUri);
            _logger.LogInformation($"Sending {eventName} with id: {@event.Id} to endpoint: {endpointUri}");
            await endpoint.Send(@event);
        }
    }
}
