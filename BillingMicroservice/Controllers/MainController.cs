using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillingMicroservice.Services;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BillingMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        // GET compute data
        [HttpGet("compute")]
        public ActionResult<string> GetComputeData([FromServices]  IApiService api)
        {
            return Ok(api.GetBillingComputeData());
        }
    }

}
