using System.Threading.Tasks;
using Factorial.Api.Repository;
using Factorial.Messages.Commands;
using Factorial.Messages.Events;

namespace Factorial.Api.Handlers
{
    public class FactorialCalculatedHandler : IEventHandler<FactorialCalculated>
    {
        private readonly IRepository _repository;

        public FactorialCalculatedHandler(IRepository repository)
        {
            _repository=repository;
        }

        public async Task HandleAsync(FactorialCalculated @event)
        {
            _repository.Insert(n: @event.n, result: @event.Result);
            await Task.CompletedTask;
        }
    }
}