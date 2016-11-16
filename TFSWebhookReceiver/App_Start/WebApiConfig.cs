using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace TFSWebhookReceiver
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services
			#region Activando CORS de manera Global (Para todos los Controllers Web API)
			var cors = new EnableCorsAttribute(origins: "*", headers: "*", methods: "*");
			config.EnableCors(cors);
			#endregion

			// Web API routes
			config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);
		}
	}
}
