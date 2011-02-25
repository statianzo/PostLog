using log4net.Layout;

namespace PostLog
{
	public class HttpAppenderAttribute
	{
		public string Name { get; set; }
		public IRawLayout Layout { get; set; }
	}
}