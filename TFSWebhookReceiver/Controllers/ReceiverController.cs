using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
			var card = new ConnectorCard();
			string urlWebhook = WebConfigurationManager.AppSettings["build.complete.webhook"].ToString();


			//card.title = "BUILD COMPLETED";
			card.text = "# Build Process Completed";
			//card.summary = parameters.message.markdown;
			card.themeColor = "#737373";
			
			card.sections = new List<Section>()
			{
				new Section()
				{
					activityTitle = "Build executed by TFS Continues Integration",
					activitySubtitle = "Details notification",
					activityText = String.Format("**Requested by:** {0}<br/>**Domain user:** {1}", parameters.resource.requestedBy.displayName, parameters.resource.requestedBy.uniqueName),
					activityImage = parameters.resource.requestedBy.imageUrl,
					markdown = true
				},
				new Section()
				{
					title = "**Information Details**",
					facts = new List<Fact>()
					{
						new Fact() { name = "Build: ", value = String.Format("{0}", parameters.resource.buildNumber) },
						new Fact() { name = "URL: ", value = String.Format("{0}", parameters.message.markdown) },
						new Fact() { name = "Queue Time: ", value = String.Format("{0}", parameters.resource.queueTime.ToString("dd/MM/yyyy h:mm:ss tt")) },
						new Fact() { name = "Start Time: ", value = String.Format("{0}", parameters.resource.startTime.ToString("dd/MM/yyyy h:mm:ss tt")) },
						new Fact() { name = "Finish Time: ", value = String.Format("{0}", parameters.resource.finishTime.ToString("dd/MM/yyyy h:mm:ss tt")) },
						new Fact() { name = "Duration: ", value = String.Format("{0:00}:{1:00}:{2:00}", (parameters.resource.finishTime.Subtract(parameters.resource.startTime)).Hours, (parameters.resource.finishTime.Subtract(parameters.resource.startTime)).Minutes, (parameters.resource.finishTime.Subtract(parameters.resource.startTime)).Seconds) },
						new Fact() { name = "Result: ", value = parameters.resource.result == "failed" ? String.Format("<span style='color:red;'>**{0}**</span>", parameters.resource.result) : String.Format("{0}", parameters.resource.result) }
					},
					markdown = true
				}
			};


			HttpResponseMessage response = await client.PostAsJsonAsync(urlWebhook, card);
			response.EnsureSuccessStatusCode();

			return Ok(new { success = true });
		}
	}
}
