using System.Collections.Generic;
using System.Linq;

namespace GameGlobals
{
    /// <summary>
    /// Holds a game state for transfer between Game and the file.
    /// </summary>
    public class State
    {
        protected Position _player;
        protected int _moves;
        protected List<Position> _blocks;

        public Position Player
        {
            get
            {
                return _player;
            }
        }

        public int Moves
        {
            get
            {
                return _moves;
            }
        }

        public List<Position> Blocks
        {
            get
            {
                return _blocks.ToList();
            }
        }

        public State(int moves, Position player, string blocks)
        {
            _moves = moves;
            _player = player;
            MakeBlocksList(blocks);
        }

        public State(int moves, Position player, List<Position> blocks)
        {
            _moves = moves;
            _player = player;
            _blocks = blocks;
        }

        /// <summary>
        /// Makes the blocks position list from a string of rows and columns.
        /// </summary>
        /// <param name="theBlocks">The blocks.</param>
        protected void MakeBlocksList(string theBlocks)
        {
            string[] each = theBlocks.Split(';');
            _blocks = new List<Position>();

            foreach (string b in each)
            {
                string[] xy = b.Split(',');
                int row = int.Parse(xy[0]);
                int col = int.Parse(xy[1]);
                _blocks.Add(new Position(row, col));
            }
        }
    }
}
