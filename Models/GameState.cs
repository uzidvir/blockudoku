namespace BlockudokuGame.Models;

public enum GamePhase { Playing, GameOver }

/// <summary>Represents one suggested placement in a hint sequence.</summary>
public record HintMove(int TrayIndex, int Row, int Col);

public class GameState
{
    public Board Board { get; } = new Board();

    /// <summary>Exactly 3 slots; a slot is null when the piece has been placed.</summary>
    public PieceShape?[] TrayPieces { get; } = new PieceShape?[3];

    public int Score { get; set; }
    public int HighScore { get; set; }
    public GamePhase Phase { get; set; } = GamePhase.Playing;

    /// <summary>Index 0..2 of the piece currently being dragged; -1 if none.</summary>
    public int DraggingIndex { get; set; } = -1;

    /// <summary>Pick offset row: which row of the piece was clicked when drag started.</summary>
    public int DragPickRow { get; set; }

    /// <summary>Pick offset col: which col of the piece was clicked when drag started.</summary>
    public int DragPickCol { get; set; }

    // ── Hint state ────────────────────────────────────────────────────────────

    /// <summary>Optimal placement sequence: [0]=first, [1]=second, [2]=third. Null = not set.</summary>
    public HintMove?[] HintMoves { get; } = new HintMove?[3];

    /// <summary>True when at least the first hint move is populated.</summary>
    public bool HintActive => HintMoves[0] is not null;

    /// <summary>Clears all hint moves.</summary>
    public void ClearHint()
    {
        for (int i = 0; i < HintMoves.Length; i++)
            HintMoves[i] = null;
    }
}
