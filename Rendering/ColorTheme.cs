namespace BlockudokuGame.Rendering;

public static class ColorTheme
{
    // ── Light theme matching the original Blockudoku look ──────────────────
    public static readonly Color Background   = Color.FromArgb(255, 255, 255);  // pure white
    public static readonly Color GridLine     = Color.FromArgb(210, 216, 232);  // soft gray-blue
    public static readonly Color BoxBorder    = Color.FromArgb(155, 165, 200);  // medium gray-blue
    public static readonly Color EmptyCell    = Color.FromArgb(232, 236, 248);  // very light lavender
    public static readonly Color GhostInvalid = Color.FromArgb(140, 220, 60, 60);
    public static readonly Color ScoreText    = Color.FromArgb(30, 35, 65);     // dark navy
    public static readonly Color LabelText    = Color.FromArgb(150, 155, 185);  // medium gray

    // ── All pieces are blue (original Blockudoku style) ────────────────────
    // Four subtle shades so pieces remain visually distinct on the board.
    private static readonly Color PieceBlue1 = Color.FromArgb(74,  144, 226);  // cornflower — main
    private static readonly Color PieceBlue2 = Color.FromArgb(55,  120, 210);  // slightly darker
    private static readonly Color PieceBlue3 = Color.FromArgb(95,  165, 240);  // slightly lighter
    private static readonly Color PieceBlue4 = Color.FromArgb(42,  100, 195);  // deeper blue

    public static readonly Dictionary<string, Color> PieceColors = new()
    {
        ["Yellow"]    = PieceBlue1,
        ["Cyan"]      = PieceBlue3,
        ["Green"]     = PieceBlue2,
        ["Blue"]      = PieceBlue1,
        ["Orange"]    = PieceBlue4,
        ["Purple"]    = PieceBlue2,
        ["Red"]       = PieceBlue4,
        ["Pink"]      = PieceBlue3,
        ["Teal"]      = PieceBlue2,
        ["LightBlue"] = PieceBlue3,
        ["Gold"]      = PieceBlue1,
        ["Magenta"]   = PieceBlue4,
        ["Salmon"]    = PieceBlue1,
        ["Default"]   = PieceBlue1,
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
