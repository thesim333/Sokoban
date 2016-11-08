using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filer
{
    public interface IConverter
    {
        string Compress(string uncompressed);
        string Expand(string compressed);
    }
}
