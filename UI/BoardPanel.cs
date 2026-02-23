using System.Drawing.Drawing2D;
using BlockudokuGame.Engine;
using BlockudokuGame.Models;
using BlockudokuGame.Rendering;

namespace BlockudokuGame.UI;

public class BoardPanel : Panel
{
    private readonly GameState      _state;
    private readonly GameEngine     _engine;
    private readonly BoardRenderer  _boardRend = new();
    private readonly PieceRenderer  _pieceRend = new();

    // Drag state
    private int   _dragTrayIndex  = -1;
    private Point _mousePos       = Point.Empty;
    private int   _ghostAnchorRow = -1;
    private int   _ghostAnchorCol = -1;
    private bool  _ghostValid     = false;

    public event Action<PlacementResult>? PiecePlaced;
    public event Action? DragEnded;

    public BoardPanel(GameState state, GameEngine engine)
    {
        _state  = state;
        _engine = engine;
        DoubleBuffered = true;
        SetStyle(ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.AllPaintingInWmPaint  |
                 ControlStyles.UserPaint, true);
    }

    public void BeginDrag(int trayIndex)
    {
        _dragTrayIndex = trayIndex;
    }

    /// <summary>Called by MainForm on forwarded mouse move. mouseInPanel = coords relative to this panel.</summary>
    public void UpdateDrag(Point mouseInPanel)
    {
        _mousePos = mouseInPanel;
        UpdateGhost();
        Invalidate();
    }

    private void UpdateGhost()
    {
        if (_dragTrayIndex < 0) return;
        var piece = _state.TrayPieces[_dragTrayIndex];
        if (piece is null) return;

        // Anchor pixel = mouse minus pick-offset * cell size
        int anchorPx = _mousePos.X - _state.DragPickCol * BoardRenderer.CellSize;
        int anchorPy = _mousePos.Y - _state.DragPickRow * BoardRenderer.CellSize;

        var (anchorRow, anchorCol) = BoardRenderer.PixelToCell(anchorPx, anchorPy);
        _ghostAnchorRow = anchorRow;
        _ghostAnchorCol = anchorCol;

        _ghostValid = anchorRow >= 0 && anchorCol >= 0
                      && _engine.CanPlace(piece, anchorRow, anchorCol);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        IEnumerable<(int, int)>? ghostCells = null;
        Color ghostPieceColor = Color.White;

        if (_dragTrayIndex >= 0 && _ghostAnchorRow >= 0)
        {
            var piece = _state.TrayPieces[_dragTrayIndex];
            if (piece is not null)
            {
                ghostCells = piece.Cells
                    .Select(c => (_ghostAnchorRow + c.Row, _ghostAnchorCol + c.Col));
                ghostPieceColor = ColorTheme.GetPieceColor(piece.ColorKey);
            }
        }

        _boardRend.Draw(g, _state.Board, ghostCells, _ghostValid, ghostPieceColor);

        // Draw floating piece image following the cursor
        if (_dragTrayIndex >= 0)
        {
            var dragged = _state.TrayPieces[_dragTrayIndex];
            if (dragged is not null)
            {
                int ox = _mousePos.X - _state.DragPickCol * BoardRenderer.CellSize;
                int oy = _mousePos.Y - _state.DragPickRow * BoardRenderer.CellSize;
                var color = ColorTheme.GetPieceColor(dragged.ColorKey);
                _pieceRend.DrawPieceAtCellSize(g, dragged, ox, oy, BoardRenderer.CellSize, color);
            }
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (_dragTrayIndex >= 0)
        {
            _mousePos = e.Location;
            UpdateGhost();
            Invalidate();
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (_dragTrayIndex < 0) return;

        PlacementResult result;
        if (_ghostAnchorRow >= 0 && _ghostAnchorCol >= 0)
            result = _engine.TryPlace(_dragTrayIndex, _ghostAnchorRow, _ghostAnchorCol);
        else
            result = new PlacementResult(false, 0, 0, 0, 0, 0);

        EndDrag();
        PiecePlaced?.Invoke(result);
    }

    public void EndDrag()
    {
        _dragTrayIndex  = -1;
        _ghostAnchorRow = -1;
        _ghostAnchorCol = -1;
        _state.DraggingIndex = -1;
        DragEnded?.Invoke();
        Invalidate();
    }
}
