using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using API.Models;
using Newtonsoft.Json;

namespace API.Controllers
{
	public class HistoryController : ApiController
	{
		private class ChangeLogResponse
		{
			public Issues[] issues;
		}

		private class Issues
		{
			public string key;
			public Field fields;
			public Changelog changelog;
		}

		private class Field
		{
			public string summary;
			public Component[] components;
		}

		private class Changelog
		{
			public History[] histories;
		}

		private class Component
		{
			//public string name;
		}

		private class History
		{
			public Author author;
			public DateTime created;
			public Item[] items;
		}

		private class Author
		{
			public string name;
			public AvatarUrl avatarUrls;
		}

		private class AvatarUrl
		{
			[JsonProperty("48x48")]
			public string Large;
		}
		

		private class Item
		{
			public string field;
			public string toString;
		}


		public bool Post()
		{
			var request = CreateAuditRequest("GET");

			var json = new JsonSerializer();
			var events = new List<HistoryEvent>();
			using (var stream = request.GetResponse().GetResponseStream())
			{
				var result = json.Deserialize<ChangeLogResponse>(new JsonTextReader(new StreamReader(stream)));
				foreach (var issue in result.issues)
				{
					foreach (var history in issue.changelog.histories.Where(h => h.created > DateTime.Now.AddHours(-26)))
					{
						foreach (var item in history.items)
						{
							if (item.field == "status")
							{
								events.Add(new HistoryEvent()
								{
									Status = item.toString,
									Time = history.created,
									Title = issue.fields.summary,
									Username = history.author.name,
									AvatarUrl = history.author.avatarUrls.Large,
								   // Type = issue.fields.components[0].name,
								   
								});
							}
						}
					}
					
				}
			}

			return true;
		}

		private WebRequest CreateAuditRequest(string method)
		{
			var request =
				HttpWebRequest.Create("http://fedexatlassian:8080/rest/api/2/search?MaxResults=30&expand=changelog&jql=project%3DMWD");
			//HttpWebRequest.Create("http://fedexatlassian:8080/rest/api/2/search?MaxResults=30&fields=items&expand=changelog&jql=project%3DMWD");
			request.Method = method;
			request.ContentType = "application/json;charset=UTF-8";
			request.Headers["Cookie"] = ControllerContext.Request.Headers.GetValues("Cookie").First();
			return request;
		}
	}
}
