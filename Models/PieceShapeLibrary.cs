namespace BlockudokuGame.Models;

public static class PieceShapeLibrary
{
    public static readonly IReadOnlyList<PieceShape> AllShapes;

    static PieceShapeLibrary()
    {
        AllShapes = BuildShapes().AsReadOnly();
    }

    private static List<PieceShape> BuildShapes()
    {
        var shapes = new List<PieceShape>();

        // ── Singles ─────────────────────────────────────────────────────────
        shapes.Add(Make("Dot",        "Yellow",    new[] { (0, 0) }));

        // ── Dominoes ────────────────────────────────────────────────────────
        shapes.Add(Make("H-Domino",   "Cyan",      new[] { (0, 0), (0, 1) }));
        shapes.Add(Make("V-Domino",   "Cyan",      new[] { (0, 0), (1, 0) }));

        // ── Triominoes ───────────────────────────────────────────────────────
        shapes.Add(Make("H-Tri",      "Green",     new[] { (0, 0), (0, 1), (0, 2) }));
        shapes.Add(Make("V-Tri",      "Green",     new[] { (0, 0), (1, 0), (2, 0) }));
        shapes.Add(Make("L-Tri-1",    "Green",     new[] { (0, 0), (1, 0), (1, 1) }));  // ┘
        shapes.Add(Make("L-Tri-2",    "Green",     new[] { (0, 1), (1, 0), (1, 1) }));  // └
        shapes.Add(Make("L-Tri-3",    "Green",     new[] { (0, 0), (0, 1), (1, 0) }));  // ┐
        shapes.Add(Make("L-Tri-4",    "Green",     new[] { (0, 0), (0, 1), (1, 1) }));  // ┌

        // ── Tetrominoes ──────────────────────────────────────────────────────
        // I-pieces
        shapes.Add(Make("I-H4",       "Blue",      new[] { (0, 0), (0, 1), (0, 2), (0, 3) }));
        shapes.Add(Make("I-V4",       "Blue",      new[] { (0, 0), (1, 0), (2, 0), (3, 0) }));

        // O-piece (2x2 square)
        shapes.Add(Make("O-2x2",      "Orange",    new[] { (0, 0), (0, 1), (1, 0), (1, 1) }));

        // T-piece and rotations
        shapes.Add(Make("T-Up",       "Purple",    new[] { (0, 1), (1, 0), (1, 1), (1, 2) }));
        shapes.Add(Make("T-Down",     "Purple",    new[] { (0, 0), (0, 1), (0, 2), (1, 1) }));
        shapes.Add(Make("T-Left",     "Purple",    new[] { (0, 0), (1, 0), (1, 1), (2, 0) }));
        shapes.Add(Make("T-Right",    "Purple",    new[] { (0, 1), (1, 0), (1, 1), (2, 1) }));

        // S-piece and rotations
        shapes.Add(Make("S-H",        "Red",       new[] { (0, 1), (0, 2), (1, 0), (1, 1) }));
        shapes.Add(Make("S-V",        "Red",       new[] { (0, 0), (1, 0), (1, 1), (2, 1) }));

        // Z-piece and rotations
        shapes.Add(Make("Z-H",        "Pink",      new[] { (0, 0), (0, 1), (1, 1), (1, 2) }));
        shapes.Add(Make("Z-V",        "Pink",      new[] { (0, 1), (1, 0), (1, 1), (2, 0) }));

        // L-piece and rotations
        shapes.Add(Make("L-0",        "Orange",    new[] { (0, 0), (1, 0), (2, 0), (2, 1) }));
        shapes.Add(Make("L-90",       "Orange",    new[] { (0, 0), (0, 1), (0, 2), (1, 0) }));
        shapes.Add(Make("L-180",      "Orange",    new[] { (0, 0), (0, 1), (1, 1), (2, 1) }));
        shapes.Add(Make("L-270",      "Orange",    new[] { (0, 2), (1, 0), (1, 1), (1, 2) }));

        // J-piece and rotations
        shapes.Add(Make("J-0",        "Blue",      new[] { (0, 1), (1, 1), (2, 0), (2, 1) }));
        shapes.Add(Make("J-90",       "Blue",      new[] { (0, 0), (1, 0), (1, 1), (1, 2) }));
        shapes.Add(Make("J-180",      "Blue",      new[] { (0, 0), (0, 1), (1, 0), (2, 0) }));
        shapes.Add(Make("J-270",      "Blue",      new[] { (0, 0), (0, 1), (0, 2), (1, 2) }));

        // ── Pentominoes ───────────────────────────────────────────────────────
        // I-5
        shapes.Add(Make("I-H5",       "Teal",      new[] { (0, 0), (0, 1), (0, 2), (0, 3), (0, 4) }));
        shapes.Add(Make("I-V5",       "Teal",      new[] { (0, 0), (1, 0), (2, 0), (3, 0), (4, 0) }));

        // Big L (4+1)
        shapes.Add(Make("BigL-0",     "LightBlue", new[] { (0, 0), (1, 0), (2, 0), (3, 0), (3, 1) }));
        shapes.Add(Make("BigL-90",    "LightBlue", new[] { (0, 0), (0, 1), (0, 2), (0, 3), (1, 0) }));
        shapes.Add(Make("BigL-180",   "LightBlue", new[] { (0, 0), (0, 1), (1, 1), (2, 1), (3, 1) }));
        shapes.Add(Make("BigL-270",   "LightBlue", new[] { (0, 3), (1, 0), (1, 1), (1, 2), (1, 3) }));

        // Big J (mirror BigL)
        shapes.Add(Make("BigJ-0",     "LightBlue", new[] { (0, 1), (1, 1), (2, 1), (3, 0), (3, 1) }));
        shapes.Add(Make("BigJ-90",    "LightBlue", new[] { (0, 0), (1, 0), (1, 1), (1, 2), (1, 3) }));
        shapes.Add(Make("BigJ-180",   "LightBlue", new[] { (0, 0), (0, 1), (1, 0), (2, 0), (3, 0) }));
        shapes.Add(Make("BigJ-270",   "LightBlue", new[] { (0, 0), (0, 1), (0, 2), (0, 3), (1, 3) }));

        // U-shapes
        shapes.Add(Make("U-H",        "Magenta",   new[] { (0, 0), (0, 2), (1, 0), (1, 1), (1, 2) }));
        shapes.Add(Make("U-V",        "Magenta",   new[] { (0, 0), (0, 1), (1, 0), (2, 0), (2, 1) }));

        // 2×3 and 3×2 rectangles
        shapes.Add(Make("Rect-2x3",   "Gold",      new[] { (0, 0), (0, 1), (0, 2), (1, 0), (1, 1), (1, 2) }));
        shapes.Add(Make("Rect-3x2",   "Gold",      new[] { (0, 0), (0, 1), (1, 0), (1, 1), (2, 0), (2, 1) }));

        // Full 3×3 square
        shapes.Add(Make("Square-3x3", "Gold",      new[]
        {
            (0, 0), (0, 1), (0, 2),
            (1, 0), (1, 1), (1, 2),
            (2, 0), (2, 1), (2, 2)
        }));

        // Corner / Plus-arm pieces
        shapes.Add(Make("Corner-TL",  "Salmon",    new[] { (0, 0), (1, 0), (1, 1), (1, 2), (2, 2) }));
        shapes.Add(Make("Corner-TR",  "Salmon",    new[] { (0, 2), (1, 0), (1, 1), (1, 2), (2, 0) }));
        shapes.Add(Make("Plus-3",     "Salmon",    new[] { (0, 1), (1, 0), (1, 1), (1, 2), (2, 1) }));

        return shapes;
    }

    private static PieceShape Make(string name, string color, (int Row, int Col)[] cells)
    {
        int rowSpan = cells.Max(c => c.Row) + 1;
        int colSpan = cells.Max(c => c.Col) + 1;
        return new PieceShape
        {
            Name     = name,
            ColorKey = color,
            Cells    = cells,
            RowSpan  = rowSpan,
            ColSpan  = colSpan
        };
    }
}
