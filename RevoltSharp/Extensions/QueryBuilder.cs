using System.Text;

namespace RevoltSharp
{
	public class QueryBuilder
	{
		private StringBuilder sb = new StringBuilder();


		public QueryBuilder Add(string key, string value)
			=> AddIf(true, key, value);

		public QueryBuilder AddIf(bool match, string key, string value)
		{
			if (match)
			{
				if (sb.Length == 0)
					sb.Append($"?{key}={value}");
				else
					sb.Append($"&{key}={value}");
			}
			return this;
		}

		public QueryBuilder Add(string key, int value)
			=> AddIf(true, key, value);

		public QueryBuilder AddIf(bool match, string key, int value)
		{
			if (match)
			{
				if (sb.Length == 0)
					sb.Append($"?{key}={value}");
				else
					sb.Append($"&{key}={value}");
			}
			return this;
		}

		public QueryBuilder Add(string key, bool value)
			=> AddIf(true, key, value);

		public QueryBuilder AddIf(bool match, string key, bool value)
		{
			if (match)
			{
				if (sb.Length == 0)
					sb.Append($"?{key}={value}");
				else
					sb.Append($"&{key}={value}");
			}
			return this;
		}

		public string GetQuery()
		{
			return sb.ToString();
		}
	}
}