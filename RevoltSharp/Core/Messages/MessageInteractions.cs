using System.Linq;

namespace RevoltSharp
{

    public class MessageInteractions
    {
        public MessageInteractions(Emoji[] reactions, bool restricted = false)
        {
            Reactions = reactions;
            RestrictReactions = restricted;
        }
        public Emoji[] Reactions { get; }
        public bool RestrictReactions { get; }

        internal MessageInteractionsJson ToJson()
        {
            return new MessageInteractionsJson
            {
                reactions = Reactions.Select(x => x.Name).ToArray(),
                restrict_reactions = RestrictReactions
            };
        }
    }
}
