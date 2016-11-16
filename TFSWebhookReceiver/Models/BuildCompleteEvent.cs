using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TFSWebhookReceiver.Models
{
	public class BuildCompleteEvent
	{
		
		public Guid subscriptionId { get; set; }
		
		public int notificationId { get; set; }
		
		public Guid id { get; set; }
		
		public string eventType { get; set; }
		
		public string publisherId { get; set; }
		
		public string resourceVersion { get; set; }
		
		public DateTime createdDate { get; set; }


		
		public Message message { get; set; }
		
		public DetailedMessage detailedMessage { get; set; }
		
		public Resource resource { get; set; }
	}

	public class Message
	{

		
		public string text { get; set; }

		
		public string html { get; set; }

		
		public string markdown { get; set; }
	}

	public class DetailedMessage
	{

		
		public string text { get; set; }

		
		public string html { get; set; }

		
		public string markdown { get; set; }
	}

	public class Resource
	{
		
		public int id { get; set; }
		
		public string buildNumber { get; set; }
		
		public string status { get; set; }
		
		public string result { get; set; }
		
		public DateTime queueTime { get; set; }
		
		public DateTime startTime { get; set; }
		
		public DateTime finishTime { get; set; }
		
		public string url { get; set; }
		
		public int buildNumberRevision { get; set; }
		
		public string uri { get; set; }
		
		public string sourceBranch { get; set; }
		
		public string sourceVersion { get; set; }
		
		public string priority { get; set; }
		
		public string reason { get; set; }
		
		public TfsUser requestedFor { get; set; }
		
		public TfsUser requestedBy { get; set; }
	}

	public class TfsUser
	{
		
		public Guid id { get; set; }
		
		public string displayName { get; set; }
		
		public string uniqueName { get; set; }
		
		public string url { get; set; }
		
		public string imageUrl { get; set; }
	}
}