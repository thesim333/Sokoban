using Designer;
using GameGlobals;

namespace Filer
{
    public interface IFiler
    {
        string Save(string fileName, IDesign filable);
        string LoadGrid(string fileName);
        void AppendState(string stateName, State theState);
        string[] GetAllStates();
        State LoadState(string stateName);
        bool StateExists(string stateName);
        void ReplaceState(string stateName, State theState);
        void DeleteState(string stateName);
        void AppendStat(string playerName, int moves);
        Stat[] GetBestX_Stats(int x);
        string GetCurrentPath();
        void InsertApplicationPath(string path);
    }
}
