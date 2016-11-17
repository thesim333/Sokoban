using Designer;

namespace GameNS
{
    public class Move
    {
        /*
         * Player Start
         * Player End
         * Box Start
         * Box End
         * Last 2 will not be created if a box is not moved
         */
        protected Position[] MovePositions;
        protected const int PLSTRT = 0;
        protected const int POS2 = 1;
        protected const int BOXEND = 3;

        public Move(Position playerStart, Position playerEnd)
        {
            MovePositions = new Position[] { playerStart, playerEnd };
        }

        public Move(Position playerStart, Position pos2, Position boxEnd)
        {
            MovePositions = new Position[] { playerStart, pos2, boxEnd };
        }

        public Position GetPlayerStart()
        {
            return MovePositions[PLSTRT];
        }

        public Position GetPos2()
        {
            return MovePositions[POS2];
        }

        public bool BoxWasMoved()
        {
            return (MovePositions.Length > 2);
        }

        public Position GetBoxEnd()
        {
            return MovePositions[BOXEND];
        }

        public Position[] GetArray()
        {
            return MovePositions;
        }
    }
}
