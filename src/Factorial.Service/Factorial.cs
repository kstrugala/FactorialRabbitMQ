namespace Factorial.Service
{
    public class Factorial : IFactorialCalculator
    {
        public int FactorialCalculator(int n)
        {
            if(n==0) 
                return 1;
            else
                return n*FactorialCalculator(n-1);
        }
    }
}