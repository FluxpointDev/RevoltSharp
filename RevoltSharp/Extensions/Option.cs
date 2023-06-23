namespace RevoltSharp;

public class Option<TValue>
{
    public Option(TValue value)
    {
        Value = value;
    }
    public readonly TValue Value;
}
