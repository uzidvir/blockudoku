using BlockudokuGame.Models;
using BlockudokuGame.Rendering;

namespace BlockudokuGame.UI;

public class ScorePanel : Panel
{
    private readonly GameState _state;

    private static readonly Font LabelFont = new("Segoe UI", 10f, FontStyle.Regular);
    private static readonly Font ValueFont = new("Segoe UI", 22f, FontStyle.Bold);

    public ScorePanel(GameState state)
    {
        _state = state;
        DoubleBuffered = true;
        SetStyle(ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.AllPaintingInWmPaint  |
                 ControlStyles.UserPaint, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.Clear(ColorTheme.Background);

        // Score (left)
        DrawScoreBlock(g, "SCORE", _state.Score.ToString(), Width / 4, Height / 2);

        // Best (right)
        DrawScoreBlock(g, "BEST", _state.HighScore.ToString(), Width * 3 / 4, Height / 2);
    }

    private static void DrawScoreBlock(Graphics g, string label, string value, int cx, int cy)
    {
        // Label
        using var labelBrush = new SolidBrush(ColorTheme.LabelText);
        var labelSize = g.MeasureString(label, LabelFont);
        g.DrawString(label, LabelFont, labelBrush,
            cx - labelSize.Width / 2, cy - labelSize.Height - 2);

        // Value
        using var valueBrush = new SolidBrush(ColorTheme.ScoreText);
        var valueSize = g.MeasureString(value, ValueFont);
        g.DrawString(value, ValueFont, valueBrush,
            cx - valueSize.Width / 2, cy - valueSize.Height / 2 + 4);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            LabelFont.Dispose();
            ValueFont.Dispose();
        }
        base.Dispose(disposing);
    }
}
