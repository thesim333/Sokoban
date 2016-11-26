using System.Collections.Generic;
using GameGlobals;

namespace Designer
{
    public class GridChecker : IGridChecker
    {
        protected char[,] MyGrid;
        protected Position Player;
        protected Queue<Position> TheQ;

        /// <summary>
        /// Inserts the designer.
        /// Makes the checker grid.
        /// Places Walls.
        /// Player as start location Position.
        /// </summary>
        /// <param name="des">The Designer object as IDesign</param>
        public void InsertDesigner(IDesign des)
        {
            int rows = des.GetRowCount();
            int cols = des.GetColumnCount();
            MyGrid = new char[rows, cols];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Parts p = des.WhatsAt(r, c);
                    MyGrid[r, c] = (p == Parts.Wall) ? '#' : '0';

                    if (p == Parts.Player || p == Parts.PlayerOnGoal)
                    {
                        Player = new Position(r, c);
                    }
                }
            }
        }

        /// <summary>
        /// Marks the position as queued.
        /// </summary>
        /// <param name="p">The position</param>
        protected void MarkPos(Position p)
        {
            MyGrid[p.Row, p.Column] = '1';
        }

        /// <summary>
        /// Runs the check algorithm to see if the player can reach the edge of the grid.
        /// Starting with the player position.
        /// Get the next position from the queue,
        /// If the position is not next to the edge,
        /// Queue the neighbours of this position.
        /// </summary>
        /// <returns><c>true</c>if the player is walled on all sides; otherwise, <c>false</c></returns>
        public bool PlayerCannotReachEdge()
        {
            TheQ = new Queue<Position>();
            TheQ.Enqueue(Player);
            MarkPos(Player);

            do
            {
                Position current = TheQ.Dequeue();

                if (IsNextToEdge(current))
                {
                    return false;
                }

                FindNeighbours(current);

            } while (TheQ.Count > 0);

            return true;
        }

        /// <summary>
        /// Determines whether the current position is next to an edge.
        /// </summary>
        /// <param name="p">The position</param>
        /// <returns>
        ///   <c>true</c> if the position is next to an edge; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsNextToEdge(Position p)
        {
            if (p.Row < 0 || p.Column < 0 || p.Row > MyGrid.GetLength(0) || p.Column > MyGrid.GetLength(1))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Finds the neighbours of the current position.
        /// If these neighbours are not already queued, queue them.
        /// </summary>
        /// <param name="p">The position</param>
        protected void FindNeighbours(Position p)
        {
            for (int r = p.Row - 1; r <= p.Row + 1; r++)
            {
                for (int c = p.Column - 1; c <= p.Column + 1; c++)
                {
                    if (MyGrid[r, c] == '0')
                    {
                        Position x = new Position(r, c);
                        TheQ.Enqueue(x);
                        MarkPos(x);
                    }
                }
            }
        }
    }
}
