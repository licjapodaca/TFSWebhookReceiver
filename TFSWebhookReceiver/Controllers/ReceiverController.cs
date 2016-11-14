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

		[HttpPost]
		public async Task<IHttpActionResult> BuildCompleteEvent(BuildCompleteEvent parameters)
		{
			var card = new ConnectorCard();
			string urlWebhook = WebConfigurationManager.AppSettings["build.complete.webhook"].ToString();


			card.text = "Hello World!";
			card.summary = "Este es el summary";
			card.title = "Y este es el titulo";
			card.themeColor = "#737373";
			card.sections = new List<Section>()
			{
				new Section()
				{
					activityTitle = "Titulo Seccion",
					activitySubtitle = "Subtitulo Seccion",
					activityText = "Texto de la Eccion",
					activityImage = parameters.resource.requestedBy.imageUrl,
					facts = new List<Fact>()
					{
						new Fact() { name = "Build", value = parameters.resource.buildNumber },
						new Fact() { name = "URL", value = "[Prueba](http://svr-tfs:8080/tfs)" }
					}
				}
			};


			HttpResponseMessage response = await client.PostAsJsonAsync(urlWebhook, card);
			response.EnsureSuccessStatusCode();

			return Ok(new { success = true });
		}
	}
}
