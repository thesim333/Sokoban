using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Designer;

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
        void FinishGame();
        void ReestablishKeys();
    }
}
