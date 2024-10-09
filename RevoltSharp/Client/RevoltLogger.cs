using Newtonsoft.Json;
using RevoltSharp.Rest;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace RevoltSharp;

/// <summary>
/// Special logger class with custom title and console colors.
/// </summary>
public class RevoltLogger
{
    /// <summary>
    /// Initialize your own logging system with a custom title and log mode.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="logMode"></param>
    public RevoltLogger(string title, RevoltLogSeverity logMode)
    {
        Title = title;
        LogMode = logMode;
        LoggerTask = Task.Factory.StartNew(async () =>
        {
            foreach (RevoltLogJsonMessage msg in MessageQueue.GetConsumingEnumerable())
            {
                if (msg.Data is string str)
                {
                    Console.WriteLine($"[{Title}] {LightMagenta}{msg.Message}\n" +
                        $"--- --- ---\n" +
                        $"{FormatJsonPretty(str, AllowOptionals)}\n" +
                        $"--- --- ---{Reset}");
                }
                else
                {
                    Console.WriteLine($"[{Title}] {LightMagenta}{msg.Message}\n" +
                        $"--- --- ---\n" +
                        $"{FormatJsonPretty(msg.Data, AllowOptionals)}\n" +
                        $"--- --- ---{Reset}");
                }

            }

        }, TaskCreationOptions.LongRunning);
    }

    private string Title { get; set; }

    public bool AllowOptionals { get; set; }

    private Task LoggerTask { get; set; }

    private RevoltLogSeverity LogMode { get; set; }

    private BlockingCollection<RevoltLogJsonMessage> MessageQueue = new BlockingCollection<RevoltLogJsonMessage>();

    #pragma warning disable IDE0051 // Remove unused private members

    /// <summary> Reset console color </summary>
    public static readonly string Reset = "\u001b[39m";
    /// <summary> Red console color </summary>
    public static readonly string Red = "\u001b[31m";
    /// <summary> Light Red console color </summary>
    public static readonly string LightRed = "\u001b[91m";
    /// <summary> Green console color </summary>
    public static readonly string Green = "\u001b[32m";
    /// <summary> Light Green console color </summary>
    public static readonly string LightGreen = "\u001b[92m";
    /// <summary> Yellow console color </summary>
    public static readonly string Yellow = "\u001b[33m";
    /// <summary> Light Yellow console color </summary>
    public static readonly string LightYellow = "\u001b[93m";
    /// <summary> Blue console color </summary>
    public static readonly string Blue = "\u001b[34m";
    /// <summary> Light Blue console color </summary>
    public static readonly string LightBlue = "\u001b[94m";
    /// <summary> Magenta console color </summary>
    public static readonly string Magenta = "\u001b[35m";
    /// <summary> Light Magenta console color </summary>
    public static readonly string LightMagenta = "\u001b[95m";
    /// <summary> Cyan console color </summary>
    public static readonly string Cyan = "\u001b[36m";
    /// <summary> Light Cyan console color </summary>
    public static readonly string LightCyan = "\u001b[96m";
    /// <summary> Grey console color </summary>
    public static readonly string Grey = "\u001b[90m";
    /// <summary> Light Grey console color </summary>
    public static readonly string LightGrey = "\u001b[37m";

    #pragma warning restore IDE0051 // Remove unused private members


    private static string FormatJsonPretty(string json, bool allowOptionals)
    {
        if (allowOptionals)
        {
            using (StringReader text = new StringReader(json))
            using (JsonReader reader = new JsonTextReader(text))
            {
                dynamic parsedJson = RevoltClient.Deserializer.Deserialize<dynamic>(reader);
                return RevoltRestClient.SerializeJsonPretty(parsedJson);
            }
        }
        else
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }
        
    }

    private static string FormatJsonPretty(object json, bool allowOptionals)
    {
        if (allowOptionals)
            return RevoltRestClient.SerializeJsonPretty(json);

        return JsonConvert.SerializeObject(json, Formatting.Indented);
    }

    /// <summary>
    /// Special json log message with json data that can be a json string or class/object
    /// </summary>
    public void LogJson(string message, object data)
    {
        MessageQueue.Add(new RevoltLogJsonMessage { Message = message, Data = data });
    }


    /// <summary>
    /// Log a message to the console with the severity color
    /// <para>Info: White</para>
    /// <para>Warn: Yellow</para>
    /// <para>Error: Red</para>
    /// <para>Debug: Grey</para>
    /// </summary>
    public void LogMessage(string message, RevoltLogSeverity severity = RevoltLogSeverity.Debug)
    {
        if (severity < LogMode)
            return;

        switch (severity)
        {
            // White
            case RevoltLogSeverity.Info:
                {
                    Console.WriteLine($"[{Title}] {message}");
                }
                break;
            // Yellow
            case RevoltLogSeverity.Warn:
                {
                    Console.WriteLine($"[{Title}] {Yellow}{message}{Reset}");
                }
                break;
            // Red
            case RevoltLogSeverity.Error:
                {
                    Console.WriteLine($"[{Title}] {Red}{message}{Reset}");
                }
                break;
            // Grey
            case RevoltLogSeverity.Debug:
                {
                    Console.WriteLine($"[{Title}] {Grey}{message}{Reset}");
                }
                break;
        }
    }

    /// <summary>
    /// Log a rest response to the console with the color
    /// <para>Success: Green</para>
    /// <para>Fail: Light Red</para>
    /// </summary>
    public void LogRestMessage(HttpResponseMessage res, HttpMethod method, string message)
    {
        if (res.IsSuccessStatusCode)
        {
            Console.WriteLine($"[{Title}] {Green}({method.Method.ToUpper()}) {message}{Reset}");
        }
        else
        {
            Console.WriteLine($"[{Title}] {LightRed}({method.Method.ToUpper()}) {message}{Reset}");
        }
    }
}

internal class RevoltLogJsonMessage
{
    public string Message;
    public object Data;
}

/// <summary>
/// The severity of a log message raised by <see cref="RevoltClient.OnLog"/>.
/// </summary>
public enum RevoltLogSeverity
{
    /// <summary>
    /// All messages including debug ones.
    /// </summary>
    Debug,

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