using System.Threading.Tasks;

namespace Factorial.Messages.Commands
{
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
         Task HandleAsync(TCommand command);
    }
}