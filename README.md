# Info
RevoltSharp is a bot lib used to connect to [Revolt](https://revolt.chat/) chat app.
This lib only supports bots for now and not userbots!

# Install
You can download the lib in visual studio/code using this [Nuget Package](https://www.nuget.org/packages/RevoltSharp)

Once it's installed you can use this basic example.
```cs
static void Main(string[] args)
{
    Start().GetAwaiter().GetResult();
}

public static RevoltClient Client;
public static async Task Start()
{
    Client = new RevoltClient("Bot Token", ClientMode.WebSocket);
    await Client.StartAsync();
    await Task.Delay(-1);
}
```
