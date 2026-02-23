using BlockudokuGame.Models;

namespace BlockudokuGame.Rendering;

public class PieceRenderer
{
    /// <summary>
    /// Draw a piece scaled to fit inside <paramref name="bounds"/>, centered.
    /// <paramref name="alpha"/> 0..1 controls transparency (used to dim dragged piece).
    /// </summary>
    public void DrawPiece(
        Graphics g,
        PieceShape piece,
        Rectangle bounds,
        float scale = 1.0f,
        float alpha = 1.0f)
    {
        int maxCellW = (bounds.Width  - 10) / Math.Max(piece.ColSpan, 1);
        int maxCellH = (bounds.Height - 10) / Math.Max(piece.RowSpan, 1);
        // Cap at board cell size so tray pieces match the board visually
        int cellPx   = Math.Min(Math.Min(maxCellW, maxCellH), BoardRenderer.CellSize);
        cellPx = Math.Max(cellPx, 4);

        int totalW = piece.ColSpan * cellPx;
        int totalH = piece.RowSpan * cellPx;
        int ox = bounds.X + (bounds.Width  - totalW) / 2;
        int oy = bounds.Y + (bounds.Height - totalH) / 2;

        Color baseColor = ColorTheme.GetPieceColor(piece.ColorKey);
        int   a         = (int)(Math.Clamp(alpha, 0f, 1f) * 255);
        Color drawColor = Color.FromArgb(a, baseColor);

        foreach (var (dr, dc) in piece.Cells)
        {
            var rect = new Rectangle(
                ox + dc * cellPx + 1,
                oy + dr * cellPx + 1,
                cellPx - 2,
                cellPx - 2);

            using var fill = new SolidBrush(drawColor);
            g.FillRectangle(fill, rect);

            // Top highlight
            using var glint = new SolidBrush(Color.FromArgb((int)(a * 0.22f), 255, 255, 255));
            g.FillRectangle(glint, new Rectangle(rect.X, rect.Y, rect.Width, rect.Height / 3));

            // Bottom shadow
            Color dark = ColorTheme.Darken(baseColor);
            using var shadow = new SolidBrush(Color.FromArgb(a, dark));
            g.FillRectangle(shadow, new Rectangle(
                rect.X, rect.Y + rect.Height * 2 / 3,
                rect.Width, rect.Height / 3));
        }
    }

    /// <summary>
    /// Draw a piece at board cell-size scale, anchored so piece offset (0,0) maps to (originX, originY).
    /// Used for the floating drag image above the board.
    /// </summary>
    public void DrawPieceAtCellSize(
        Graphics g,
        PieceShape piece,
        int originX,
        int originY,
        int cellSize,
        Color color)
    {
        foreach (var (dr, dc) in piece.Cells)
        {
            var rect = new Rectangle(
                originX + dc * cellSize + 1,
                originY + dr * cellSize + 1,
                cellSize - 2,
                cellSize - 2);

            using var fill = new SolidBrush(Color.FromArgb(200, color));
            g.FillRectangle(fill, rect);

            using var glint = new SolidBrush(Color.FromArgb(55, 255, 255, 255));
            g.FillRectangle(glint, new Rectangle(rect.X, rect.Y, rect.Width, rect.Height / 3));
        }
    }
}
