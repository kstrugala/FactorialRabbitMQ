using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Factorial.Api.Repository;
using Factorial.Messages.Commands;
using Microsoft.AspNetCore.Mvc;
using RawRabbit;

namespace Factorial.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FactorialController : ControllerBase
    {
        private readonly IBusClient _client;
        private readonly IRepository _repository;
        
        public FactorialController(IBusClient client, IRepository repository)
        {
            _client=client;
            _repository=repository;
        }

        [HttpGet("{n}")]
        public IActionResult Get(int n)
        {
            ulong? result = _repository.Get(n);

            if(result.HasValue)
            {
                return Content(result.ToString());
            }

            return Content("Not ready yet...");
        }

        [HttpPost("{n}")]
        public async Task<IActionResult> Post(int n)
        {
            ulong? result = _repository.Get(n);

            if(!result.HasValue)
            {
                await _client.PublishAsync(new CalculateFactorial{
                    Number = n
                });
            }

            return Accepted($"factorial/{n}", null);            
        }

    }
}
