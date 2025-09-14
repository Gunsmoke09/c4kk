namespace LineUp
{
    public class Player
    {
        public PlayerId Id { get; }
        public char OrdinaryChar { get; }
        public char BoringChar { get; }
        public char MagneticChar { get; }

        public int OrdinaryCount { get; set; }
        public int BoringCount { get; set; } 
        public int MagneticCount { get; set; }
        public int MaxBoring   { get; set; } = 2;
        public int MaxMagnetic { get; set; } = 2;

        public Player(PlayerId id)
        {
            Id = id;
            if (id == PlayerId.P1)
            {
                OrdinaryChar = '@';
                BoringChar = 'B';
                MagneticChar = 'M';
            }
            else
            {
                OrdinaryChar = '#';
                BoringChar = 'b';
                MagneticChar = 'm';
            }
        }

        public char ToChar(DiscKind kind) => kind switch
        {
            DiscKind.Ordinary  => OrdinaryChar,
            DiscKind.Boring    => BoringChar,
            DiscKind.Magnetic => MagneticChar,
            _ => '?'
        };
    }
}
