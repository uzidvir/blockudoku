using BlockudokuGame.Models;

namespace BlockudokuGame.Engine;

public class ScoreEngine
{
    private const int PointsPerCell        = 1;
    private const int PointsPerClearedCell = 2;
    private const int ComboBonus           = 10;

    public int Calculate(PieceShape piece, ClearResult clear)
    {
        int placementScore = piece.Cells.Length * PointsPerCell;
        int clearScore     = clear.TotalClearedCells * PointsPerClearedCell;
        int comboScore     = Math.Max(0, clear.TotalClearedRegions - 1) * ComboBonus;
        return placementScore + clearScore + comboScore;
    }
}
