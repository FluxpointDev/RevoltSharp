using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;

namespace RevoltSharp;

/// <summary>
/// Special logger class with custom title and console colors.
/// </summary>
public class RevoltLogger
{
    public RevoltLogger(string title, RevoltLogSeverity logMode)
    {
        Title = title;
        LogMode = logMode;
        _ = Task.Factory.StartNew(async () =>
        {
            foreach (RevoltLogJsonMessage msg in MessageQueue.GetConsumingEnumerable())
            {
                if (msg.Data is string str)
                {
                    Console.WriteLine($"[{Title}] {LightMagenta}{msg.Message}\n" +
                        $"--- --- ---\n" +
                        $"{FormatJsonPretty(str)}\n" +
                        $"--- --- ---{Reset}");
                }
                else
                {
                    Console.WriteLine($"[{Title}] {LightMagenta}{msg.Message}\n" +
                        $"--- --- ---\n" +
                        $"{FormatJsonPretty(msg.Data)}\n" +
                        $"--- --- ---{Reset}");
                }
            }

        }, TaskCreationOptions.LongRunning);
    }

    private string Title { get; set; }

    private RevoltLogSeverity LogMode { get; set; }

    private BlockingCollection<RevoltLogJsonMessage> MessageQueue = new BlockingCollection<RevoltLogJsonMessage>();

    private static readonly string Reset = "\u001b[39m";

    #pragma warning disable IDE0051 // Remove unused private members

    private static readonly string Red = "\u001b[31m";
    private static readonly string LightRed = "\u001b[91m";
    private static readonly string Green = "\u001b[32m";
    private static readonly string LightGreen = "\u001b[92m";
    private static readonly string Yellow = "\u001b[33m";
    private static readonly string LightYellow = "\u001b[93m";
    private static readonly string Blue = "\u001b[34m";
    private static readonly string LightBlue = "\u001b[94m";
    private static readonly string Magenta = "\u001b[35m";
    private static readonly string LightMagenta = "\u001b[95m";
    private static readonly string Cyan = "\u001b[36m";
    private static readonly string LightCyan = "\u001b[96m";
    private static readonly string Grey = "\u001b[90m";
    private static readonly string LightGrey = "\u001b[37m";

    #pragma warning restore IDE0051 // Remove unused private members


    private static string FormatJsonPretty(string json)
    {
        dynamic parsedJson = JsonConvert.DeserializeObject(json);
        return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
    }

    private static string FormatJsonPretty(object json)
    {
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