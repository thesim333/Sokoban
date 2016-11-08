using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Designer
{
    public enum Parts
    {
        Wall = (int)'#',
        Empty = (int)'-',
        Player = (int)'@',
        Goal = (int)'.',
        Block = (int)'$',
        BlockOnGoal = (int)'*',
        PlayerOnGoal = (int)'+'
    };

    public interface IDesign
    {
        void NewLevel(int rows, int cols);
        void LoadLevel(string name, string level);
        Parts WhatsAt(int row, int column);
        int GetRowCount();
        int GetColumnCount();
        void SetPart(int row, int col, Parts part);
        string GetName();
        bool HasUnsavedChanges();
        void SetSaved();
    }
}
