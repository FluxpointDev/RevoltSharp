using Optionals;

namespace RevoltSharp.Rest.Requests;

internal class ReasonRequest : IRevoltRequest
{
    public Optional<string> reason { get; set; }

}
