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

        private class TransitionResponse
        {
            public TransitionResponseTransition[] transitions;
        }

        private class TransitionResponseTransition
        {
            public string id;
        }

        public bool Post(string issueId, bool undo)
        {
            var status = GetStatusToUpdateTo(issueId, !undo);
            UpdateIssueStatus(status, issueId);
            return true;
        }

        private Status GetStatusToUpdateTo(string issueId, bool forward = true)
        {
            var request = CreateTransitionRequest(issueId, "GET");

            var json = new JsonSerializer();
            var transitionIds = new List<int>();
            using (var stream = request.GetResponse().GetResponseStream())
            {
                var result = json.Deserialize<TransitionResponse>(new JsonTextReader(new StreamReader(stream)));
                transitionIds.AddRange(result.transitions.Select(transition => Convert.ToInt32(transition.id)));
            }

            if (forward)
            {
                if (transitionIds.Contains((int) Status.ProgressStopped)) return Status.Done;
                return Status.InProgress;
            }
            if (transitionIds.Contains((int) Status.Done)) return Status.ProgressStopped;
            return Status.ReopenStartProgress;
            

        }

        private void UpdateIssueStatus(Status toStatus, string issueId)
        {
            var request = CreateTransitionRequest(issueId, "POST");

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

        private WebRequest CreateTransitionRequest(string issueId, string method )
        {
            var request =
                HttpWebRequest.Create("http://fedexatlassian:8080/rest/api/2/issue/" + issueId + "/transitions");
            request.Method = method;
            request.ContentType = "application/json;charset=UTF-8";
            request.Headers["Cookie"] = base.ControllerContext.Request.Headers.GetValues("Cookie").First();
            return request;
        }
    }
}
