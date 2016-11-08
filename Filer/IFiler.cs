using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Designer;

namespace Filer
{
    public interface IFiler
    {
        void Save(string fileName, IDesign filable);
        string Load(string fileName);
        string[] GetAllLevels();
        void AppendState(string fileName, string stateName, string stateString);
        void ReplaceFile(string fileName, string file);
        bool LevelExists(string fileName);
    }
}
