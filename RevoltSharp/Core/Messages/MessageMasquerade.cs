using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public class MessageMasquerade
    {
        public MessageMasquerade(string name, string avatar = "", RevoltColor color = null)
        {
            Name = name;
            Avatar = avatar;
            Color = color == null ? new RevoltColor("") : color;
        }
        internal MessageMasquerade(MessageMasqueradeJson model)
        {
            Name = model.Name;
            Avatar = model.Avatar;
            Color = string.IsNullOrEmpty(model.Color) ? null : new RevoltColor(model.Color);
        }
        public string Name;
        public string Avatar;
        public RevoltColor Color;

        internal MessageMasqueradeJson ToJson()
        {
            return new MessageMasqueradeJson
            {
                Name = Name,
                Avatar = Avatar,
                Color = Color == null ? "#ffffff" : Color.IsEmpty ? "#ffffff" : Color.Hex
            };
        }
    }
}
