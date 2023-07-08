using Optionals;
using RevoltSharp.Rest;

namespace RevoltSharp.Requests;
internal class AdminGetMessagesRequest : IRevoltRequest
{
    public Optional<string> nearby { get; set; }

    public Optional<string> before { get; set; }

    public Optional<string> after { get; set; }

    public Optional<string> sort { get; set; }

    public int limit { get; set; }

    public Optional<string> channel { get; set; }

    public Optional<string> author { get; set; }

    public Optional<string> query { get; set; }
}
