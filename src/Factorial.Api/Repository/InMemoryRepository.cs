using System.Collections.Generic;

namespace Factorial.Api.Repository
{
    public class InMemoryRepository : IRepository
    {
        private readonly Dictionary<int, ulong> _results = new Dictionary<int, ulong>();

        public ulong? Get(int n)
        {
            ulong result;
            if(_results.TryGetValue(n, out result))
            {
                return result;
            }
            return null;
        }

        public void Insert(int n, ulong result)
        {
            _results[n] = result;
        }
    }
}