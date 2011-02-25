using System.Collections.Generic;
using log4net.Core;

namespace PostLog
{
	public interface IBodyFormatter
	{
		string CreateBody(LoggingEvent loggingEvent, IEnumerable<HttpAppenderAttribute> parameters);

		string ContentType { get; }
	}
}