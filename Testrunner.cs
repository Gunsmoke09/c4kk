using System;

namespace LineUp
{
    public static class TestRunner
    {
        public static void RunTestingMode(string line)
        {
            // testing game default setting (grid size 6*7 and only human vs human)
            var gs = GameState.CreateNew(6, 7, GameMode.HvH);
            Console.WriteLine("Testing mode: parsing moves order. P1 starts the move first.");
            Console.WriteLine($"Input: {line}");

            // Split the input line into individual moves
            var idv_moves = line.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

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

                // Stop if the game is over after this move
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
