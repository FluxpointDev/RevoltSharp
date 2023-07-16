using System.Text;

namespace RevoltSharp;

public class QueryBuilder
{
	private readonly StringBuilder sb = new StringBuilder();

	public QueryBuilder Add(string key, string value)
	{
        if (sb.Length == 0)
            sb.Append($"?{key}={value}");
        else
            sb.Append($"&{key}={value}");

		return this;
    }

	public QueryBuilder AddIf(bool match, string key, string value)
	{
		if (match)
			Add(key, value);
		
		return this;
	}

	public QueryBuilder Add(string key, int value)
	{
        if (sb.Length == 0)
            sb.Append($"?{key}={value}");
        else
            sb.Append($"&{key}={value}");

		return this;
    }

	public QueryBuilder AddIf(bool match, string key, int value)
	{
		if (match)
			Add(key, value);

		return this;
	}

	public QueryBuilder Add(string key, bool value)
	{
        if (sb.Length == 0)
            sb.Append($"?{key}={value}");
        else
            sb.Append($"&{key}={value}");

		return this;
    }

	public QueryBuilder AddIf(bool match, string key, bool value)
	{
		if (match)
			Add(key, value);

		return this;
	}

	public string GetQuery()
	{
		if (sb.Length == 0)
			return string.Empty;

		return sb.ToString();
	}

	public override string ToString()
		=> GetQuery();
}