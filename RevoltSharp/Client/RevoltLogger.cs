using System;
using System.Net.Http;

namespace RevoltSharp
{

	public class RevoltLogger
	{
		private static string Reset = "\u001b[39m";

		private static string Red = "\u001b[31m";
		private static string LightRed = "\u001b[91m";
		private static string Green = "\u001b[32m";
		private static string LightGreen = "\u001b[92m";
		private static string Yellow = "\u001b[33m";
		private static string LightYellow = "\u001b[93m";
		private static string Blue = "\u001b[34m";
		private static string LightBlue = "\u001b[94m";
		private static string Magenta = "\u001b[35m";
		private static string LightMagenta = "\u001b[95m";
		private static string Cyan = "\u001b[36m";
		private static string LightCyan = "\u001b[96m";
		private static string Grey = "\u001b[90m";
		private static string LightGrey = "\u001b[37m";

		//public void TestColors()
		//{
		//	Console.WriteLine($"{Red}Red{Reset}\n" +
		//		$"{LightRed}Light Red{Reset}\n" +
		//		$"{Green}Green{Reset}\n" +
		//		$"{LightGreen}Light Green{Reset}\n" +
		//		$"{Yellow}Yellow{Reset}\n" +
		//		$"{LightYellow}Light Yellow{Reset}\n" +
		//		$"{Blue}Blue{Reset}\n" +
		//		$"{LightBlue}Light Blue{Reset}\n" +
		//		$"{Magenta}Magenta{Reset}\n" +
		//		$"{LightMagenta}Light Magenta{Reset}\n" +
		//		$"{Cyan}Cyan{Reset}\n" +
		//		$"{LightCyan}Light Cyan{Reset}\n" +
		//		$"{Grey}Grey{Reset}\n" +
		//		$"{LightGrey}Light Grey{Reset}\n");
		//}

		public void LogMessage(RevoltClient client, string message, RevoltLogSeverity severity)
		{
			if (severity < client.Config.LogMode)
				return;

			switch (severity)
			{
				// White
				case RevoltLogSeverity.Info:
					{
						if (!client.Config.LogReducedColors)
							Console.WriteLine($"[RevoltSharp] {message}");
						else
							Console.WriteLine($"[RevoltSharp] Info : {message}");
					}
					break;
				// Yellow
				case RevoltLogSeverity.Warn:
					{
						if (!client.Config.LogReducedColors)
							Console.WriteLine($"[RevoltSharp] {Yellow}{message}{Reset}");
						else
							Console.WriteLine($"[RevoltSharp] {Yellow}Warn{Reset} : {message}");
					}
					break;
				// Red
				case RevoltLogSeverity.Error:
					{
						if (!client.Config.LogReducedColors)
							Console.WriteLine($"[RevoltSharp] {Red}{message}{Reset}");
						else
							Console.WriteLine($"[RevoltSharp] {Red}Error{Reset}: {message}");
					}
					break;
				// Grey
				case RevoltLogSeverity.Verbose:
					{
						if (!client.Config.LogReducedColors)
							Console.WriteLine($"[RevoltSharp] {Grey}{message}{Reset}");
						else
							Console.WriteLine($"[RevoltSharp] {Grey}Debug{Reset}: {message}");
					}
					break;
			}
		}

		public void LogRestMessage(RevoltClient client, HttpResponseMessage res, HttpMethod method, string message)
		{
			if (!client.Config.Debug.LogRestRequest)
				return;

			if (res.IsSuccessStatusCode)
			{
				if (!client.Config.LogReducedColors)
					Console.WriteLine($"[RevoltSharp] {Green}({method.Method.ToUpper()}) {message}{Reset}");
				else
					Console.WriteLine($"[RevoltSharp] {Green}Ok{Reset}   : ({method.Method.ToUpper()}) {message}");
			}
			else
			{
				if (!client.Config.LogReducedColors)
					Console.WriteLine($"[RevoltSharp] {Cyan}({method.Method.ToUpper()}) {message}{Reset}");
				else
					Console.WriteLine($"[RevoltSharp] {Cyan}Fail{Reset} : ({method.Method.ToUpper()}) {message}");
			}
		}
	}

	/// <summary>
	/// The severity of a log message raised by <see cref="ClientEvents.OnLog"/>.
	/// </summary>
	public enum RevoltLogSeverity
	{
		/// <summary>
		/// Debug info message.
		/// </summary>
		Verbose,

		/// <summary>
		/// Error message info.
		/// </summary>
		Error,

		/// <summary>
		/// Log info and warning messages.
		/// </summary>
		Warn,

		/// <summary>
		/// Only log info messages.
		/// </summary>
		Info,

		/// <summary>
		/// Do not log anything.
		/// </summary>
		None
	}
}