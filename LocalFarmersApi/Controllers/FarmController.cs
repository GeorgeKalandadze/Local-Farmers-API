using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LocalFarmersApi.Controllers
{
    [Authorize]
    public class FarmController : ApiController
    {
        [HttpGet]
        public string Get()
        {
            return "Welcome to the Local Farmers API!";
        }
    }
}
