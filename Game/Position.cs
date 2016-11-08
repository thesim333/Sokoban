using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameNS
{
    public struct Position
    {
        private int row;
        private int col;
        public int Column
        {
            get
            {
                return row;
            }
            set
            {
                row = value;
            }
        }
        public int Row
        {
            get
            {
                return col;
            }
            set
            {
                col = value;
            }
        }

        public Position(int _row, int _col)
        {
            row = _row;
            col = _col;
        }
    }
}
