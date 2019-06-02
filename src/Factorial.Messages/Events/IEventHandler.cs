using System.Threading.Tasks;

namespace Factorial.Messages.Events
{
    public interface IEventHandler<in TEvent> where TEvent : IEvent
    {
         Task HandleAsync(TEvent @event);
    }
}