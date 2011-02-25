using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using log4net.Appender;
using log4net.Core;

namespace PostLog
{
	public class HttpAppender : AppenderSkeleton
	{
		private readonly List<HttpAppenderAttribute> _parameters;
		private IBodyFormatter _formatter;

		public HttpAppender()
		{
			_parameters = new List<HttpAppenderAttribute>();

			Method = "POST";
			UserAgent = "HttpAppender";
		}

		public string Method { get; set; }
		public string UserAgent { get; set; }
		public string Uri { get; set; }
		public string FormatterType { get; set; }

		private IBodyFormatter BodyFormatter
		{
			get { return _formatter ?? (_formatter = ConstructFormatter(FormatterType)); }
		}

		public IBodyFormatter ConstructFormatter(string formatterType)
		{
			Type type = Type.GetType(formatterType, true);
			return (IBodyFormatter) Activator.CreateInstance(type);
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
				string body = BodyFormatter.CreateBody(loggingEvent, _parameters);
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
											request.EndGetResponse(a);
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
			var request = (HttpWebRequest) WebRequest.Create(Uri);
			request.Method = Method;
			request.UserAgent = UserAgent;
			request.ContentType = BodyFormatter.ContentType;

			return request;
		}
	}
}