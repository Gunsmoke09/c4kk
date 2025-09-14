using System;
using System.Text; 

namespace LineUp
{
    public class GameState
    {
        public int Rows { get; set; }
        public int Cols { get; set; }
        public int WinLength { get; set; } = 4;  // Default to 4, but it will be recalculate based on the grid size change
        public GameMode Mode { get; set; } = GameMode.HvH;
        public PlayerId CurrentTurn { get; set; } = PlayerId.P1;

        public Cell[][] Grid { get; set; } = Array.Empty<Cell[]>();
        public Player P1 { get; set; } = new Player(PlayerId.P1);
        public Player P2 { get; set; } = new Player(PlayerId.P2);

        public bool GameOver { get; set; }
        public PlayerId Winner { get; set; } = PlayerId.None;

        // Create a new game with the given board size and mode.
        public static GameState CreateNew(int rows, int cols, GameMode mode)
        {
            var gs = new GameState
            {
                Rows = rows,
                Cols = cols,
                Mode = mode,
                Grid = new Cell[rows][]
            };
            // Build the grid with loops
            for (int r = 0; r < rows; r++)
            {
                gs.Grid[r] = new Cell[cols];
                for (int c = 0; c < cols; c++)
                    gs.Grid[r][c] = new Cell();
            }
            // Calculate the win length based on grid size
            gs.WinLength = ComputeWinLength(rows, cols);
            // Initialize discs for both players
            int totalCells = rows * cols;
            int perPlayer = totalCells / 2;
            int specials = 2 + 2; // 2 boring + 2 magnetic
            gs.P1.BoringCount = 2; gs.P2.BoringCount = 2;
            gs.P1.MagneticCount = 2; gs.P2.MagneticCount = 2;
            gs.P1.OrdinaryCount = Math.Max(0, perPlayer - specials);
            gs.P2.OrdinaryCount = Math.Max(0, perPlayer - specials);
            gs.P1.MaxBoring = 2; gs.P2.MaxBoring = 2;
            gs.P1.MaxMagnetic = 2; gs.P2.MaxMagnetic = 2;
            gs.CurrentTurn = PlayerId.P1; // Player 1 starts
            return gs;
        }

        // Calculate the win length based on grid size
        public static int ComputeWinLength(int rows, int cols)
        {
            // Calculate the total number of cells in the grid
            int totalCells = rows * cols;
            // Calculate the win length as 10% of the total cells, rounded to the nearest integer
            int winLength = (int)Math.Round(totalCells * 0.1);
            // Ensure that the win length is at least 4
            return Math.Max(winLength, 4);
        }

        public Player CurrentPlayer => CurrentTurn == PlayerId.P1 ? P1 : P2;
        public Player OtherPlayer => CurrentTurn == PlayerId.P1 ? P2 : P1;

        // True if the top cell of column is occupied.
        public bool ColumnFull(int col) => Grid[Rows - 1][col].Owner != PlayerId.None;

        // Returns the lowest empty row index for a column, or -1 if none.
        public int DropToRow(int col)
        {
            for (int r = 0; r < Rows; r++)
                if (Grid[r][col].IsEmpty) return r;
            return -1;  // Column is full if no empty cell is found
        }

        // Returns the display character at the grid.
        public char RenderCharAt(int r, int c)
        {
            var cell = Grid[r][c];
            if (cell.IsEmpty) return ' ';
            var owner = cell.Owner == PlayerId.P1 ? P1 : P2;
            return owner.ToChar(cell.Kind);
        }

        // Appends the current grid to a StringBuilder using the | x | layout.
        public void Render(StringBuilder sb)
        {
            for (int r = Rows - 1; r >= 0; r--)
            {
                for (int c = 0; c < Cols; c++)
                {
                    char ch = RenderCharAt(r, c);
                    sb.Append('|').Append(' ').Append(ch).Append(' ');
                }
                sb.Append('|').AppendLine();
            }
        }
    }
}
