using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filer
{
    public class Stat
    {
        protected string name;
        public string Name
        {
            get
            {
                return name;
            }
        }
        protected int moves;
        public int Moves
        {
            get
            {
                return moves;
            }
        }

        public Stat(string statLine)
        {
            string[] line = statLine.Split('-');
            name = line[0];
            moves = int.Parse(line[1]);
        }

        public Stat(string theName, int theMoves)
        {
            name = theName;
            moves = theMoves;
        }
    }
}
