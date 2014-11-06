using System.IO;
using System.Net;
using System.Web.Http;
using Newtonsoft.Json;
using API.Models;

namespace API.Controllers
{
    public class LoginController : ApiController
    {
        private class JiraLoginRequest
        {
            public string username;
            public string password;
        }

        private class JiraLoginResponse
        {
            public string[] errorMessages;
        }

        // GET api/login
        public LoginResult Post([FromBody]Credentials value)
        {
            var json = new JsonSerializer();

            var request = HttpWebRequest.Create("http://fedexatlassian:8080/rest/auth/1/session");
            request.Method = "POST";
            request.ContentType = "application/json;charset=UTF-8";
            using (var writer = new JsonTextWriter(new StreamWriter(request.GetRequestStream())))
            {
                json.Serialize(writer, new JiraLoginRequest()
                    {
                        username = value.Username,
                        password = value.Password,
                    });
            }

            WebResponse response;
            var status = WebExceptionStatus.Success;
            try
            {
                response = request.GetResponse();
            }
            catch (WebException ex)
            {
                if (ex.Response == null)
                    throw;
                response = ex.Response;
                status = ex.Status;
            }
            using (var stream = response.GetResponseStream())
            {
                var result = json.Deserialize<JiraLoginResponse>(new JsonTextReader(new StreamReader(stream)));
                if (status == WebExceptionStatus.Success)
                {
                    return new LoginResult
                    {
                        Token = response.Headers["Set-Cookie"],
                        Success = true,
                    }; 
                }
                else
                {
                    return new LoginResult
                    {
                        ErrorMessage = result.errorMessages[0],
                        Success = false,
                    }; 
                }
            }
        }
    }
}
