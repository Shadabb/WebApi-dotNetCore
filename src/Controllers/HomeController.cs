using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreCodeCamp.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [TypeFilter(typeof(FeatureSwitchAuthMiddleware))]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public ActionResult GetAll()
        {
            return Ok();
        }
    }
}