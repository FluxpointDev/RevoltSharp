namespace RevoltSharp
{
    public abstract class Entity
    {
        public RevoltClient Client { get; }

        protected Entity(RevoltClient client)
        {
            Client = client;
        }
    }
}