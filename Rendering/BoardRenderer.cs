using BlockudokuGame.Models;

namespace BlockudokuGame.Rendering;

public class BoardRenderer
{
    public const int CellSize  = 40;
    public const int CellPad   = 2;

    private static readonly Pen GridPen  = new(ColorTheme.GridLine, 1f);
    private static readonly Pen BoxPen   = new(ColorTheme.BoxBorder, 2.5f);

    public void Draw(
        Graphics g,
        Board board,
        IEnumerable<(int Row, int Col)>? ghostCells,
        bool ghostValid,
        Color ghostPieceColor)
    {
        g.Clear(ColorTheme.Background);
        DrawCells(g, board);
        if (ghostCells is not null)
            DrawGhost(g, ghostCells, ghostValid, ghostPieceColor);
        DrawGridLines(g);
        DrawBoxBorders(g);
    }

    private static void DrawCells(Graphics g, Board board)
    {
        for (int r = 0; r < Board.Size; r++)
        {
            for (int c = 0; c < Board.Size; c++)
            {
                var rect = CellRect(r, c);
                if (board.GetCell(r, c) == CellState.Filled)
                {
                    var color = ColorTheme.GetPieceColor(board.GetCellColor(r, c));
                    DrawFilledCell(g, rect, color);
                }
                else
                {
                    DrawEmptyCell(g, rect);
                }
            }
        }
    }

    private static void DrawGhost(
        Graphics g,
        IEnumerable<(int Row, int Col)> cells,
        bool valid,
        Color pieceColor)
    {
        Color ghostColor = valid
            ? ColorTheme.WithAlpha(pieceColor, 130)
            : ColorTheme.GhostInvalid;

        foreach (var (r, c) in cells)
        {
            if (r < 0 || r >= Board.Size || c < 0 || c >= Board.Size) continue;
            var rect = CellRect(r, c);
            var inner = InnerRect(rect);
            using var brush = new SolidBrush(ghostColor);
            g.FillRectangle(brush, inner);
        }
    }

    private static void DrawFilledCell(Graphics g, Rectangle rect, Color color)
    {
        var inner = InnerRect(rect);
        using var fill = new SolidBrush(color);
        g.FillRectangle(fill, inner);
    }

    private static void DrawEmptyCell(Graphics g, Rectangle rect)
    {
        var inner = InnerRect(rect);
        using var fill = new SolidBrush(ColorTheme.EmptyCell);
        g.FillRectangle(fill, inner);
    }

    private static void DrawGridLines(Graphics g)
    {
        int total = Board.Size * CellSize;
        for (int i = 0; i <= Board.Size; i++)
        {
            int pos = i * CellSize;
            g.DrawLine(GridPen, pos, 0, pos, total);
            g.DrawLine(GridPen, 0, pos, total, pos);
        }
    }

    private static void DrawBoxBorders(Graphics g)
    {
        int total = Board.Size * CellSize;
        for (int i = 0; i <= Board.BoxSize; i++)
        {
            int pos = i * Board.BoxSize * CellSize;
            g.DrawLine(BoxPen, pos, 0, pos, total);
            g.DrawLine(BoxPen, 0, pos, total, pos);
        }
    }

    public static Rectangle CellRect(int row, int col) =>
        new Rectangle(col * CellSize, row * CellSize, CellSize, CellSize);

    internal static Rectangle InnerRect(Rectangle cell) =>
        new Rectangle(
            cell.X + CellPad,
            cell.Y + CellPad,
            cell.Width  - CellPad * 2,
            cell.Height - CellPad * 2);

    /// <summary>Convert pixel coords (within the board panel) to a board cell. Returns (-1,-1) if out of bounds.</summary>
    public static (int Row, int Col) PixelToCell(int px, int py)
    {
        int col = px / CellSize;
        int row = py / CellSize;
        if (row < 0 || row >= Board.Size || col < 0 || col >= Board.Size)
            return (-1, -1);
        return (row, col);
    }
}
