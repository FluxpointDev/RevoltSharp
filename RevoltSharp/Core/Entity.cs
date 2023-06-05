using Newtonsoft.Json;
using System;

namespace RevoltSharp;

public abstract class Entity
{
    [JsonIgnore]
    internal RevoltClient Client { get; }

    internal Entity(RevoltClient client)
    {
        Client = client;
    }
}
public abstract class CreatedEntity : Entity
{
    internal CreatedEntity(RevoltClient client, string id) : base(client)
    {
        if (id.StartsWith(':'))
            Id = id.Substring(1, id.Length - 2);
        else
            Id = id;

        if (Ulid.TryParse(Id, out Ulid UID))
            CreatedAt = UID.Time;
    }

    /// <summary>
    /// Id of the object.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Date of when the object was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
}