using GameGlobals;

namespace GameNS
{
    /// <summary>
    /// Holds the information from a game move
    /// </summary>
    public class Move
    {
        protected Position[] MovePositions;
        protected const int PLSTRT = 0;
        protected const int POS2 = 1;
        protected const int BOXEND = 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="Move"/> class.
        /// Player move only.
        /// </summary>
        /// <param name="playerStart">The player start position.</param>
        /// <param name="playerEnd">The player end position.</param>
        public Move(Position playerStart, Position playerEnd)
        {
            MovePositions = new Position[] { playerStart, playerEnd };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Move"/> class.
        /// Player and box move.
        /// </summary>
        /// <param name="playerStart">The player start position.</param>
        /// <param name="pos2">The middle position.</param>
        /// <param name="boxEnd">The box end position.</param>
        public Move(Position playerStart, Position pos2, Position boxEnd)
        {
            MovePositions = new Position[] { playerStart, pos2, boxEnd };
        }

        /// <summary>
        /// Gets the player start position.
        /// </summary>
        /// <returns>The position</returns>
        public Position GetPlayerStart()
        {
            return MovePositions[PLSTRT];
        }

        /// <summary>
        /// Gets the middle position.
        /// Where the player ends and the box starts.
        /// </summary>
        /// <returns>The position</returns>
        public Position GetPos2()
        {
            return MovePositions[POS2];
        }

        /// <summary>
        /// Was a box moved.
        /// </summary>
        /// <returns><c>true</c> if the positions for a box move are stored; otherwise, <c>false</c></returns>
        public bool BoxWasMoved()
        {
            return (MovePositions.Length > 2);
        }

        /// <summary>
        /// Gets the box end position.
        /// </summary>
        /// <returns>The position</returns>
        public Position GetBoxEnd()
        {
            return MovePositions[BOXEND];
        }

        /// <summary>
        /// Gets the array of positions.
        /// </summary>
        /// <returns>The array</returns>
        public Position[] GetArray()
        {
            return MovePositions;
        }
    }
}
