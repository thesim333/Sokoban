using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameGlobals
{
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
