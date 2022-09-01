using System.Drawing;

namespace RevoltSharp
{
    public class RevoltColor
    {
        public int R { get; }
        public int G { get; }
        public int B { get; }
        public string Hex
        => string.Format("{0:X2}{1:X2}{2:X2}", R, G, B);

        public RevoltColor(int r, int g, int b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }
        public RevoltColor(string hex)
        {
            Color color = ColorTranslator.FromHtml(hex);
            R = color.R;
            G = color.G;
            B = color.B;
        }
    }
}
