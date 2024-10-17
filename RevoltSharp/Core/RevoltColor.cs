using System;
using System.Drawing;

namespace RevoltSharp;

/// <summary>
/// Represents a color in Revolt, with Hex and RGB values accepted.
/// </summary>
public class RevoltColor
{
    #region Constant Colors

    /// <summary>
    /// Represents the color red, or #FF0000.
    /// </summary>
    public static RevoltColor Red { get; } = new RevoltColor(255, 0, 0);
    /// <summary>
    /// Represents a darker shade of red, or #8B0000.
    /// </summary>
    public static RevoltColor DarkRed { get; } = new RevoltColor(139, 0, 0);
    /// <summary>
    /// Represents the color orange, or #FFA500.
    /// </summary>
    public static RevoltColor Orange { get; } = new RevoltColor(255, 165, 0);
    /// <summary>
    /// Represents the color yellow, or #FFFF00.
    /// </summary>
    public static RevoltColor Yellow { get; } = new RevoltColor(255, 255, 0);
    /// <summary>
    /// Represents the color green, or #00FF00.
    /// </summary>
    public static RevoltColor Green { get; } = new RevoltColor(0, 255, 0);
    /// <summary>
    /// Represents a darker shade of green, or #006400.
    /// </summary>
    public static RevoltColor DarkGreen { get; } = new RevoltColor(0, 100, 0);
    /// <summary>
    /// Represents the color cyan, or #00FFFF.
    /// </summary>
    public static RevoltColor Cyan { get; } = new RevoltColor(0, 255, 255);
    /// <summary>
    /// Represents the color blue, or #0000FF.
    /// </summary>
    public static RevoltColor Blue { get; } = new RevoltColor(0, 0, 255);
    /// <summary>
    /// Represents a darker shade of blue, or #00008B.
    /// </summary>
    public static RevoltColor DarkBlue { get; } = new RevoltColor(0, 0, 139);
    /// <summary>
    /// Represents the color purple, or #800080.
    /// </summary>
    public static RevoltColor Purple { get; } = new RevoltColor(128, 0, 128);
    /// <summary>
    /// Represents the color pink, or #FFC0CB.
    /// </summary>
    public static RevoltColor Pink { get; } = new RevoltColor(255, 192, 203);
    /// <summary>
    /// Represents the color black, or #000000.
    /// </summary>
    public static RevoltColor Black { get; } = new RevoltColor(0, 0, 0);
    /// <summary>
    /// Represents the color white, or #FFFFFF.
    /// </summary>
    public static RevoltColor White { get; } = new RevoltColor(255, 255, 255);
    /// <summary>
    /// Represents the color gray, or #808080.
    /// </summary>
    public static RevoltColor Gray { get; } = new RevoltColor(128, 128, 128);
    /// <summary>
    /// Represents a color that's not quite black, or #36393F.
    /// </summary>
    public static RevoltColor NotQuiteBlack { get; } = new RevoltColor(54, 57, 63);

    #endregion

    public bool HasValue => !string.IsNullOrEmpty(Value);
    public bool IsLinearGradient => (!string.IsNullOrEmpty(Value) && Value.StartsWith("linear-gradient", StringComparison.OrdinalIgnoreCase));
    public bool IsHex => (!string.IsNullOrEmpty(Value) && Value.StartsWith("#", StringComparison.OrdinalIgnoreCase));
    public bool IsRGB => (!string.IsNullOrEmpty(Value) && int.TryParse(Value[0].ToString(), out _));
    public int R { get; internal set; } = 0;
    public int G { get; internal set; } = 0;
    public int B { get; internal set; } = 0;
    public string Hex
    => (R == 0 && G == 0 && B == 0) ? "#000000" : '#' + string.Format("{0:X2}{1:X2}{2:X2}", R, G, B);
    public string Value { get; set; }

    /// <summary>
    /// Creates a new RevoltColor from a <see cref="Color"/>.
    /// </summary>
    /// <param name="color">The color to use</param>
    public RevoltColor(Color color)
    {
        R = color.R;
        G = color.G;
        B = color.B;
        Value = $"{R}, {G}, {B}";
    }

    /// <summary>
    /// Creates a new RevoltColor from RGB values.
    /// </summary>
    /// <param name="r">The value of red (cannot be lower than 0 or higher than 255)</param>
    /// <param name="g">The value of green (cannot be lower than 0 or higher than 255)</param>
    /// <param name="b">The value of blue (cannot be lower than 0 or higher than 255)</param>
    public RevoltColor(int r, int g, int b)
    {
        if (r < 0 || r > 255)
            throw new ArgumentOutOfRangeException(nameof(r), "Value must be between 0 and 255.");
        if (g < 0 || g > 255)
            throw new ArgumentOutOfRangeException(nameof(g), "Value must be between 0 and 255.");
        if (b < 0 || b > 255)
            throw new ArgumentOutOfRangeException(nameof(b), "Value must be between 0 and 255.");

        R = r;
        G = g;
        B = b;
        Value = $"{r}, {g}, {b}";
    }

    /// <summary>
    /// Creates a new RevoltColor from a hex string.
    /// </summary>
    /// <param name="hex">The hex code to use</param>
    public RevoltColor(string hex)
    {
        Value = hex;
        if (HasValue)
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

    /// <summary> Returns a string that represents the current object.</summary>
    /// <returns> Color format </returns>
    public override string ToString()
    {
        return Value;
    }
}