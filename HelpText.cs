using System;
namespace LineUp
{
   //game guide
    public static class HelpText
    {
        public static void Show()
        {
            Console.WriteLine("\nGuide of Commands:");
            Console.WriteLine("  For your move, please frist enter the disc type with inital and follow with the cloumn number like below:");
            Console.WriteLine("  o4       -> Drop an Ordinary disc in column 4");
            Console.WriteLine("  b3      -> Drop a Boring disc in column 3");
            Console.WriteLine("  m2       -> Drop an Magnetic disc in column 2");
            Console.WriteLine("  Others:");
            Console.WriteLine("  save     -> save the game for next time'");
            Console.WriteLine("  help     -> show this help screen");
            Console.WriteLine("  quit     -> exit game");

            Console.WriteLine("\nRules of the game:");
            Console.WriteLine("  - Discs will always fall to the lowest empty row in a column.");
            Console.WriteLine("  - Boring (B/b): Clears the column and returns the discs to their owners; the boring disc stays at the bottom but behaves like an ordinary disc.");
            Console.WriteLine("  - Magnetic (M/m): Lifts the nearest own ordinary disc in the column up by 1 space; then becomes ordinary.");
        }
    }
}
