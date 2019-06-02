using System.Threading.Tasks;

namespace Factorial.Messages.Commands
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
         Task HandleAsync(TCommand command);
    }
}