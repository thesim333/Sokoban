using System.Xml.Linq;

namespace Filer
{
    /// <summary>
    /// Checker for the level files.
    /// </summary>
    /// <seealso cref="Filer.IFileChecker" />
    public class FileChecker : IFileChecker
    {
        /// <summary>
        /// Checks the file for inconsistancies in the xml.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
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
