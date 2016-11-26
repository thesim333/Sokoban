using GameGlobals;

namespace GameNS
{
    public interface IGame
    {
        Position[] Move(Direction moveDirection);
        int GetMoveCount();
        Position[] Undo();
        void Restart();
        bool IsFinished();
        void Load(string newLevel);
        int GetRows();
        int GetCols();
        Parts GetPartAt(int row, int col);
        void LoadState(State state);
        State MakeState();
    }
}
