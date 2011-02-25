using System;
using System.IO;
using System.Net;
using System.Threading;
using FakeItEasy;
using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
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

		[Test]
		public void ShouldCreateFormatter()
		{
			var formatterName = typeof (JsonBodyFormatter).FullName;
			var formatter = _appender.ConstructFormatter(formatterName);
			Assert.IsInstanceOf<JsonBodyFormatter>(formatter);
		}

		[Test]
		public void ShouldConfigure()
		{
			XmlConfigurator.Configure();
			ILog logger = LogManager.GetLogger(typeof(HttpAppenderTests));

			using (var listener = new HttpListener())
			{
				listener.Prefixes.Add("http://localhost:34343/");
				listener.Start();
				try
				{
					throw new Exception("KABOOM!");
				}
				catch (Exception e)
				{
					logger.Error("Oh noes!", e);
				}

				var ctx = listener.GetContext();
				using (var reader = new StreamReader(ctx.Request.InputStream))
				{
					var body = reader.ReadToEnd();
					Console.WriteLine(body);
					Assert.IsNotNull(body);
				}
				ctx.Response.Close();
			}
		}
	}
}