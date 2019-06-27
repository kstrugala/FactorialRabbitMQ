using System.Threading.Tasks;
using Factorial.Messages.Commands;
using Factorial.Messages.Events;
using RawRabbit;

namespace Factorial.Service.Handlers
{
    public class CalculateFactorialHandler : ICommandHandler<CalculateFactorial>
    {
        private readonly IBusClient _client;
        private readonly IFactorialCalculator _factorialCalculator;

        public CalculateFactorialHandler(IBusClient client, IFactorialCalculator factorialCalculator)
        {
            _client = client;
            _factorialCalculator = factorialCalculator;
        }

        public async Task HandleAsync(CalculateFactorial command)
        {
            ulong result = _factorialCalculator.FactorialCalculator(command.Number);

            await _client.PublishAsync(new FactorialCalculated{
                n = command.Number,
                Result = result
            });
        }
    }
}