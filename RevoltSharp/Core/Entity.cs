using Newtonsoft.Json;
using RevoltSharp.Internal;
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
        {
            if (Ulid.TryParse(id.Substring(1, id.Length - 2), out Ulid ID))
                CreatedAt = ID.Time;
        }
        else
        {
            if (Ulid.TryParse(id, out Ulid ID))
                CreatedAt = ID.Time;
        }
    }

    public DateTimeOffset CreatedAt { get; internal set; } 
}