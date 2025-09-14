using System;

namespace LineUp
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            //setup the effect display for the "boring" and "exploding" 
            GameMethods.FramePrinter = Display.PrintFrame;

            //trigger testrunner 
            int testArgIndex = Array.FindIndex(args, a => a.Equals("--test", StringComparison.OrdinalIgnoreCase));
            if (testArgIndex >= 0 && testArgIndex + 1 < args.Length)
            {
                TestRunner.RunTestingMode(args[testArgIndex + 1]);
                return;
            }

            while (true)
            {
                //main menu
                Console.WriteLine("Welcome to LineUp");
                Console.WriteLine("1. New Game");
                Console.WriteLine("2. Load Game");
                Console.WriteLine("3. Game Guide");
                Console.WriteLine("4. Quit Game");
                Console.WriteLine("5. Test Mode");
                Console.Write("Please enter the number you would like to choose: ");
                var ch = Console.ReadLine()?.Trim();

                if (ch == "1")
                {
                    //go Gameloop
                    var gs = SetupNewGame();
                    GameLoop(gs);
                }
                else if (ch == "2")
                {
                    //go loadgame
                    Console.Write("Enter the file name to load: ");
                    var fileName = FileJson(Console.ReadLine() ?? "");

                    string filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), fileName);

                    if (SaveLoad.TryLoad(filePath, out var gs))
                    {
                        Console.WriteLine("Loaded.");
                        GameLoop(gs!);
                    }
                    else
                    {
                        Console.WriteLine("Failed to load the file, please enter the correct file name.");
                    }
                }
                else if (ch == "3")
                {
                    //go gmaeguide
                    HelpText.Show();
                    Display.ContinuePrompt();
                }
                else if (ch == "4" || (ch?.Equals("q", StringComparison.OrdinalIgnoreCase) ?? false))
                {
                    //go quitgame
                    return;
                }
                else if (ch == "5")
                {
                    //go test mode 
                    Console.Write("Enter the moves and please seperate each move with comma, e.g., o4, b3, e2): ");
                    string? moves = Console.ReadLine()?.Trim();
                    if (!string.IsNullOrEmpty(moves))
                    {
                        TestRunner.RunTestingMode(moves);
                    }
                    else
                    {
                        Console.WriteLine("No moves entered. Please try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid entry. Please try again.");
                }
            }
        }

        private static GameState SetupNewGame()
        {
            //ask user the number of rows and colimns they want for the game grid
            const int defCols = 7, defRows = 6;

            int cols = ReadInt($"Please enter the number of columns (min {defCols}): ", defCols, 100);
            int rows = ReadInt($"Please enter the number of rows (min {defRows}, max = columns): ", defRows, cols);
            var mode = ReadMode();

            var gs = GameState.CreateNew(rows, cols, mode);
            Console.WriteLine($"Grid size: {rows}x{cols} | Win condition: {gs.WinLength} in a row (vertically, horizontally, or diagonally)");
            return gs;
        }

        private static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                //if user input invalid number, prompt error and enter again
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int v) && v >= min && v <= max) return v;
                Console.WriteLine("Invalid number. Please enter again.");
            }
        }

        private static GameMode ReadMode()
        {
            while (true)
            {
                //choose game mode
                Console.Write("Please enter the number for Game Mode (1 = Human vs Human, 2 = Human vs Computer): ");
                var s = Console.ReadLine()?.Trim();
                if (s == "1") return GameMode.HvH;
                if (s == "2") return GameMode.HvC;
                Console.WriteLine("Invalid. Please enter the number again");
            }
        }

        private static void GameLoop(GameState gs)
        {
            while (!gs.GameOver)
            {
                //runs the main loop of the game
                Console.Clear();
                Display.PrintHeader(gs);
                Display.PrintGrid(gs);

                if (gs.GameOver) break;

                if (gs.Mode == GameMode.HvC && gs.CurrentTurn == PlayerId.P2)
                {
                    GameMethods.AITurn(gs);
                    continue;
                }

                Console.Write("[Please enter your move (e.g., o4, b2, e3), or 'save game', 'help', 'quit']: ");
                var input = Console.ReadLine() ?? "";
                var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) continue;

                if (parts[0].Equals("help", StringComparison.OrdinalIgnoreCase))
                {
                    HelpText.Show();
                    Display.ContinuePrompt();
                    continue;
                }
                if (parts[0].Equals("quit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Exiting game.");
                    return;
                }

                // Save game handling
                if (parts[0].Equals("save", StringComparison.OrdinalIgnoreCase) && parts.Length >= 2 && parts[1].Equals("game", StringComparison.OrdinalIgnoreCase))
                {
                    Console.Write("Please enter the file name: ");
                    string? fileName = Console.ReadLine()?.Trim();

                    if (string.IsNullOrWhiteSpace(fileName))
                    {
                        Console.WriteLine("Invalid file name. Please try again.");
                        Display.ContinuePrompt();
                        continue;
                    }

                    //file saving and loading
                    fileName = FileJson(fileName);

                    string filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), fileName); // Save in current directory

                    if (SaveLoad.TrySave(filePath, gs))
                    {
                        Console.WriteLine($"Saved to {fileName}");
                    }
                    else
                    {
                        Console.WriteLine("Save failed.");
                    }

                    Display.ContinuePrompt();
                    continue;
                }

                // Game moves
                if (!Parser.TryParseMove(parts, out var kind, out var colIndex))
                {
                    Console.WriteLine("Invalid input. Please try again");
                    Display.ContinuePrompt();
                    continue;
                }

                if (!GameMethods.ApplyMove(gs, kind, colIndex))
                {
                    // validate the players move
                    Display.ContinuePrompt();
                    continue;
                }
            }

            Console.Clear();
            Display.PrintHeader(gs);
            Display.PrintGrid(gs);
            if (gs.Winner != PlayerId.None)
                Console.WriteLine($"Game over — Winner: {(gs.Winner == PlayerId.P1 ? "P1" : "P2")}");
            else
                Console.WriteLine("Game over — Tie.");
            Display.ContinuePrompt();
        }

        private static string FileJson(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "savegame.json";
            name = name.Trim();
            return name.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ? name : name + ".json";
        }
    }
}
