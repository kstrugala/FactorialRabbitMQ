namespace Factorial.Api.Repository
{
    public interface IRepository
    {
         int? Get(int n);
         void Insert(int n, int result);
    }
}