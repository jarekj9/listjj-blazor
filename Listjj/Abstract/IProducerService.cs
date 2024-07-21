using System.Threading.Tasks;
using Listjj.Infrastructure.Events;

namespace Listjj.Abstract
{ 
    public interface IProducerService
    {
        Task SendToTenant<TEvent>(TEvent @event, string tenantCode) where TEvent : BaseEvent;
    }

}