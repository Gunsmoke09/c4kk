using System;
using System.Collections.Generic;

namespace LineUp
{
    public static class GameMethods
    {
        private static readonly Random Rng = new Random();

        public static Action<string, GameState>? FramePrinter { get; set; }

        // Apply a move for the current player.
        public static bool ApplyMove(GameState gs, DiscKind kind, int col)
{
    if (col < 0 || col >= gs.Cols) { Console.WriteLine("Column out of range."); return false; }
    if (gs.ColumnFull(col)) { Console.WriteLine("That column is full."); return false; }

    var player = gs.CurrentPlayer;
    if (!HasDisc(player, kind))
    {
        Console.WriteLine("You don't have that disc available.");
        return false;
    }

    int row = gs.DropToRow(col);
    if (row < 0) { Console.WriteLine("No space in column."); return false; }

    Place(gs, row, col, player.Id, kind);

    // Apply gravity after placing a disc
    ApplyGravityColumn(gs, col);  // Ensure gravity is applied

    PrintFrame("Ordinary disc used", gs);

    switch (kind)
    {
        case DiscKind.Ordinary:
            break;

        case DiscKind.Boring:
            BoreColumn(gs, col, player);
            PrintFrame("Boring disc used", gs);
            gs.Grid[0][col].Kind = DiscKind.Ordinary;
            break;

        case DiscKind.Magnetic:
            MagneticEffect(gs, col, player);
            ApplyGravityColumn(gs, col);
            PrintFrame("Magnetic disc used", gs);
            break;
    }

    SpendDisc(player, kind);

    if (IsWin(gs, player.Id))
    {
        gs.GameOver = true;
        gs.Winner = player.Id;
    }
    else if (IsTie(gs))
    {
        gs.GameOver = true;
    }
    else
    {
        gs.CurrentTurn = gs.CurrentTurn == PlayerId.P1 ? PlayerId.P2 : PlayerId.P1;
    }

    return true;
}



        public static void Place(GameState gs, int row, int col, PlayerId pid, DiscKind kind)
        {
            var cell = gs.Grid[row][col];
            cell.Owner = pid;
            cell.Kind = kind;
        }

        public static void RemoveAt(GameState gs, int row, int col)
        {
            gs.Grid[row][col] = new Cell();
        }

        // Boring disc: clears a column and refunds discs to owners, leaves boring disc at bottom.
        public static void BoreColumn(GameState gs, int col, Player actor)
        {
            for (int r = 0; r < gs.Rows; r++)
            {
                var cell = gs.Grid[r][col];
                if (!cell.IsEmpty)
                {
                    RefundDisc(gs, cell.Owner, cell.Kind);
                    gs.Grid[r][col] = new Cell();
                }
            }
            gs.Grid[0][col].Owner = actor.Id;
            gs.Grid[0][col].Kind = DiscKind.Boring;
        }

        
        public static void MagneticEffect(GameState gs, int col, Player actor)
        {
            // Loop through the grid from bottom up to find the nearest ordinary disc
            for (int r = gs.Rows - 1; r > 0; r--)  // Starting from second-to-last row
            {
                var cell = gs.Grid[r][col];

                // If the current cell is not empty and belongs to the current player
                if (!cell.IsEmpty && cell.Owner == actor.Id && cell.Kind == DiscKind.Ordinary)
                {
                    // Check if the nearest ordinary disc is directly below the magnetic disc
                    if (gs.Grid[r - 1][col].IsEmpty)
                    {
                        // Lift the ordinary disc one row upwards
                        gs.Grid[r - 1][col] = cell;
                        gs.Grid[r][col] = new Cell();  // Empty the original cell

                        break;  // Only lift one disc, exit the loop after lifting
                    }
                    else
                    {
                        break;  // No effect if the nearest disc is immediately below
                    }
                }
            }

            // After the effect, the magnetic disc becomes an ordinary disc
            gs.Grid[gs.Rows - 1][col].Kind = DiscKind.Ordinary;
        }

        public static void ApplyGravityColumn(GameState gs, int col)
{
    int write = 0;
    for (int r = 0; r < gs.Rows; r++)
    {
        if (!gs.Grid[r][col].IsEmpty)
        {
            if (write != r)
            {
                gs.Grid[write][col] = gs.Grid[r][col];
                gs.Grid[r][col] = new Cell();
            }
            write++;
        }
    }
}


        public static bool IsTie(GameState gs)
        {
            for (int c = 0; c < gs.Cols; c++)
                if (!gs.ColumnFull(c)) return false;
            return true;
        }

        public static bool IsWin(GameState gs, PlayerId pid)
        {
            int need = gs.WinLength;
            for (int r = 0; r < gs.Rows; r++)
            {
                for (int c = 0; c < gs.Cols; c++)
                {
                    if (gs.Grid[r][c].Owner != pid) continue;
                    if (CountDir(gs, r, c, 0, 1, pid) >= need) return true;   // →
                    if (CountDir(gs, r, c, 1, 0, pid) >= need) return true;   // ↑
                    if (CountDir(gs, r, c, 1, 1, pid) >= need) return true;   // ↗
                    if (CountDir(gs, r, c, 1, -1, pid) >= need) return true;  // ↖
                }
            }
            return false;
        }

