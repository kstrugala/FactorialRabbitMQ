namespace Factorial.Service
{
    public class Factorial : IFactorialCalculator
    {
        public ulong FactorialCalculator(int n)
        {
            if(n==0) 
                return 1;
            else
                return (ulong)n*FactorialCalculator(n-1);
        }
    }
}