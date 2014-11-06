using System.Web.Http;
using API.Models;

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
