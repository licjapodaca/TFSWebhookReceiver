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

		[Route("api/Receiver/BuildCompleteEvent")]
		[HttpPost]
		public async Task<IHttpActionResult> BuildCompleteEvent(BuildCompleteEvent parameters)
		{
			try
			{
				var card = new ConnectorCard();
				string urlWebhook = WebConfigurationManager.AppSettings["build.complete.webhook"].ToString(),
					logoUrl = WebConfigurationManager.AppSettings["urlLogoEncoded"].ToString();

				card.text = "# Build Process Completed";
				card.themeColor = "#737373";

				card.sections = new List<Section>()
				{
					new Section()
					{
						activityTitle = "Build executed by TFS Continues Integration",
						activitySubtitle = "Details notification",
						activityText = String.Format("**Requested by:** {0}<br/>**Domain user:** {1}", String.IsNullOrEmpty(parameters.resource.requestedBy.displayName) ? "Test" : parameters.resource.requestedBy.displayName, String.IsNullOrEmpty(parameters.resource.requestedBy.uniqueName) ? "Test" : parameters.resource.requestedBy.uniqueName),
						activityImage = HttpUtility.UrlDecode(logoUrl),
						markdown = true
					},
					new Section()
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
					}
				};


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
