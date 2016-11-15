using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Designer;

namespace GameNS
{
    public enum Direction { Up, Down, Left, Right };

    public interface IGame
    {
        Position[] Move(Direction moveDirection);
        int GetMoveCount();
        Position[] Undo();
        void Restart();
        bool IsFinished();
        void Load(string name, string newLevel);
        int GetRows();
        int GetCols();
        Parts GetPartAt(int row, int col);
        void LoadState(State state);
        string GetName();
        State MakeState();
    }
}