        private static int CountDir(GameState gs, int r, int c, int dr, int dc, PlayerId pid)
        {
            int cnt = 0;
            while (r >= 0 && r < gs.Rows && c >= 0 && c < gs.Cols && gs.Grid[r][c].Owner == pid)
            {
                cnt++;
                r += dr; c += dc;
            }
            return cnt;
        }

        public static bool HasDisc(Player p, DiscKind kind)
        {
            switch (kind)
            {
                case DiscKind.Ordinary: return p.OrdinaryCount > 0;
                case DiscKind.Boring: return p.BoringCount > 0;
                case DiscKind.Magnetic: return p.MagneticCount > 0;
                default: return false;
            }
        }

        public static void SpendDisc(Player p, DiscKind kind)
        {
            switch (kind)
            {
                case DiscKind.Ordinary: p.OrdinaryCount--; break;
                case DiscKind.Boring: p.BoringCount--; break;
                case DiscKind.Magnetic: p.MagneticCount--; break;
            }
        }

        public static void RefundDisc(GameState gs, PlayerId owner, DiscKind kind)
        {
            if (owner == PlayerId.None) return;
            var p = owner == PlayerId.P1 ? gs.P1 : gs.P2;
            switch (kind)
            {
                case DiscKind.Ordinary:
                    // Ordinary discs don’t have a fixed max
                    p.OrdinaryCount++;
                    break;

                case DiscKind.Boring:
                    if (p.BoringCount < p.MaxBoring)
                        p.BoringCount++;
                    break;

                case DiscKind.Magnetic:
                    if (p.MagneticCount < p.MaxMagnetic)
                        p.MagneticCount++;
                    break;
            }
        }


        //computer try immediate win, else random valid move.
        public static void AITurn(GameState gs)
        {
            var choices = new List<(DiscKind kind, int col)>();
            for (int c = 0; c < gs.Cols; c++)
            {
                if (gs.ColumnFull(c)) continue;
                if (gs.P2.OrdinaryCount > 0) choices.Add((DiscKind.Ordinary, c));
                if (gs.P2.BoringCount > 0) choices.Add((DiscKind.Boring, c));
                if (gs.P2.MagneticCount > 0) choices.Add((DiscKind.Magnetic, c));
            }

            // Try immediate win
            foreach (var move in choices)
            {
                if (WouldWin(gs, PlayerId.P2, move.kind, move.col))
                {
                    ApplyMove(gs, move.kind, move.col);
                    return;
                }
            }

            // Else random
            if (choices.Count == 0) { gs.GameOver = true; return; }
            var pick = choices[Rng.Next(choices.Count)];
            ApplyMove(gs, pick.kind, pick.col);
        }

        public static bool WouldWin(GameState gs, PlayerId pid, DiscKind kind, int col)
        {
            var clone = Clone(gs);
            clone.CurrentTurn = pid;
            if (!ApplyMoveNoFrames(clone, kind, col)) return false;
            return clone.GameOver && clone.Winner == pid;
        }

        public static GameState Clone(GameState gs)
        {
            var copy = new GameState
            {
                Rows = gs.Rows,
                Cols = gs.Cols,
                WinLength = gs.WinLength,
                Mode = gs.Mode,
                CurrentTurn = gs.CurrentTurn,
                GameOver = gs.GameOver,
                Winner = gs.Winner,
                P1 = new Player(PlayerId.P1)
                {
                    OrdinaryCount = gs.P1.OrdinaryCount,
                    BoringCount = gs.P1.BoringCount,
                    MagneticCount = gs.P1.MagneticCount,
                },
                P2 = new Player(PlayerId.P2)
                {
                    OrdinaryCount = gs.P2.OrdinaryCount,
                    BoringCount = gs.P2.BoringCount,
                    MagneticCount = gs.P2.MagneticCount,
                },
                Grid = new Cell[gs.Rows][]
            };

            for (int r = 0; r < gs.Rows; r++)
            {
                copy.Grid[r] = new Cell[gs.Cols];
                for (int c = 0; c < gs.Cols; c++)
                {
                    var src = gs.Grid[r][c];
                    copy.Grid[r][c] = new Cell
                    {
                        Owner = src.Owner,
                        Kind = src.Kind
                    };
                }
            }

            return copy;
        }

        public static bool ApplyMoveNoFrames(GameState gs, DiscKind kind, int col)
        {
            if (col < 0 || col >= gs.Cols || gs.ColumnFull(col)) return false;
            var player = gs.CurrentPlayer;
            if (!HasDisc(player, kind)) return false;
            int row = gs.DropToRow(col);
            if (row < 0) return false;

            Place(gs, row, col, player.Id, kind);
            ApplyGravityColumn(gs, col);
            switch (kind)
            {
                case DiscKind.Boring:
                    BoreColumn(gs, col, player);
                    gs.Grid[0][col].Kind = DiscKind.Ordinary;
                    break;
                case DiscKind.Magnetic:
                    MagneticEffect(gs, col, player);
                    ApplyGravityColumn(gs, col);
                    break;
            }
            SpendDisc(player, kind);

            if (IsWin(gs, player.Id)) { gs.GameOver = true; gs.Winner = player.Id; }
            else if (IsTie(gs)) gs.GameOver = true;
            else gs.CurrentTurn = gs.CurrentTurn == PlayerId.P1 ? PlayerId.P2 : PlayerId.P1;

            return true;
        }

        private static void PrintFrame(string title, GameState gs)
        {
            FramePrinter?.Invoke(title, gs);
        }
    }
}
