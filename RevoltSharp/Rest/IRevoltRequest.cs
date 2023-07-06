namespace RevoltSharp.Rest;


/// <summary>
/// Send a custom json body request to the Revolt instance API<br /><br />
/// Use <see cref="RevoltRestClient.SendRequestAsync(RequestType, string, IRevoltRequest)"/> or <see cref="RevoltRestClient.SendRequestAsync{TResponse}(RequestType, string, IRevoltRequest, bool)"/>
/// </summary>
public interface IRevoltRequest { }