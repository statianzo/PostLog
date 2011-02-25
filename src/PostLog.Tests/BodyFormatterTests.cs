using System;
using System.Collections.Generic;
using FakeItEasy;
using log4net.Core;
using log4net.Layout;
using NUnit.Framework;

namespace PostLog.Tests
{
	[TestFixture]
	public class BodyFormatterTests
	{
		private List<HttpAppenderAttribute> _parameters;

		[SetUp]
		public void Setup()
		{
			_parameters = new List<HttpAppenderAttribute>();
		}

		private void AddParameter(string name)
		{
			var attribute = CreateAttribute(name, "value-of-" + name);
			_parameters.Add(attribute);
		}

		private static HttpAppenderAttribute CreateAttribute(string name, object value)
		{
			var layout = A.Fake<IRawLayout>();
			A.CallTo(() => layout.Format(A<LoggingEvent>.Ignored)).Returns(value);
			var attribute = new HttpAppenderAttribute
			{
				Name = name,
				Layout = layout
			};
			return attribute;
		}
		[Test]
		public void ShouldCreateFormBody()
		{
			const string expected = "thread=value-of-thread&message=value-of-message&exception=value-of-exception";
			TestFormatter<FormBodyFormatter>(expected, "thread", "message", "exception");
		}

		[Test]
		public void ShouldEscapeFormCharacters()
		{
			const string expected = "escape=+%2c%24%25!%7e.*";

			var attr = CreateAttribute("escape", " ,$%!~.*");
			_parameters.Add(attr);
			TestFormatter<FormBodyFormatter>(expected);
		}

		[Test]
		public void ShouldCreateJsonBody()
		{
			const string expected =
				@"{""thread"":""value-of-thread"",""message"":""value-of-message"",""exception"":""value-of-exception""}";
			TestFormatter<JsonBodyFormatter>(expected, "thread", "message", "exception");
		}

		[Test]
		public void ShouldUseIsoDateTime()
		{
			var date = new DateTime(2011, 2, 3, 4, 5, 6, 7);
			var attribute = CreateAttribute("date", date);
			_parameters.Add(attribute);


			const string expected =
				@"{""date"":""2011-02-03T04:05:06.007""}";
			TestFormatter<JsonBodyFormatter>(expected);
		}

		[Test]
		public void ShouldCreateXmlBody()
		{
			const string expected =
				"<log><thread>value-of-thread</thread><message>value-of-message</message><exception>value-of-exception</exception></log>";
			TestFormatter<XmlBodyFormatter>(expected, "thread", "message", "exception");
		}

		private void TestFormatter<TBuilder>(string expected, params string[] attributes) where TBuilder: IBodyFormatter, new()
		{
			var builder = new TBuilder();
			var e = new LoggingEvent(new LoggingEventData());
			foreach (var a in attributes)
			{
				AddParameter(a);
			}
			string body = builder.CreateBody(e, _parameters);
			Assert.AreEqual(expected, body);
		}
	}
}