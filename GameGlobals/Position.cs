namespace GameGlobals
{
    /// <summary>
    /// A Point like object for holding level grid position information.
    /// [Row, Col]
    /// </summary>
    public struct Position
    {
        private int _row;
        private int _col;
        public int Row
        {
            get
            {
                return _row;
            }
            set
            {
                _row = value;
            }
        }
        public int Column
        {
            get
            {
                return _col;
            }
            set
            {
                _col = value;
            }
        }

        public Position(int row, int col)
        {
            _row = row;
            _col = col;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> struct.
        /// xy string from file = row,col
        /// </summary>
        /// <param name="xy">The xy.</param>
        public Position(string xy)
        {
            string[] myXY = xy.Split(',');
            _row = int.Parse(myXY[0]);
            _col = int.Parse(myXY[1]);
        }

        public string AsString()
        {
            return _row.ToString() + "," + _col.ToString();
        }
    }
}
