using GameGlobals;

namespace Sokoban
{
    public interface IView
    {
        void DesignerNewLevel(int rows, int cols);
        void CreateLevelGridButton(int row, int col, Parts part);
        void DesignerLoadLevel();
        void DisplayMain();
        void GameSetup(int moves, int rows, int cols);
        void SetMoves(int moves);
        void SetGamePosition(int row, int col, Parts part);
        void FinishGame(int moves);
        string SaveMyFile(string initialDir);
        void Display(string message);
        string GetInput(string message);
        int[] GetLevelSize();
        string GetUserResponse(string message, string caption);
        string GetFileToLoad(string initialDir);
        string GetSelectedState(string[] states);
    }
}
