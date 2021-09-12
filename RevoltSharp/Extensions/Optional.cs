namespace RevoltSharp
{
    public class Optional<TValue>
    {
        public Optional(TValue value)
        {
            Value = value;
        }
        public TValue Value;
    }
}
