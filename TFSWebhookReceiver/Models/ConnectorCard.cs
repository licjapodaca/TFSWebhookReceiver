using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TFSWebhookReceiver.Models
{
	public class ConnectorCard
	{
		public string summary { get; set; }
		public string text { get; set; }
		public string title { get; set; }
		public string themeColor { get; set; }

		public List<Section> sections { get; set; }
		public List<PotentialAction> potentialAction { get; set; }
	}

	public class Section
	{
		public string title { get; set; }
		public string activityTitle { get; set; }
		public string activitySubtitle { get; set; }
		public string activityImage { get; set; }
		public string activityText { get; set; }
		public string text { get; set; }
		public bool markdown { get; set; }

		public List<Fact> facts { get; set; }
		public List<Image> images { get; set; }
		public List<PotentialAction> potentialAction { get; set; }
	}

	public class Fact
	{
		public string name { get; set; }
		public string value { get; set; }
	}

	public class Image
	{
		public string title { get; set; }
		public string image { get; set; }
	}

	public class PotentialAction
	{
		public string name { get; set; }
		public string target { get; set; }
		public string @context { get; set; }
		public string @type { get; set; }
	}
}