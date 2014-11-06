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

        public bool Post(string issueId)
        {
            UpdateIssueStatus(Status.InProgress, issueId);
            return true;
        }

        private void UpdateIssueStatus(Status toStatus, string issue)
        {
            var request =
                HttpWebRequest.Create("http://fedexatlassian:8080/rest/api/2/issue/"+issue+"/transitions");
            request.Method = "POST";
            request.ContentType = "application/json;charset=UTF-8";
            request.Headers["Cookie"] = base.ControllerContext.Request.Headers.GetValues("Cookie").First();

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
        }
     
    }
}
