using BlockudokuGame.Engine;
using BlockudokuGame.Models;
using BlockudokuGame.Rendering;

namespace BlockudokuGame.UI;

public class MainForm : Form
{
    private readonly GameState   _state  = new();
    private readonly GameEngine  _engine;
    private readonly HintEngine  _hintEngine = new();

    private readonly ScorePanel _scorePanel;
    private readonly BoardPanel _boardPanel;
    private readonly TrayPanel  _trayPanel;
    private          Button     _hintButton = null!;

    private const int BoardSize = BoardRenderer.CellSize * Board.Size;

    public MainForm()
    {
        _engine = new GameEngine(_state);

        _scorePanel = new ScorePanel(_state);
        _boardPanel = new BoardPanel(_state, _engine);
        _trayPanel  = new TrayPanel(_state);

        SetupLayout();
        WireEvents();
        LoadHighScore();

        _engine.NewGame();
        RefreshAll();
    }

    private void SetupLayout()
    {
        Text            = "Blockudoku";
        ClientSize      = new Size(BoardSize + 40, BoardSize + 284);  // +45 for hint button row
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        BackColor       = ColorTheme.Background;
        Font            = new Font("Segoe UI", 9f);

        _scorePanel.Location = new Point(20, 10);
        _scorePanel.Size     = new Size(BoardSize, 65);

        _boardPanel.Location = new Point(20, 82);
        _boardPanel.Size     = new Size(BoardSize, BoardSize);

        _trayPanel.Location = new Point(20, 82 + BoardSize + 12);
        _trayPanel.Size     = new Size(BoardSize, 140);

        // Hint button — centred below the tray
        _hintButton = new Button
        {
            Text      = "Hint",
            Size      = new Size(80, 30),
            Location  = new Point(20 + (BoardSize - 80) / 2, 82 + BoardSize + 12 + 140 + 8),
            FlatStyle = FlatStyle.Flat,
            BackColor = ColorTheme.Background,
            ForeColor = Color.FromArgb(30, 35, 65),
            Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
            Cursor    = Cursors.Hand,
        };
        _hintButton.FlatAppearance.BorderColor = Color.FromArgb(180, 180, 180);
        _hintButton.Click += OnHintClicked;

        Controls.AddRange(new Control[] { _scorePanel, _boardPanel, _trayPanel, _hintButton });
    }

    private void WireEvents()
    {
        _trayPanel.DragStarted  += OnTrayDragStarted;
        _boardPanel.PiecePlaced += OnPiecePlaced;
        _boardPanel.DragEnded   += OnDragEnded;
    }

    private void OnTrayDragStarted(int trayIndex, int pickRow, int pickCol)
    {
        _state.DraggingIndex = trayIndex;
        _state.DragPickRow   = pickRow;
        _state.DragPickCol   = pickCol;
        _boardPanel.BeginDrag(trayIndex);
        _trayPanel.Capture = false;         // Release tray capture so board panel can receive events
        _boardPanel.Capture = true;         // Board panel takes over capture
        ClearHint();                        // Hide hint while dragging
    }

    private void OnPiecePlaced(PlacementResult result)
    {
        if (result.Success) ClearHint();    // Board changed — hint is stale
        _scorePanel.Invalidate();
        _trayPanel.Invalidate();

        if (result.Success && _state.Phase == GamePhase.GameOver)
        {
            SaveHighScore();
            ShowGameOverDialog();
        }
    }

    // ── Hint ─────────────────────────────────────────────────────────────────

    private void OnHintClicked(object? sender, EventArgs e)
    {
        if (_state.Phase != GamePhase.Playing) return;

        if (_state.HintActive) { ClearHint(); return; }    // toggle off

        var moves = _hintEngine.FindBest(_state);
        if (moves is null) return;

        for (int i = 0; i < moves.Length; i++)
            _state.HintMoves[i] = moves[i];

        _boardPanel.Invalidate();
        _trayPanel.Invalidate();
    }

    private void ClearHint()
    {
        _state.ClearHint();
        _boardPanel.Invalidate();
        _trayPanel.Invalidate();
    }

    private void OnDragEnded()
    {
        _trayPanel.Invalidate();
        _boardPanel.Invalidate();
    }

    private void RefreshAll()
    {
        _scorePanel.Invalidate();
        _boardPanel.Invalidate();
        _trayPanel.Invalidate();
    }

    // Forward mouse move to board panel while board has capture (drag in progress)
    protected override void WndProc(ref Message m)
    {
        const int WM_MOUSEMOVE = 0x0200;

        if (m.Msg == WM_MOUSEMOVE && _state.DraggingIndex >= 0 && _boardPanel.Capture)
        {
            var screenPt = Cursor.Position;
            var boardPt  = _boardPanel.PointToClient(screenPt);
            _boardPanel.UpdateDrag(boardPt);
        }

        base.WndProc(ref m);
    }

    private void ShowGameOverDialog()
    {
        // Small delay so the last piece render is visible before the dialog
        Application.DoEvents();

        var dlgResult = MessageBox.Show(
            $"Game Over!\n\nFinal Score: {_state.Score}\nBest: {_state.HighScore}\n\nPlay again?",
            "Blockudoku",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Information);

        if (dlgResult == DialogResult.Yes)
        {
            _engine.NewGame();
            RefreshAll();
        }
        else
        {
            Close();
        }
    }

    // ── High Score Persistence ────────────────────────────────────────────

    private static string HighScorePath =>
        Path.Combine(Application.UserAppDataPath, "highscore.txt");

    private void LoadHighScore()
    {
        try
        {
            if (File.Exists(HighScorePath))
                _state.HighScore = int.Parse(File.ReadAllText(HighScorePath).Trim());
        }
        catch { /* ignore corrupt / missing file */ }
    }

    private void SaveHighScore()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(HighScorePath)!);
            File.WriteAllText(HighScorePath, _state.HighScore.ToString());
        }
        catch { /* ignore write errors */ }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        SaveHighScore();
        base.OnFormClosing(e);
    }
}
