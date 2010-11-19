using System;
using System.IO;
using System.Net;
using log4net;
using log4net.Config;

namespace PostLog
{
	class Program
	{
		static void Main(string[] args)
		{
			XmlConfigurator.Configure();
			HttpListener listener = new HttpListener();
			listener.Prefixes.Add("http://localhost:34343/");
			listener.Start();
			Console.WriteLine("Started");
			Logger.Info("Hello");
			
			var ctx = listener.GetContext();

			using(var reader = new StreamReader(ctx.Request.InputStream))
			{
				var body = reader.ReadToEnd();
				Console.WriteLine(body);
			}
			Console.ReadLine();
			
		}

		private static readonly ILog Logger = LogManager.GetLogger(typeof (Program));
	}
}
