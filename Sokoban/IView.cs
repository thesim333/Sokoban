using GameGlobals;

namespace Sokoban
{
    public interface IView
    {
        void DesignerNewLevel(int rows, int cols);
        void CreateLevelGridButton(int row, int col, Parts part);
        void DesignerLoadLevel();
        void DisplayMain();
        void GameSetup(int moves);
        void SetMoves(int moves);
        void SetGamePosition(int row, int col, Parts part);
        void FinishGame(int moves);
    }
}
