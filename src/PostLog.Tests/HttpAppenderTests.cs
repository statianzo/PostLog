using System;
using FakeItEasy;
using log4net.Core;
using log4net.Layout;
using LogPost;
using NUnit.Framework;

namespace PostLog.Tests
{
	[TestFixture]
	public class HttpAppenderTests
	{
		[SetUp]
		public void Setup()
		{
			_appender = new HttpAppender();
		}

		private HttpAppender _appender;

		private void AddParameter(string name)
		{
			var layout = A.Fake<IRawLayout>();
			A.CallTo(() => layout.Format(A<LoggingEvent>.Ignored)).Returns("value-of-" + name);
			var attribute = new HttpAppenderAttribute
			{
				Name = name,
				Layout = layout
			};
			_appender.AddParameter(attribute);
		}

		[Test]
		public void ShouldCreateFormBody()
		{
			var e = new LoggingEvent(new LoggingEventData());
			AddParameter("thread");
			AddParameter("message");
			AddParameter("exception");
			string body = _appender.CreateFormBody(e);
			Console.WriteLine(body);
		}
	}
}