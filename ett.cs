using System;
using System.Drawing;
using System.Windows.Forms;

public class TetrisGame : Form
{
    private const int GridWidth = 10;
    private const int GridHeight = 20;
    private const int BlockSize = 30;

    private int[,] grid = new int[GridWidth, GridHeight];
    private Timer gameTimer;
    private Tetromino currentTetromino;

    public TetrisGame()
    {
        this.Text = "Tetris";
        this.Size = new Size(GridWidth * BlockSize + 16, GridHeight * BlockSize + 39);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;
        
        this.gameTimer = new Timer();
        this.gameTimer.Interval = 500; // Game speed, milliseconds
        this.gameTimer.Tick += (sender, e) => UpdateGame();
        this.gameTimer.Start();
        
        this.KeyDown += OnKeyDown;
        NewTetromino();
    }

    private void NewTetromino()
    {
        currentTetromino = Tetromino.GetRandomTetromino();
    }

    private void UpdateGame()
    {
        if (!MoveTetromino(0, 1))
        {
            PlaceTetromino();
            ClearLines();
            NewTetromino();
        }

        Invalidate(); // Redraw the form
    }

    private void PlaceTetromino()
    {
        foreach (var block in currentTetromino.GetBlocks())
        {
            int x = block.X + currentTetromino.X;
            int y = block.Y + currentTetromino.Y;

            if (y >= 0 && y < GridHeight && x >= 0 && x < GridWidth)
                grid[x, y] = 1;
        }
    }

    private void ClearLines()
    {
        for (int y = GridHeight - 1; y >= 0; y--)
        {
            bool fullLine = true;
            for (int x = 0; x < GridWidth; x++)
            {
                if (grid[x, y] == 0)
                {
                    fullLine = false;
                    break;
                }
            }

            if (fullLine)
            {
                // Clear the line
                for (int moveY = y; moveY > 0; moveY--)
                    for (int x = 0; x < GridWidth; x++)
                        grid[x, moveY] = grid[x, moveY - 1];

                // Reset the top line
                for (int x = 0; x < GridWidth; x++)
                    grid[x, 0] = 0;

                y++; // Recheck the same row
            }
        }
    }

    private bool MoveTetromino(int dx, int dy)
    {
        // Check if the move is valid
        foreach (var block in currentTetromino.GetBlocks())
        {
            int newX = block.X + currentTetromino.X + dx;
            int newY = block.Y + currentTetromino.Y + dy;

            if (newX < 0 || newX >= GridWidth || newY >= GridHeight || (newY >= 0 && grid[newX, newY] != 0))
                return false;
        }

        currentTetromino.X += dx;
        currentTetromino.Y += dy;
        return true;
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Left:
                MoveTetromino(-1, 0);
                break;
            case Keys.Right:
                MoveTetromino(1, 0);
                break;
            case Keys.Down:
                UpdateGame();
                break;
            case Keys.Up:
                RotateTetromino();
                break;
        }
        Invalidate();
    }

    private void RotateTetromino()
    {
        currentTetromino.Rotate();
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        // Draw grid
        for (int x = 0; x < GridWidth; x++)
            for (int y = 0; y < GridHeight; y++)
                if (grid[x, y] == 1)
                    e.Graphics.FillRectangle(Brushes.Blue, x * BlockSize, y * BlockSize, BlockSize, BlockSize);

        // Draw the current tetromino
        foreach (var block in currentTetromino.GetBlocks())
        {
            int x = block.X + currentTetromino.X;
            int y = block.Y + currentTetromino.Y;
            e.Graphics.FillRectangle(Brushes.Red, x * BlockSize, y * BlockSize, BlockSize, BlockSize);
        }
    }

    public static void Main()
    {
        Application.Run(new TetrisGame());
    }
}

public class Tetromino
{
    public int X { get; set; }
    public int Y { get; set; }
    private readonly Point[] blocks;

    private Tetromino(Point[] blocks)
    {
        this.blocks = blocks;
        X = 4; // Initial horizontal position
        Y = 0; // Start at the top
    }

    public static Tetromino GetRandomTetromino()
    {
        Random rand = new Random();
        switch (rand.Next(7))
        {
            case 0: return new Tetromino(new Point[] { new Point(0, 0), new Point(1, 0), new Point(2, 0), new Point(3, 0) }); // I
            case 1: return new Tetromino(new Point[] { new Point(0, 0), new Point(1, 0), new Point(1, 1), new Point(2, 1) }); // Z
            case 2: return new Tetromino(new Point[] { new Point(0, 0), new Point(1, 0), new Point(2, 0), new Point(2, 1) }); // L
            case 3: return new Tetromino(new Point[] { new Point(0, 0), new Point(1, 0), new Point(2, 0), new Point(1, 1) }); // T
            case 4: return new Tetromino(new Point[] { new Point(0, 0), new Point(0, 1), new Point(1, 0), new Point(1, 1) }); // O
            case 5: return new Tetromino(new Point[] { new Point(0, 1), new Point(1, 1), new Point(1, 0), new Point(2, 0) }); // S
            case 6: return new Tetromino(new Point[] { new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(1, 2) }); // J
            default: throw new InvalidOperationException();
        }
    }

    public void Rotate()
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            int temp = blocks[i].X;
            blocks[i].X = -blocks[i].Y;
            blocks[i].Y = temp;
        }
    }

    public Point[] GetBlocks() => blocks;
}
