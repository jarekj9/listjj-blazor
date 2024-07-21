using MassTransit;
using Listjj.Infrastructure.Events;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Listjj.Consumers
{
    public class TestEventConsumer : IConsumer<TestEvent>
    {
        private readonly ILogger<TestEventConsumer> _logger;
        public TestEventConsumer(ILogger<TestEventConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<TestEvent> context)
        {
            var eventData = context.Message.EventData;
            _logger.LogInformation($"Consumed new {nameof(TestEvent)} with Id: {context.Message.Id}");
            return Task.CompletedTask;
        }
    }
}
