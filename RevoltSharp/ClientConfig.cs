namespace RevoltSharp
{
    public class ClientConfig
    {
        public ClientDebugConfig Debug = new ClientDebugConfig();
    }
    public class ClientDebugConfig
    {
        public bool LogFullEvents { get; set; }
        public bool LogRestRequest { get; set; }
        public bool LogRestRequestContent { get; set; }
        public bool CheckRestRequest { get; set; }
    }

}
