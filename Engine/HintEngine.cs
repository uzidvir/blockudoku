using BlockudokuGame.Models;

namespace BlockudokuGame.Engine;

/// <summary>
/// Finds the globally optimal placement sequence for all tray pieces via depth-3 exhaustive search.
/// </summary>
public class HintEngine
{
    private readonly ClearingEngine _clearer = new();
    private readonly ScoreEngine    _scorer  = new();

    /// <summary>
    /// Returns the best ordered sequence of moves for the current tray pieces,
    /// or null if no piece can be placed anywhere.
    /// The array length equals the number of non-null tray pieces (1-3).
    /// </summary>
    public HintMove[]? FindBest(GameState state)
    {
        var available = Enumerable.Range(0, 3)
            .Where(i => state.TrayPieces[i] is not null)
            .ToList();

        if (available.Count == 0) return null;

        int        bestScore  = -1;
        int        bestFilled = int.MaxValue;
        HintMove[]? best      = null;

        foreach (int i1 in available)
        {
            var p1 = state.TrayPieces[i1]!;

            for (int r1 = 0; r1 <= Board.Size - p1.RowSpan; r1++)
            for (int c1 = 0; c1 <= Board.Size - p1.ColSpan; c1++)
            {
                if (!CanPlace(state.Board, p1, r1, c1)) continue;

                var b1  = state.Board.Clone();
                Fill(b1, p1, r1, c1);
                var cl1 = _clearer.ClearCompleted(b1);
                int s1  = _scorer.Calculate(p1, cl1);

                var rem1 = available.Where(i => i != i1).ToList();

                if (rem1.Count == 0)
                {
                    int filled = CountFilled(b1);
                    if (s1 > bestScore || (s1 == bestScore && filled < bestFilled))
                    {
                        bestScore = s1; bestFilled = filled;
                        best = [new HintMove(i1, r1, c1)];
                    }
                    continue;
                }

                foreach (int i2 in rem1)
                {
                    var p2 = state.TrayPieces[i2]!;

                    for (int r2 = 0; r2 <= Board.Size - p2.RowSpan; r2++)
                    for (int c2 = 0; c2 <= Board.Size - p2.ColSpan; c2++)
                    {
                        if (!CanPlace(b1, p2, r2, c2)) continue;

                        var b2  = b1.Clone();
                        Fill(b2, p2, r2, c2);
                        var cl2 = _clearer.ClearCompleted(b2);
                        int s2  = _scorer.Calculate(p2, cl2);

                        var rem2 = rem1.Where(i => i != i2).ToList();

                        if (rem2.Count == 0)
                        {
                            int total  = s1 + s2;
                            int filled = CountFilled(b2);
                            if (total > bestScore || (total == bestScore && filled < bestFilled))
                            {
                                bestScore = total; bestFilled = filled;
                                best = [new HintMove(i1, r1, c1), new HintMove(i2, r2, c2)];
                            }
                            continue;
                        }

                        int i3 = rem2[0];
                        var p3 = state.TrayPieces[i3]!;

                        for (int r3 = 0; r3 <= Board.Size - p3.RowSpan; r3++)
                        for (int c3 = 0; c3 <= Board.Size - p3.ColSpan; c3++)
                        {
                            if (!CanPlace(b2, p3, r3, c3)) continue;

                            var b3  = b2.Clone();
                            Fill(b3, p3, r3, c3);
                            var cl3 = _clearer.ClearCompleted(b3);
                            int s3  = _scorer.Calculate(p3, cl3);

                            int total  = s1 + s2 + s3;
                            int filled = CountFilled(b3);
                            if (total > bestScore || (total == bestScore && filled < bestFilled))
                            {
                                bestScore = total; bestFilled = filled;
                                best = [
                                    new HintMove(i1, r1, c1),
                                    new HintMove(i2, r2, c2),
                                    new HintMove(i3, r3, c3)
                                ];
                            }
                        }
                    }
                }
            }
        }

        return best;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static bool CanPlace(Board board, PieceShape piece, int row, int col)
    {
        foreach (var (dr, dc) in piece.Cells)
        {
            int r = row + dr, c = col + dc;
            if (r < 0 || r >= Board.Size || c < 0 || c >= Board.Size) return false;
            if (!board.IsCellEmpty(r, c)) return false;
        }
        return true;
    }

    private static void Fill(Board board, PieceShape piece, int row, int col)
    {
        foreach (var (dr, dc) in piece.Cells)
            board.FillCell(row + dr, col + dc, piece.ColorKey);
    }

    private static int CountFilled(Board board)
    {
        int count = 0;
        for (int r = 0; r < Board.Size; r++)
            for (int c = 0; c < Board.Size; c++)
                if (!board.IsCellEmpty(r, c)) count++;
        return count;
    }
}
