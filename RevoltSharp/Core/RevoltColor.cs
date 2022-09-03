using System;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace RevoltSharp
{
    public class RevoltColor
    {
        public bool IsEmpty { get; internal set; }
        public int R { get; internal set; } = 0;
        public int G { get; internal set; } = 0;
        public int B { get; internal set; } = 0;
        public string Hex
        => (R == 0 && G == 0 && B == 0) ? "#000000" : string.Format("{0:X2}{1:X2}{2:X2}", R, G, B);

        public RevoltColor(Color color)
        {
            R = color.R;
            G = color.G;
            B = color.B;
        }
        public RevoltColor(int r, int g, int b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }
        public RevoltColor(string hex)
        {
            IsEmpty = string.IsNullOrEmpty(hex);
            if (!IsEmpty)
            {
                try
                {
                    Color color = ColorTranslator.FromHtml(hex);
                    R = color.R;
                    G = color.G;
                    B = color.B;
                }
                catch { }
            }
        }
    }
}
