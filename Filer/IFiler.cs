using Designer;
using GameGlobals;

namespace Filer
{
    public interface IFiler
    {
        void Save(string fileName, IDesign filable);
        string LoadGrid(string fileName);
        string[] GetAllLevels();
        void AppendState(string fileName, string stateName, State theState);
        string[] GetAllStates(string fileName);
        State LoadState(string fileName, string stateName);
        bool LevelExists(string fileName);
        bool StateExists(string fileName, string stateName);
        void ReplaceState(string fileName, string stateName, State theState);
        void DeleteState(string fileName, string stateName);
        void AppendStat(string fileName, string playerName, int moves);
        Stat[] GetBestX_Stats(string fileName, int x);
    }
}
