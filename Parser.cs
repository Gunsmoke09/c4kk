namespace LineUp
{
    public static class Parser
    {
        public static bool TryParseMove(string[] parts, out DiscKind kind, out int colIndex0)
        {
            kind = DiscKind.Ordinary;
            colIndex0 = -1;

            if (parts == null || parts.Length == 0) return false;
            //validate input length
            string move = string.Join("", parts).Trim();
            if (move.Length < 2) return false;

            char type = move[0];
            string num = move.Substring(1);

            // Validate disc type (only o, b, e are valid input)
            kind = type switch
            {
                'o' or 'O' => DiscKind.Ordinary,
                'b' or 'B' => DiscKind.Boring,
                'm' or 'M' => DiscKind.Magnetic,
                _ => DiscKind.Invalid  
            };

            // If kind is invalid, return false
            if (kind == DiscKind.Invalid) return false;

            // Validate column number
            if (!int.TryParse(num, out int col1)) return false;   
            if (col1 <= 0 || col1 > 7) return false;            

            colIndex0 = col1 - 1;   
            return true;
        }
    }
}
