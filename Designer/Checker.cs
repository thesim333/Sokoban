using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Designer
{
    public class Checker
    {
        protected char[,] MyGrid;
        protected Position Player;
        protected Queue<Position> TheQ;

        public Checker(IDesign des)
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

        protected void MarkPos(Position p)
        {
            MyGrid[p.Row, p.Column] = '1';
        }

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
