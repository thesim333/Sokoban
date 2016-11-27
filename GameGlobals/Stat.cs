namespace GameGlobals
{
    /// <summary>
    /// Holds a Stat to and from a file.
    /// </summary>
    public class Stat
    {
        protected string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        protected string moves;
        public string Moves
        {
            get
            {
                return moves;
            }
            set
            {
                moves = value;
            }
        }

        public Stat(string statLine)
        {
            string[] line = statLine.Split('-');
            name = line[0];
            moves = line[1];
        }

        public Stat(string theName, string theMoves)
        {
            name = theName;
            moves = theMoves;
        }

        public Stat()
        {

        }
    }
}
