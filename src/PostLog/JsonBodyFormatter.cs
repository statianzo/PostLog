using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Xml;
using log4net.Core;

namespace PostLog
{
	public class JsonBodyFormatter : IBodyFormatter
	{
		private readonly JavaScriptSerializer _serializer;

		public JsonBodyFormatter()
		{
			_serializer = new JavaScriptSerializer();
		}
		public string CreateBody(LoggingEvent loggingEvent, IEnumerable<HttpAppenderAttribute> parameters)
		{
			var formattedAttributes = new Dictionary<string, object>();
			foreach (HttpAppenderAttribute attribute in parameters)
			{
				var value = attribute.Layout.Format(loggingEvent);
				if (value is DateTime)
					value = ConvertDateTime((DateTime)value);
				formattedAttributes[attribute.Name] = value;
			}
			return _serializer.Serialize(formattedAttributes);
		}

		private static string ConvertDateTime(DateTime date)
		{
			return XmlConvert.ToString(date,XmlDateTimeSerializationMode.Unspecified);
		}

		public string ContentType
		{
			get { return "application/json"; }
		}
	}
}