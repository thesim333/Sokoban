using System.Xml.Linq;

namespace Filer
{
    public class FileChecker : IFileChecker
    {
        public bool FileChecksOut(string fileName)
        {
            try
            {
                XDocument xml = XDocument.Load(fileName);
                if (xml.Root.Name != "Level")
                {
                    return false;
                }
                if (xml.Root.Element("Stats") == null)
                {
                    return false;
                }
                if (xml.Root.Element("Grid") == null)
                {
                    return false;
                }
                if (xml.Root.Element("States") == null)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
