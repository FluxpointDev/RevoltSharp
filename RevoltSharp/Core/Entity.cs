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
        Created = Ulid.Parse(id).Time;
    }

    public DateTimeOffset Created { get; internal set; } 
}