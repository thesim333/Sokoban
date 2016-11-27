using GameGlobals;

namespace Sokoban
{
    public interface IController
    {
        void DeleteState();
        void SaveGameState();
        void DesignerSetPartAt(int row, int col, Parts part);
        void CloseDesigner();
        void OpenDesignerNew();
        void CloseGame();
        void Move(Direction d);
        void Undo();
        void CheckDesignerLevel();
        void LoadGameState();
        void NewGame();
        void LoadLevelGame();
        void LoadLevelDesign();
        void SaveLevelDesigner();
    }
}
