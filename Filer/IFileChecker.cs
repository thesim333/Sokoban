using System.Xml.Linq;

namespace Filer
{
    public interface IFileChecker
    {
        bool FileChecksOut(string fileName);
    }
}
