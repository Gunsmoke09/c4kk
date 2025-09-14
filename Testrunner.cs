using System;

namespace LineUp
{
    // Runs a sequence of scripted moves for testing/evaluation.
    public static class TestRunner
    {
        public static void RunTestingMode(string line)
        {
            // Create a new game state with default settings (6 rows, 7 columns, Player vs Player)
            var gs = GameState.CreateNew(6, 7, GameMode.HvH);  // You can change this if you want a different mode
            Console.WriteLine("Testing mode: parsing moves and applying in order. P1 starts the move first.");
            Console.WriteLine($"Input: {line}");

            // Split the input line into tokens (individual moves)
            var tokens = line.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var tok in tokens)
            {
                // Parse each move token into a disc type and column index
                if (!Parser.TryParseMove(new[] { tok }, out var kind, out var col))
                {
                    Console.WriteLine($"Invalid Input '{tok}'. Please enter again.");
                    break;
                }

                Console.WriteLine($"P1: {gs.P1.OrdinaryCount} Ordinary, {gs.P1.BoringCount} Boring, {gs.P1.MagneticCount} Magnetic");
                Console.WriteLine($"P2: {gs.P2.OrdinaryCount} Ordinary, {gs.P2.BoringCount} Boring, {gs.P2.MagneticCount} Magnetic");

                // Apply the move and check if it's legal
                if (!GameMethods.ApplyMoveNoFrames(gs, kind, col))
                {
                    Console.WriteLine($"Illegal move '{tok}'. Stopping.");
                    break;
                }

                // Stop if the game is over after this move
                if (gs.GameOver)
                {
                    break;
                }

                // Print the grid after applying the move
                Console.WriteLine("\nCurrent Grid:");
                Display.PrintGrid(gs);
                Console.WriteLine();  // Space between moves for readability
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
            Console.WriteLine($"\nInventory P1: @={gs.P1.OrdinaryCount}, B={gs.P1.BoringCount}, E={gs.P1.MagneticCount}");
            Console.WriteLine($"Inventory P2: #={gs.P2.OrdinaryCount}, b={gs.P2.BoringCount}, e={gs.P2.MagneticCount}");
        }
    }
}
