namespace RevoltSharp;

public class AdminClient
{
    internal AdminClient(RevoltClient client)
    {
        Client = client;
    }
    internal RevoltClient Client;
}
