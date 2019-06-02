namespace Factorial.Messages.Commands
{
    public class CalculateFactorial : ICommand
    {
        public int Number { get; set; }
    }
}