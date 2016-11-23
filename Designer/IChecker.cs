using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Designer
{
    public interface IChecker
    {
        bool PlayerCannotReachEdge();
        void InsertDesigner(IDesign des);
    }
}
