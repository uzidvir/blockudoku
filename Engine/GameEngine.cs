using BlockudokuGame.Models;

namespace BlockudokuGame.Engine;

public class GameEngine
{
    private readonly GameState      _state;
    private readonly ClearingEngine _clearer = new();
    private readonly ScoreEngine    _scorer  = new();
    private readonly Random         _rng     = new();

    public GameEngine(GameState state)
    {
        _state = state;
    }

    public void NewGame()
    {
        for (int r = 0; r < Board.Size; r++)
            for (int c = 0; c < Board.Size; c++)
                _state.Board.ClearCell(r, c);

        _state.Score          = 0;
        _state.DraggingIndex  = -1;
        _state.Phase          = GamePhase.Playing;
        RefillTray();
    }

    /// <summary>
    /// Attempt to place tray piece [trayIndex] at board anchor (boardRow, boardCol),
    /// where (boardRow, boardCol) corresponds to piece offset (0,0).
    /// </summary>
    public PlacementResult TryPlace(int trayIndex, int boardRow, int boardCol)
    {
        if (_state.Phase != GamePhase.Playing)
            return Fail();

        var piece = _state.TrayPieces[trayIndex];
        if (piece is null) return Fail();

        if (!CanPlace(piece, boardRow, boardCol))
            return Fail();

        // Commit cells to board
        foreach (var (dr, dc) in piece.Cells)
            _state.Board.FillCell(boardRow + dr, boardCol + dc, piece.ColorKey);

        // Remove piece from tray
        _state.TrayPieces[trayIndex] = null;

        // Detect and clear completed regions
        var clearResult = _clearer.ClearCompleted(_state.Board);

        // Calculate and apply score
        int delta = _scorer.Calculate(piece, clearResult);
        _state.Score += delta;
        if (_state.Score > _state.HighScore)
            _state.HighScore = _state.Score;

        // Refill tray when all 3 slots are empty
        if (_state.TrayPieces.All(p => p is null))
            RefillTray();

        // Check for game over
        if (!AnyPieceFits())
            _state.Phase = GamePhase.GameOver;

        return new PlacementResult(
            Success:      true,
            RowsCleared:  clearResult.RowsCleared,
            ColsCleared:  clearResult.ColsCleared,
            BoxesCleared: clearResult.BoxesCleared,
            ScoreDelta:   delta,
            ComboCount:   clearResult.TotalClearedRegions
        );
    }

    public bool CanPlace(PieceShape piece, int boardRow, int boardCol)
    {
        foreach (var (dr, dc) in piece.Cells)
        {
            int r = boardRow + dr;
            int c = boardCol + dc;
            if (r < 0 || r >= Board.Size || c < 0 || c >= Board.Size)
                return false;
            if (!_state.Board.IsCellEmpty(r, c))
                return false;
        }
        return true;
    }

    public bool AnyPieceFits()
    {
        foreach (var piece in _state.TrayPieces)
        {
            if (piece is null) continue;
            for (int r = 0; r < Board.Size; r++)
                for (int c = 0; c < Board.Size; c++)
                    if (CanPlace(piece, r, c))
                        return true;
        }
        return false;
    }

    private void RefillTray()
    {
        var pool = PieceShapeLibrary.AllShapes;
        for (int i = 0; i < 3; i++)
            _state.TrayPieces[i] = pool[_rng.Next(pool.Count)];
    }

    private static PlacementResult Fail() => new(false, 0, 0, 0, 0, 0);
}
