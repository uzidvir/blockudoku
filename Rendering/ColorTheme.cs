namespace BlockudokuGame.Rendering;

public static class ColorTheme
{
    public static readonly Color Background   = Color.FromArgb(28, 28, 38);
    public static readonly Color GridLine     = Color.FromArgb(55, 55, 70);
    public static readonly Color BoxBorder    = Color.FromArgb(110, 110, 135);
    public static readonly Color EmptyCell    = Color.FromArgb(42, 42, 55);
    public static readonly Color GhostInvalid = Color.FromArgb(160, 220, 60, 60);
    public static readonly Color ScoreText    = Color.FromArgb(220, 220, 240);
    public static readonly Color LabelText    = Color.FromArgb(140, 140, 165);

    public static readonly Dictionary<string, Color> PieceColors = new()
    {
        ["Yellow"]    = Color.FromArgb(255, 215, 55),
        ["Cyan"]      = Color.FromArgb(50, 210, 210),
        ["Green"]     = Color.FromArgb(75, 195, 95),
        ["Blue"]      = Color.FromArgb(55, 115, 250),
        ["Orange"]    = Color.FromArgb(250, 145, 45),
        ["Purple"]    = Color.FromArgb(155, 75, 215),
        ["Red"]       = Color.FromArgb(225, 65, 65),
        ["Pink"]      = Color.FromArgb(225, 125, 155),
        ["Teal"]      = Color.FromArgb(35, 175, 155),
        ["LightBlue"] = Color.FromArgb(95, 185, 235),
        ["Gold"]      = Color.FromArgb(215, 185, 45),
        ["Magenta"]   = Color.FromArgb(195, 75, 195),
        ["Salmon"]    = Color.FromArgb(225, 135, 105),
        ["Default"]   = Color.FromArgb(175, 175, 175),
    };

    public static Color GetPieceColor(string? key)
    {
        if (key is not null && PieceColors.TryGetValue(key, out var c)) return c;
        return PieceColors["Default"];
    }

    public static Color Darken(Color c, float factor = 0.55f) =>
        Color.FromArgb(c.A,
            (int)(c.R * factor),
            (int)(c.G * factor),
            (int)(c.B * factor));

    public static Color WithAlpha(Color c, int alpha) =>
        Color.FromArgb(alpha, c.R, c.G, c.B);
}
