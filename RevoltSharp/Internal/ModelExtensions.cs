using Optionals;

namespace RevoltSharp
{

    internal static class ModelExtensions
    {
        internal static Optional<Attachment?> ToModel(this Optional<AttachmentJson> json, RevoltClient client)
        {
            if (!json.HasValue)
                return Optional.None<Attachment?>();

            return Optional.Some(json.Value.ToModel(client));
        }

        internal static Attachment? ToModel(this AttachmentJson json, RevoltClient client)
        {
            if (json == null)
                return null;

            return Attachment.Create(client, json);
        }
    }
}
