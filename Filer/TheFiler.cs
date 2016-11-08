using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Designer;

namespace Filer
{
    public class TheFiler : IFiler
    {
        protected const string EXTENSION = @".lvl";
        protected const string DIR = @"levels\";
        IConverter Convert;

        public TheFiler(IConverter c)
        {
            Convert = c;
        }

        public bool LevelExists(string fileName)
        {
            return File.Exists(DIR + fileName + EXTENSION);
        }

        public string Load(string fileName) 
        {
            return File.ReadAllText(DIR + fileName + EXTENSION);  //sends all this file text to TheLevel
        }

        public void Save(string fileName, IDesign filable)
        {
            List<string> theGrid = new List<string>();
            int rows = filable.GetRowCount();
            int cols = filable.GetColumnCount();
            StringBuilder sb = new StringBuilder();

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    sb.Append((char)filable.WhatsAt(r, c));
                }
                sb.Append(',');
            }
            sb.Remove(sb.Length - 1, 1); //remove the last ,
                        
            string compressed = Convert.Compress(sb.ToString());
            string[] fileLines = {"<" + FileManager.STATS + "></" + FileManager.STATS + ">",
                                  "<" + FileManager.GRID + ">" + compressed + "</" + FileManager.GRID + ">"};
            File.WriteAllLines(DIR + fileName + EXTENSION, fileLines);
        }

        public string[] GetAllLevels()
        {
            string[] theFiles = Directory.GetFiles(DIR, "*" + EXTENSION);
            
            for (int i = 0; i < theFiles.Length; i++)
            {
                //remove dir from name
                theFiles[i] = theFiles[i].Substring(7, theFiles[i].Length - 11);
            }

            return theFiles;
        }

        public void AppendState(string fileName, string stateName, string stateString)
        {
            string stateTag = FileManager.GetStateNameTags(stateName);
            string stateTagIn = "<" + stateTag + ">";
            string stateTagOut = "</" + stateTag + ">";
            string state = stateTag + stateString + stateTagOut;
            File.AppendAllText(DIR + fileName + EXTENSION, state);
        }

        public void ReplaceFile(string fileName, string file)
        {
            File.WriteAllText(DIR + fileName + EXTENSION, file);
        }
    }
}
