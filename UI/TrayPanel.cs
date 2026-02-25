using BlockudokuGame.Models;
using BlockudokuGame.Rendering;

namespace BlockudokuGame.UI;

public class TrayPanel : Panel
{
    private readonly GameState     _state;
    private readonly PieceRenderer _renderer = new();

    // Drag initiation tracking
    private int   _pressedSlot = -1;
    private Point _pressPoint  = Point.Empty;
    private bool  _dragging    = false;

    public event Action<int, int, int>? DragStarted;  // trayIndex, pickRow, pickCol

    public TrayPanel(GameState state)
    {
        _state = state;
        DoubleBuffered = true;
        SetStyle(ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.AllPaintingInWmPaint  |
                 ControlStyles.UserPaint, true);
    }

    private Rectangle SlotBounds(int index) =>
        new Rectangle(index * (Width / 3), 0, Width / 3, Height);

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.Clear(ColorTheme.Background);

        for (int i = 0; i < 3; i++)
        {
            var piece = _state.TrayPieces[i];
            if (piece is null) continue;

            float alpha = (i == _state.DraggingIndex) ? 0.25f : 1.0f;
            _renderer.DrawPiece(e.Graphics, piece, SlotBounds(i), 1.0f, alpha);

            // Draw hint badge (amber border + step number) if this slot is in the hint sequence
            for (int step = 0; step < _state.HintMoves.Length; step++)
            {
                var move = _state.HintMoves[step];
                if (move is null || move.TrayIndex != i) continue;

                var sb = SlotBounds(i);
                using var pen   = new Pen(ColorTheme.HintBorder, 3f);
                using var font  = new Font("Segoe UI", 14f, FontStyle.Bold);
                using var brush = new SolidBrush(ColorTheme.HintBorder);
                e.Graphics.DrawRectangle(pen, sb.X + 2, sb.Y + 2, sb.Width - 4, sb.Height - 4);
                e.Graphics.DrawString((step + 1).ToString(), font, brush, sb.X + 6, sb.Y + 6);
            }
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button != MouseButtons.Left) return;

        int slot = e.X / (Width / 3);
        if (slot < 0 || slot >= 3) return;

        var piece = _state.TrayPieces[slot];
        if (piece is null) return;

        _pressedSlot = slot;
        _pressPoint  = e.Location;
        _dragging    = false;
        Capture      = true;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (_pressedSlot < 0) return;
        if (_dragging) return;

        // Start drag after threshold
        if (Math.Abs(e.X - _pressPoint.X) < 4 && Math.Abs(e.Y - _pressPoint.Y) < 4)
            return;

        _dragging = true;

        var piece  = _state.TrayPieces[_pressedSlot]!;
        var bounds = SlotBounds(_pressedSlot);

        // Compute which cell of the piece was clicked
        int cellPx = Math.Min(
            (bounds.Width  - 10) / Math.Max(piece.ColSpan, 1),
            (bounds.Height - 10) / Math.Max(piece.RowSpan, 1));
        cellPx = Math.Clamp(cellPx, 4, 52);

        int totalW = piece.ColSpan * cellPx;
        int totalH = piece.RowSpan * cellPx;
        int ox = bounds.X + (bounds.Width  - totalW) / 2;
        int oy = bounds.Y + (bounds.Height - totalH) / 2;

        int pickCol = Math.Clamp((_pressPoint.X - ox) / Math.Max(cellPx, 1), 0, piece.ColSpan - 1);
        int pickRow = Math.Clamp((_pressPoint.Y - oy) / Math.Max(cellPx, 1), 0, piece.RowSpan - 1);

        DragStarted?.Invoke(_pressedSlot, pickRow, pickCol);
        Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        Capture      = false;
        _pressedSlot = -1;
        _dragging    = false;
    }
}
