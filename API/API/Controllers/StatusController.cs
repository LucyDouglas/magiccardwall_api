using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.SessionState;
using API.Models;
using Newtonsoft.Json;

namespace API.Controllers
{
    public class StatusController : ApiController
    {
        private class TransitionRequest
        {
            public TransitionRequestTransition transition;
        }

        private class TransitionRequestTransition
        {
            public string id;
        }

        public bool Post()
        {
            UpdateIssueStatus(Status.InProgress, "mwd-5");
            return true;
        }

        private void UpdateIssueStatus(Status toStatus, string issue)
        {
            var request =
                HttpWebRequest.Create("http://fedexatlassian:8080/rest/api/2/issue/"+issue+"/transitions");
            request.Method = "POST";
            request.ContentType = "application/json;charset=UTF-8";

            var json = new JsonSerializer();
            using (var writer = new JsonTextWriter(new StreamWriter(request.GetRequestStream())))
            {
                json.Serialize(writer, new TransitionRequest()
                    {
                        transition = new TransitionRequestTransition()
                        {
                            id = ((int)toStatus).ToString()
                        }
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
            //using (var stream = response.GetResponseStream())
            //{
            //    var result = json.Deserialize<JiraLoginResponse>(new JsonTextReader(new StreamReader(stream)));
            //    if (status == WebExceptionStatus.Success)
            //    {
            //        return new LoginResult()
            //        {
            //            Token = result.session.value,
            //            Success = true,
            //        };
            //    }
            //    else
            //    {
            //        return new LoginResult()
            //        {
            //            ErrorMessage = result.errorMessages[0],
            //            Success = false,
            //        };
            //    }
            //}

        }
        
    }
}
