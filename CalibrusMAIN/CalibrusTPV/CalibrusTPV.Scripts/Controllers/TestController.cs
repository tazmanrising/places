﻿using System.Collections.Generic;
using System.Web.Http;

namespace CalibrusTPV.Scripts.Controllers
{
    public class TestController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {

            return Ok(new List<int>() { 1, 2, 3 });
        }
    }
}
