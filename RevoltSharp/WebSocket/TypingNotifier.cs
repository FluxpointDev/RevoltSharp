using RevoltSharp.Rest;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace RevoltSharp.WebSocket;

public class TypingNotifier : IDisposable
{
    private readonly RevoltRestClient _client;
    private readonly CancellationTokenSource _cancelToken;
    private readonly string _channel;

    internal TypingNotifier(RevoltRestClient rest, string channel)
    {
        _client = rest;
        _cancelToken = new CancellationTokenSource();
        _channel = channel;
        _ = RunAsync();
    }

    internal async Task RunAsync()
    {
        _client.Client.WebSocket.TypingChannels.TryAdd(_channel, this);
        try
        {
            CancellationToken token = _cancelToken.Token;
            while (!_cancelToken.IsCancellationRequested)
            {
                try
                {
                    await _client.TriggerTypingChannelAsync(_channel);
                }
                catch { }

                await Task.Delay(2500, token);
            }
        }
        catch { }
        Dispose();
    }

    public void Stop()
    {
        _cancelToken.Cancel();
    }

    public void Dispose()
    {
        _client.Client.WebSocket.TypingChannels.TryRemove(_channel, out _);
        _ = _client.StopTypingChannelAsync(_channel);
        _cancelToken.Cancel();
    }
}
