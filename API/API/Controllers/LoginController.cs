using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class LoginController : ApiController
    {
        // GET api/values
        public LoginResult Post([FromBody]Credentials value)
        {
            return new LoginResult()
            {
                ErrorMessage = "Login Failed",
                Success = false,
            };
        }
    }
}
