using BlockudokuGame.Models;

namespace BlockudokuGame.Engine;

public record ClearResult(
    int RowsCleared,
    int ColsCleared,
    int BoxesCleared
)
{
    public int TotalClearedRegions => RowsCleared + ColsCleared + BoxesCleared;

    public int TotalClearedCells =>
        RowsCleared * Board.Size +
        ColsCleared * Board.Size +
        BoxesCleared * Board.BoxSize * Board.BoxSize;
}

public class ClearingEngine
{
    public ClearResult ClearCompleted(Board board)
    {
        var rowsToClear  = new List<int>();
        var colsToClear  = new List<int>();
        var boxesToClear = new List<(int br, int bc)>();

        for (int r = 0; r < Board.Size; r++)
            if (board.IsRowFull(r)) rowsToClear.Add(r);

        for (int c = 0; c < Board.Size; c++)
            if (board.IsColumnFull(c)) colsToClear.Add(c);

        for (int br = 0; br < Board.BoxSize; br++)
            for (int bc = 0; bc < Board.BoxSize; bc++)
                if (board.IsBoxFull(br, bc)) boxesToClear.Add((br, bc));

        // Collect all cells to clear into a set to handle overlaps (row + box sharing cells)
        var cellsToClear = new HashSet<(int r, int c)>();

        foreach (int r in rowsToClear)
            for (int c = 0; c < Board.Size; c++)
                cellsToClear.Add((r, c));

        foreach (int c in colsToClear)
            for (int r = 0; r < Board.Size; r++)
                cellsToClear.Add((r, c));

        foreach (var (br, bc) in boxesToClear)
        {
            int sr = br * Board.BoxSize, sc = bc * Board.BoxSize;
            for (int r = sr; r < sr + Board.BoxSize; r++)
                for (int c = sc; c < sc + Board.BoxSize; c++)
                    cellsToClear.Add((r, c));
        }

        foreach (var (r, c) in cellsToClear)
            board.ClearCell(r, c);

        return new ClearResult(rowsToClear.Count, colsToClear.Count, boxesToClear.Count);
    }
}
