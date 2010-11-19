using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;

namespace LogPost
{
	public class HttpAppender : AppenderSkeleton
	{
		private readonly List<HttpAppenderAttribute> _parameters;
		private readonly JavaScriptSerializer _serializer;

		public HttpAppender()
		{
			_parameters = new List<HttpAppenderAttribute>();
			_serializer = new JavaScriptSerializer();

			Method = "POST";
			UserAgent = "HttpAppender";
			Format = "FORM";
			XmlRootName = "log";
		}

		public string Method { get; set; }
		public string XmlRootName { get; set; }
		public string UserAgent { get; set; }
		public string Format { get; set; }
		public string Uri { get; set; }

		public string CreateJsonBody(LoggingEvent loggingEvent)
		{
			var formattedAttributes = new Dictionary<string, object>();
			foreach (HttpAppenderAttribute attribute in _parameters)
				formattedAttributes[attribute.Name] = attribute.Layout.Format(loggingEvent);
			return _serializer.Serialize(formattedAttributes);
		}

		public string CreateXmlBody(LoggingEvent loggingEvent)
		{
			var root = new XElement(XmlRootName);
			foreach (HttpAppenderAttribute attribute in _parameters)
				root.Add(new XElement(attribute.Name, attribute.Layout.Format(loggingEvent)));

			return root.ToString(SaveOptions.DisableFormatting);
		}

		public string CreateFormBody(LoggingEvent loggingEvent)
		{
			var bodyBuilder = new StringBuilder();
			foreach (HttpAppenderAttribute attribute in _parameters)
			{
				object value = attribute.Layout.Format(loggingEvent);
				bodyBuilder.AppendFormat("{0}={1}&", attribute.Name, HttpUtility.HtmlEncode(value.ToString()));
			}
			return bodyBuilder.ToString();
		}

		public string CreateBody(LoggingEvent loggingEvent)
		{
			switch (Format.ToUpper())
			{
				case "JSON":
					return CreateJsonBody(loggingEvent);
				case "FORM":
					return CreateFormBody(loggingEvent);
				case "XML":
					return CreateXmlBody(loggingEvent);
				default:
					ErrorHandler.Error("Invalid format specified: " + Format);
					return "";
			}
		}

		public void AddParameter(HttpAppenderAttribute item)
		{
			_parameters.Add(item);
		}

		protected override void Append(LoggingEvent loggingEvent)
		{
			byte[] bodyBytes;

			try
			{
				string body = CreateBody(loggingEvent);
				bodyBytes = Encoding.UTF8.GetBytes(body);
			}
			catch (Exception e)
			{
				ErrorHandler.Error("Failed to create body", e);
				return;
			}

			HttpWebRequest request = BuildRequest();

			request.BeginGetRequestStream(r =>
				{
					Stream stream = request.EndGetRequestStream(r);
					stream.BeginWrite(bodyBytes, 0, bodyBytes.Length, c =>
						{
							try
							{
								stream.Dispose();
								request.BeginGetResponse(a =>
									{
										try
										{
											var response = (HttpWebResponse)request.EndGetResponse(a);
											((IDisposable)request).Dispose();
											((IDisposable)response).Dispose();
										}
										catch (Exception e)
										{
											ErrorHandler.Error("Failed to get response", e);
										}
									}, null);
								stream.EndWrite(c);
							}
							catch (Exception e)
							{
								ErrorHandler.Error("Failed to write", e);
							}
						}, null);
				}, null);
		}

		private HttpWebRequest BuildRequest()
		{
			var request = (HttpWebRequest)WebRequest.Create(Uri);
			request.Method = Method;
			request.UserAgent = UserAgent;

			return request;
		}
	}

	public class HttpAppenderAttribute
	{
		public string Name { get; set; }
		public IRawLayout Layout { get; set; }
	}
}