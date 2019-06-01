using System.Threading.Tasks;

namespace Factorial.Messages.Events
{
    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
         Task HandleAsync(TEvent @event);
    }
}