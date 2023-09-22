using System.Drawing;

namespace RevoltSharp;


public class RevoltColor
{
    #region Constant Colors

    public static RevoltColor Red { get; } = new RevoltColor(255, 0, 0);
    public static RevoltColor DarkRed { get; } = new RevoltColor(139, 0, 0);
    public static RevoltColor Orange { get; } = new RevoltColor(255, 165, 0);
    public static RevoltColor Yellow { get; } = new RevoltColor(255, 255, 0);
    public static RevoltColor Green { get; } = new RevoltColor(0, 255, 0);
    public static RevoltColor DarkGreen { get; } = new RevoltColor(0, 100, 0);
    public static RevoltColor Blue { get; } = new RevoltColor(0, 0, 255);
    public static RevoltColor DarkBlue { get; } = new RevoltColor(0, 0, 139);
    public static RevoltColor Purple { get; } = new RevoltColor(128, 0, 128);
    public static RevoltColor Pink { get; } = new RevoltColor(255, 192, 203);
    public static RevoltColor Black { get; } = new RevoltColor(0, 0, 0);
    public static RevoltColor White { get; } = new RevoltColor(255, 255, 255);
    public static RevoltColor Gray { get; } = new RevoltColor(128, 128, 128);
    public static RevoltColor NotQuiteBlack { get; } = new RevoltColor(54, 57, 63);

    #endregion

    public bool IsEmpty { get; internal set; }
    public int R { get; internal set; } = 0;
    public int G { get; internal set; } = 0;
    public int B { get; internal set; } = 0;
    public string Hex
    => (R == 0 && G == 0 && B == 0) ? "#000000" : '#' + string.Format("{0:X2}{1:X2}{2:X2}", R, G, B);

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