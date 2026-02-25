using BlockudokuGame.Engine;
using BlockudokuGame.Models;
using BlockudokuGame.Rendering;

namespace BlockudokuGame.UI;

public class MainForm : Form
{
    private readonly GameState   _state      = new();
    private readonly GameEngine  _engine;
    private readonly HintEngine  _hintEngine = new();

    private readonly ScorePanel _scorePanel;
    private readonly BoardPanel _boardPanel;
    private readonly TrayPanel  _trayPanel;
    private          Button     _hintButton = null!;
    private          Button     _autoButton = null!;

    // ── Auto-play ─────────────────────────────────────────────────────────────
    private readonly System.Windows.Forms.Timer _autoTimer = new() { Interval = 50 };
    private          HintMove[]? _autoMoves;
    private          int     _autoMoveIndex;
    private          bool    _autoShowPhase = true;   // true = SHOW_HINT, false = EXECUTE
    private          bool    _isAutoPlaying;

    private const int AutoShowHintMs = 1000;   // hint overlay visible duration
    private const int AutoExecuteMs  =  600;   // pause between individual move executions

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

        _autoTimer.Tick += OnAutoPlayTick;

        _engine.NewGame();
        RefreshAll();
    }

    private void SetupLayout()
    {
        Text            = "Blockudoku";
        ClientSize      = new Size(BoardSize + 40, BoardSize + 284);
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

        // Two buttons (Hint + Auto) centred below the tray
        // Each 80×30, 10 px gap → total 170 px; centre offset = (BoardSize − 170) / 2 = 95
        int btnY    = 82 + BoardSize + 12 + 140 + 8;
        int btnLeft = 20 + (BoardSize - 170) / 2;

        _hintButton = MakeButton("Hint", btnLeft,       btnY);
        _autoButton = MakeButton("Auto", btnLeft + 90,  btnY);

        _hintButton.Click += OnHintClicked;
        _autoButton.Click += OnAutoClicked;

        Controls.AddRange(new Control[] { _scorePanel, _boardPanel, _trayPanel,
                                          _hintButton, _autoButton });
    }

    private static Button MakeButton(string text, int x, int y) =>
        new Button
        {
            Text      = text,
            Size      = new Size(80, 30),
            Location  = new Point(x, y),
            FlatStyle = FlatStyle.Flat,
            BackColor = ColorTheme.Background,
            ForeColor = Color.FromArgb(30, 35, 65),
            Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
            Cursor    = Cursors.Hand,
        }.Also(b => b.FlatAppearance.BorderColor = Color.FromArgb(180, 180, 180));

    private void WireEvents()
    {
        _trayPanel.DragStarted  += OnTrayDragStarted;
        _boardPanel.PiecePlaced += OnPiecePlaced;
        _boardPanel.DragEnded   += OnDragEnded;
    }

    private void OnTrayDragStarted(int trayIndex, int pickRow, int pickCol)
    {
        if (_isAutoPlaying) return;                 // block manual drag during auto-play

        _state.DraggingIndex = trayIndex;
        _state.DragPickRow   = pickRow;
        _state.DragPickCol   = pickCol;
        _boardPanel.BeginDrag(trayIndex);
        _trayPanel.Capture = false;
        _boardPanel.Capture = true;
        ClearHint();
    }

    private void OnPiecePlaced(PlacementResult result)
    {
        if (result.Success) ClearHint();
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
        if (_state.HintActive) { ClearHint(); return; }

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

    // ── Auto-play ─────────────────────────────────────────────────────────────

    private void OnAutoClicked(object? sender, EventArgs e)
    {
        if (_state.Phase != GamePhase.Playing) return;
        if (_isAutoPlaying) StopAutoPlay();
        else StartAutoPlay();
    }

    private void StartAutoPlay()
    {
        _isAutoPlaying  = true;
        _autoShowPhase  = true;
        _autoMoves      = null;
        _autoMoveIndex  = 0;
        _autoTimer.Interval = 50;       // fire almost immediately on first tick
        _autoTimer.Start();

        _autoButton.Text      = "Stop";
        _autoButton.BackColor = Color.FromArgb(255, 200, 0);    // amber while active
        _hintButton.Enabled   = false;
    }

    private void StopAutoPlay()
    {
        _autoTimer.Stop();
        _isAutoPlaying = false;
        _autoMoves     = null;

        _autoButton.Text      = "Auto";
        _autoButton.BackColor = ColorTheme.Background;
        _hintButton.Enabled   = true;
        ClearHint();
    }

    private void OnAutoPlayTick(object? sender, EventArgs e)
    {
        if (_autoShowPhase)
        {
            // ── Phase 1: compute best sequence and show hint ───────────────
            var moves = _hintEngine.FindBest(_state);
            if (moves is null || moves.Length == 0 || _state.Phase != GamePhase.Playing)
            {
                StopAutoPlay();
                return;
            }

            _autoMoves     = moves;
            _autoMoveIndex = 0;

            for (int i = 0; i < moves.Length; i++)
                _state.HintMoves[i] = moves[i];
            RefreshAll();

            _autoShowPhase      = false;
            _autoTimer.Interval = AutoShowHintMs;
        }
        else
        {
            // ── Phase 2: execute the next move in the sequence ─────────────
            var move   = _autoMoves![_autoMoveIndex++];
            _engine.TryPlace(move.TrayIndex, move.Row, move.Col);

            // Shift hint overlays forward (remove the move just executed)
            for (int i = 0; i < _state.HintMoves.Length - 1; i++)
                _state.HintMoves[i] = _state.HintMoves[i + 1];
            _state.HintMoves[_state.HintMoves.Length - 1] = null;
            RefreshAll();

            if (_state.Phase == GamePhase.GameOver)
            {
                StopAutoPlay();
                SaveHighScore();
                ShowGameOverDialog();
                return;
            }

            bool roundDone      = _autoMoveIndex >= _autoMoves.Length;
            _autoShowPhase      = roundDone;
            _autoTimer.Interval = roundDone ? AutoShowHintMs : AutoExecuteMs;
        }
    }

    // ── Misc ──────────────────────────────────────────────────────────────────

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

    // ── High Score Persistence ────────────────────────────────────────────────

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
        _autoTimer.Stop();
        SaveHighScore();
        base.OnFormClosing(e);
    }
}

// Small extension so FlatAppearance can be set inline in an expression
file static class ButtonExt
{
    internal static Button Also(this Button b, Action<Button> configure)
    {
        configure(b);
        return b;
    }
}
