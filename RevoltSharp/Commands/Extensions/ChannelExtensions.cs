using RevoltSharp.Rest;
using System.Threading.Tasks;

namespace RevoltSharp.Commands
{
    public static class ChannelExtensions
    {
        public static Task<Message> SendMessageAsync(this Channel chan, string content, string[] attachments = null)
           => chan.Client.Rest.SendMessageAsync(chan.Id, content, attachments);
    }
}
