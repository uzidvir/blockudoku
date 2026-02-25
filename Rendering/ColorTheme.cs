namespace BlockudokuGame.Rendering;

public static class ColorTheme
{
    // ── Light theme matching the original Blockudoku look ──────────────────
    public static readonly Color Background   = Color.FromArgb(255, 255, 255);  // pure white
    public static readonly Color GridLine     = Color.FromArgb(200, 200, 200);  // light gray
    public static readonly Color BoxBorder    = Color.FromArgb(130, 130, 130);  // medium gray
    public static readonly Color EmptyCell    = Color.FromArgb(232, 236, 248);  // very light lavender
    public static readonly Color GhostInvalid = Color.FromArgb(140, 220, 60, 60);
    public static readonly Color ScoreText    = Color.FromArgb(30, 35, 65);     // dark navy
    public static readonly Color LabelText    = Color.FromArgb(150, 155, 185);  // medium gray

    // ── All pieces are the same solid blue ─────────────────────────────────
    private static readonly Color PieceBlue = Color.FromArgb(30, 90, 180);

    public static readonly Dictionary<string, Color> PieceColors = new()
    {
        ["Yellow"]    = PieceBlue,
        ["Cyan"]      = PieceBlue,
        ["Green"]     = PieceBlue,
        ["Blue"]      = PieceBlue,
        ["Orange"]    = PieceBlue,
        ["Purple"]    = PieceBlue,
        ["Red"]       = PieceBlue,
        ["Pink"]      = PieceBlue,
        ["Teal"]      = PieceBlue,
        ["LightBlue"] = PieceBlue,
        ["Gold"]      = PieceBlue,
        ["Magenta"]   = PieceBlue,
        ["Salmon"]    = PieceBlue,
        ["Default"]   = PieceBlue,
    };

    public static Color GetPieceColor(string? key)
    {
        if (key is not null && PieceColors.TryGetValue(key, out var c)) return c;
        return PieceColors["Default"];
    }

    // ── Hint overlays (steps 1 → 2 → 3: amber → orange → deep-orange) ────────
    public static readonly Color HintColor1 = Color.FromArgb(180, 255, 200,   0);
    public static readonly Color HintColor2 = Color.FromArgb(150, 255, 140,   0);
    public static readonly Color HintColor3 = Color.FromArgb(120, 255,  80,   0);
    public static readonly Color HintBorder = Color.FromArgb(255, 200,   0);       // solid amber

    public static Color Darken(Color c, float factor = 0.55f) =>
        Color.FromArgb(c.A,
            (int)(c.R * factor),
            (int)(c.G * factor),
            (int)(c.B * factor));

    public static Color WithAlpha(Color c, int alpha) =>
        Color.FromArgb(alpha, c.R, c.G, c.B);
}
