using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Factorial.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
        {
            [HttpGet]
            public ActionResult<string> Get()
                => "Factorial.Service Home Controller";
        }
}
