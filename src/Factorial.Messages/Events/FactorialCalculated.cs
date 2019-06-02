namespace Factorial.Messages.Events
{
    public class FactorialCalculated : IEvent
    {
        public int n { get; set; }
        public int Result { get; set; }
    }
}