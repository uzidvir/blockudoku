namespace BlockudokuGame.Models;

public class Board
{
    public const int Size = 9;
    public const int BoxSize = 3;

    private readonly CellState[,] _cells = new CellState[Size, Size];
    private readonly string?[,] _cellColors = new string?[Size, Size];

    public CellState GetCell(int row, int col) => _cells[row, col];
    public string? GetCellColor(int row, int col) => _cellColors[row, col];

    public bool IsCellEmpty(int row, int col) => _cells[row, col] == CellState.Empty;

    public void FillCell(int row, int col, string colorKey)
    {
        _cells[row, col] = CellState.Filled;
        _cellColors[row, col] = colorKey;
    }

    public void ClearCell(int row, int col)
    {
        _cells[row, col] = CellState.Empty;
        _cellColors[row, col] = null;
    }

    public bool IsRowFull(int row)
    {
        for (int c = 0; c < Size; c++)
            if (_cells[row, c] != CellState.Filled) return false;
        return true;
    }

    public bool IsColumnFull(int col)
    {
        for (int r = 0; r < Size; r++)
            if (_cells[r, col] != CellState.Filled) return false;
        return true;
    }

    // boxRow, boxCol: 0..2 (which of the nine 3x3 boxes)
    public bool IsBoxFull(int boxRow, int boxCol)
    {
        int startRow = boxRow * BoxSize;
        int startCol = boxCol * BoxSize;
        for (int r = startRow; r < startRow + BoxSize; r++)
            for (int c = startCol; c < startCol + BoxSize; c++)
                if (_cells[r, c] != CellState.Filled) return false;
        return true;
    }

    public Board Clone()
    {
        var clone = new Board();
        Array.Copy(_cells, clone._cells, _cells.Length);
        Array.Copy(_cellColors, clone._cellColors, _cellColors.Length);
        return clone;
    }
}
