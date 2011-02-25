using System.Collections.Generic;
using System.Web;
using log4net.Core;

namespace PostLog
{
	public class FormBodyFormatter:IBodyFormatter
	{
		public string CreateBody(LoggingEvent loggingEvent, IEnumerable<HttpAppenderAttribute> parameters)
		{
			var formatted = new List<string>();
			foreach (HttpAppenderAttribute attribute in parameters)
			{
				object value = attribute.Layout.Format(loggingEvent);
				formatted.Add(string.Format("{0}={1}", attribute.Name, HttpUtility.UrlEncode(value.ToString())));
			}
			return string.Join("&", formatted.ToArray());
		}

		public string ContentType
		{
			get { return "application/x-www-form-urlencoded"; }
		}
	}
}