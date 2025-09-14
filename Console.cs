using System;
using System.Text;

namespace LineUp
{
    public static class Display
    {
        public static void PrintHeader(GameState gs)
        {
            Console.WriteLine($"Mode: {gs.Mode} | Gird Size: {gs.Rows}x{gs.Cols} | WinLength: {gs.WinLength}");
            Console.WriteLine($"Turn: {(gs.CurrentTurn == PlayerId.P1 ? "P1" : "P2")}");
            Console.WriteLine($"P1: @={gs.P1.OrdinaryCount}, B={gs.P1.BoringCount}, E={gs.P1.MagneticCount}");
            Console.WriteLine($"P2: #={gs.P2.OrdinaryCount}, b={gs.P2.BoringCount}, e={gs.P2.MagneticCount}");
            Console.WriteLine();
}


public static void PrintGrid(GameState gs)
{
    var sb = new StringBuilder();

    // Loop through rows (top to bottom) to display rows
    for (int r = 0; r < gs.Rows; r++)
    {
        // Loop through each column in the row
        for (int c = 0; c < gs.Cols; c++)
        {
            char ch = gs.RenderCharAt(r, c); // Get the actual character to display
            sb.Append($"| {ch} "); // Add the character for each column
        }
        sb.AppendLine("|");  // End of the row with a border
    }

    // Print column numbers at the bottom
    for (int c = 1; c <= gs.Cols; c++)
    {
        sb.Append(c < 10 ? $"  {c} " : $" {c} ");
    }

    sb.AppendLine();  // Move to the next line
    Console.Write(sb.ToString());
}


        public static void ContinuePrompt()
        {
            Console.Write("Press Enter to continue.");
            Console.ReadLine();
        }


        public static void PrintFrame(string title, GameState gs)
        {
            Console.Clear();
            PrintHeader(gs);
            Console.WriteLine($"[{title}]");
            PrintGrid(gs);
            Console.WriteLine();
            Console.Write("Press Enter to back to main menu.");
            Console.ReadLine();
        }
    }
}
