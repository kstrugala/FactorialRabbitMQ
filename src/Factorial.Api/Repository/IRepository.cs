namespace Factorial.Api.Repository
{
    public interface IRepository
    {
         ulong? Get(int n);
         void Insert(int n, ulong result);
    }
}