using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FC.Framework;
using DotPay.QueryService;

namespace DotPay.Web.Controllers
{
    public class FCAPIController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        
        public class Depth
        {
            public bool result { get; set; }
            public IEnumerable<decimal[]> asks { get; set; }
            public IEnumerable<decimal[]> bids { get; set; }
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }

     
    }
}