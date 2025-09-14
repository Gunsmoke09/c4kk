using System;

namespace LineUp
{
    public static class TestRunner
    {
        public static void RunTestingMode(string line)
        {
            // Determine required board size based on moves
            var idv_moves = line.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            int maxCol = 7;
            foreach (var m in idv_moves)
            {
                if (Parser.TryParseMove(new[] { m }, out _, out var c))
                    if (c + 1 > maxCol) maxCol = c + 1;
            }

            // testing game default setting (6 rows, dynamic columns and only human vs human)
            var gs = GameState.CreateNew(6, maxCol, GameMode.HvH);
            Console.WriteLine("Testing mode: parsing moves order. P1 starts the move first.");
            Console.WriteLine($"Input: {line}");

            foreach (var move in idv_moves)
            {
                // Parse each move into a disc type and column index
                if (!Parser.TryParseMove(new[] { move }, out var kind, out var col))
                {
                    Console.WriteLine($"Invalid Input '{move}'. Please enter again.");
                    break;
                }

                // Apply the move and check if it's valid, eg exceed the col
                if (!GameMethods.ApplyMoveNoFrames(gs, kind, col))
                {
                    Console.WriteLine($"Invalid move '{move}'. Please enter again.");
                    break;
                }

                if (gs.GameOver) break;
            }

            // Output the final grid and game results
            Console.WriteLine("\nFinal Grid:");
            Display.PrintGrid(gs);

            if (gs.GameOver)
            {
                if (gs.Winner != PlayerId.None)
                    Console.WriteLine($"\nResult: {(gs.Winner == PlayerId.P1 ? "P1" : "P2")} wins.");
                else
                    Console.WriteLine("\nResult: Tie.");
            }
            else
            {
                Console.WriteLine("\nResult: Game still in progress.");
            }

            // Display the inventory of both players
            Console.WriteLine($"\nInventory P1: @={gs.P1.OrdinaryCount}, B={gs.P1.BoringCount}, M={gs.P1.MagneticCount}");
            Console.WriteLine($"Inventory P2: #={gs.P2.OrdinaryCount}, b={gs.P2.BoringCount}, m={gs.P2.MagneticCount}");
        }
    }
}
