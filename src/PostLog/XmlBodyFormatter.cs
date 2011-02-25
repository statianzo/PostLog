using System.Collections.Generic;
using System.Xml.Linq;
using log4net.Core;

namespace PostLog
{
	public class XmlBodyFormatter:IBodyFormatter
	{
		public string CreateBody(LoggingEvent loggingEvent, IEnumerable<HttpAppenderAttribute> parameters)
		{
			var root = new XElement("log");
			foreach (HttpAppenderAttribute attribute in parameters)
				root.Add(new XElement(attribute.Name, attribute.Layout.Format(loggingEvent)));

			return root.ToString(SaveOptions.DisableFormatting);
		}

		public string ContentType
		{
			get { return "text/xml"; }
		}
	}
}