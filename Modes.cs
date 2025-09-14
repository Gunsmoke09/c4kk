namespace LineUp
{
    public enum PlayerId
    {
        None = 0,
        P1 = 1,
        P2 = 2
    }


    // Types of discs 
    public enum DiscKind
    {
        Ordinary,
        Boring,
        Magnetic,
        Invalid  
    }


    // game mode
    public enum GameMode
    {
        HvH, // Human vs Human
        HvC  // Human vs Computer
    }
}
