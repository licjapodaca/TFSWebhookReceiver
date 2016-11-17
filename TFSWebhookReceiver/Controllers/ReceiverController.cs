using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using TFSWebhookReceiver.Models;

namespace TFSWebhookReceiver.Controllers
{
	public class ReceiverController : ApiController
	{
		static HttpClient client = new HttpClient();
		private string
			urlWebhook = WebConfigurationManager.AppSettings["DevsTeam.webhook"].ToString(),
			logoUrl = WebConfigurationManager.AppSettings["urlLogoEncoded"].ToString(),
			title = "<span style='font-size:1.2em !important;color:#4C0569;'><b>{0}</b></span>";

		[Route("api/Receiver/BuildCompleteEvent")]
		[HttpPost]
		public async Task<IHttpActionResult> BuildCompleteEvent(BuildCompleteEvent parameters)
		{
			try
			{
				parameters.resource.queueTime = parameters.resource.queueTime.Kind == DateTimeKind.Unspecified ? DateTime.Now : parameters.resource.queueTime;
				parameters.resource.buildNumberRevision = parameters.resource.buildNumberRevision == 0 ? 1 : parameters.resource.buildNumberRevision;

				parameters.resourceVersion = parameters.resourceVersion == null ? "Test" : parameters.resourceVersion;
				parameters.resource.result = parameters.resource.result == null ? "Test" : parameters.resource.result;
				parameters.resource.sourceBranch = parameters.resource.sourceBranch == null ? "Test" : parameters.resource.sourceBranch;
				parameters.resource.sourceVersion = parameters.resource.sourceVersion == null ? "Test" : parameters.resource.sourceVersion;
				parameters.resource.priority = parameters.resource.priority == null ? "Test" : parameters.resource.priority;
				parameters.resource.requestedFor = parameters.resource.requestedFor == null ? new TfsUser() { id = Guid.NewGuid(), displayName = "Test", imageUrl = "http://Test", uniqueName = "Test", url = "http://Test" } : parameters.resource.requestedFor;
				parameters.resource.requestedBy = parameters.resource.requestedBy == null ? new TfsUser() { id = Guid.NewGuid(), displayName = "Test", imageUrl = "http://Test", uniqueName = "Test", url = "http://Test" } : parameters.resource.requestedBy;

				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var card = new ConnectorCard();
				
				card.summary = "Build Process Completed";
				card.themeColor = "737373";

				card.sections = new List<Section>();
				card.sections.Add(
					new Section()
					{
						activityTitle = String.Format(title, "Build process completed by TFS Continues Integration"),
						activitySubtitle = "Details notification",
						activityText = String.Format("**Requested for:** {0}<br/>**Domain user:** {1}", String.IsNullOrEmpty(parameters.resource.requestedFor.displayName) ? "Test" : parameters.resource.requestedFor.displayName, String.IsNullOrEmpty(parameters.resource.requestedFor.uniqueName) ? "Test" : parameters.resource.requestedFor.uniqueName),
						activityImage = HttpUtility.UrlDecode(logoUrl),
						markdown = true
					});
				var section = new Section()
				{
					title = "**Information Details**",
					facts = new List<Fact>()
						{
							new Fact() { name = "Build: ", value = String.Format("{0}", parameters.resource.buildNumber) },
							new Fact() { name = "URL: ", value = String.Format("**{0}**", parameters.message.markdown) },
							new Fact() { name = "Queue Time: ", value = String.Format("{0}", parameters.resource.queueTime.ToString("dd/MM/yyyy h:mm:ss tt")) },
							new Fact() { name = "Start Time: ", value = String.Format("{0}", parameters.resource.startTime.ToString("dd/MM/yyyy h:mm:ss tt")) },
							new Fact() { name = "Finish Time: ", value = String.Format("{0}", parameters.resource.finishTime.ToString("dd/MM/yyyy h:mm:ss tt")) },
							new Fact() { name = "Build execution duration: ", value = String.Format("{0:00}hrs {1:00}min {2:00}sec", (parameters.resource.finishTime.Subtract(parameters.resource.startTime)).Hours, (parameters.resource.finishTime.Subtract(parameters.resource.startTime)).Minutes, (parameters.resource.finishTime.Subtract(parameters.resource.startTime)).Seconds) },
							new Fact() { name = "Build created on: ", value = String.Format("{0}", parameters.createdDate.ToString("dd/MM/yyyy h:mm:ss tt")) },
							new Fact() { name = "Result: ", value = String.Format("##### <span style='color:{0};'>**{1}**</span>", String.IsNullOrEmpty(parameters.resource.result) ? "TEST" : parameters.resource.result.ToUpper() == "FAILED" ? "red" : "darkblue", String.IsNullOrEmpty(parameters.resource.result) ? "TEST" : parameters.resource.result.ToUpper()) }
						},
					markdown = true
				};

				if (parameters.resource.result.ToUpper() == "FAILED")
				{
					section.facts.Add(new Fact() { name = "Error message: ", value = parameters.detailedMessage.markdown });
				}

				card.sections.Add(section);

				HttpResponseMessage response = await client.PostAsJsonAsync(urlWebhook, card);
				response.EnsureSuccessStatusCode();

				return Ok(new { success = true });
			}
			catch (Exception ex)
			{
				return InternalServerError(ex);
			}
		}

		[Route("api/Receiver/WorkitemUpdateEvent")]
		[HttpPost]
		public async Task<IHttpActionResult> WorkitemUpdateEvent(WorkitemUpdateEvent parameters)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var card = new ConnectorCard();
				
				card.summary = "Workitem Updated";
				card.themeColor = "737373";

				card.sections = new List<Section>();
				card.sections.Add(
					new Section()
					{
						activityTitle = String.Format(title, "Workitem Updated"),
						activitySubtitle = "Details notification",
						activityText = parameters.detailedMessage.markdown,
						activityImage = HttpUtility.UrlDecode(logoUrl),
						markdown = true
					});

				HttpResponseMessage response = await client.PostAsJsonAsync(urlWebhook, card);
				response.EnsureSuccessStatusCode();

				return Ok(new { success = true });
			}
			catch (Exception ex)
			{
				return InternalServerError(ex);
			}
		}
	}
}
