namespace LineUp
{
    public class Cell
    {
        // Which player owns this cell 
        public PlayerId Owner { get; set; } = PlayerId.None;

        // The disc kind currently occupying the cell 
        public DiscKind Kind { get; set; } = DiscKind.Ordinary;

        // True if the cell does not contain a disc.
        public bool IsEmpty => Owner == PlayerId.None;
    }
}
