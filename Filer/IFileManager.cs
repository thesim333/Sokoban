using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filer
{
    public interface IFileManager
    {
        string GetStats(string file);
        string GetBestStat(string file);
        string GetFileWithNewStatInserted(string file, string player, int moves);
        string GetLevel(string file);
        string InsertSaveState(string file, string stateName, string state);
        string[] GetStatesSaved(string file);
        string GetState(string file, string stateName);
        string OverwriteSavedState(string file, string stateName, string state);
        string DeleteSavedState(string file, string stateName);
        bool StateExists(string file, string stateName);
    }
}
