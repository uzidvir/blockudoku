namespace BlockudokuGame.Models;

public record PlacementResult(
    bool Success,
    int RowsCleared,
    int ColsCleared,
    int BoxesCleared,
    int ScoreDelta,
    int ComboCount
);
