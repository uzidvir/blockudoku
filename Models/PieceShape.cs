namespace BlockudokuGame.Models;

public class PieceShape
{
    public string Name { get; init; } = string.Empty;

    /// <summary>List of (row, col) cell offsets that make up the piece. Bounding box top-left is (0,0).</summary>
    public (int Row, int Col)[] Cells { get; init; } = Array.Empty<(int, int)>();

    /// <summary>Number of rows in the bounding box.</summary>
    public int RowSpan { get; init; }

    /// <summary>Number of columns in the bounding box.</summary>
    public int ColSpan { get; init; }

    /// <summary>Color key resolved by ColorTheme.</summary>
    public string ColorKey { get; init; } = "Default";
}
