using System.Collections.Generic;

namespace Factorial.Api.Repository
{
    public class InMemoryRepository : IRepository
    {
        private readonly Dictionary<int, int> _results = new Dictionary<int, int>();

        public int? Get(int n)
        {
            int result;
            if(_results.TryGetValue(n, out result))
            {
                return result;
            }
            return null;
        }

        public void Insert(int n, int result)
        {
            _results[n] = result;
        }
    }
}