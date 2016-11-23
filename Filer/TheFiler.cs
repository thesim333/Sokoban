using System.Text;
using System.IO;
using Designer;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using GameGlobals;
using System.Collections.Generic;

namespace Filer
{
    public class TheFiler : IFiler
    {
        private const string EXTENSION = @".lvl";
        private const string DIR = @"levels\";
        protected IConverter Convert;
        private const string LEVEL = "Level";
        private const string GRID = "Grid";
        private const string STATES = "States";
        private const string STATE = "State";
        private const string STATS = "Stats";
        private const string STAT = "Stat";
        private const string NAME = "Name";
        private const string SCORE = "Score";
        private const string MOVES = "Moves";
        private const string PLAYER = "Player";
        private const string BLOCKS = "Blocks";

        public TheFiler(IConverter c)
        {
            Convert = c;
        }

        /// <summary>
        /// Levels the exists as a saved file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public bool LevelExists(string fileName)
        {
            return File.Exists(AddDirExt(fileName));
        }

        /// <summary>
        /// Loads the grid from the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public string LoadGrid(string fileName) 
        {
            XDocument xml = XDocument.Load(AddDirExt(fileName));
            var query = from grid in xml.Root.Descendants(GRID)
                        select grid.Value.ToString();
            return Convert.Expand(query.FirstOrDefault());
        }

        /// <summary>
        /// Creates a new save file with the level from designer.
        /// The level is saved in compressed string format.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="filable">The level designer to save from.</param>
        public void Save(string fileName, IDesign filable)
        {
            XmlTextWriter writer = new XmlTextWriter(AddDirExt(fileName), null);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            writer.WriteStartElement(LEVEL);
            writer.WriteStartElement(STATS);
            writer.WriteEndElement();
            writer.WriteStartElement(GRID);
            writer.WriteString(Convert.Compress(MakeLvlString(filable)));
            writer.WriteEndElement();
            writer.WriteStartElement(STATES);
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Flush();
            writer.Close();            
        }

        /// <summary>
        /// Makes the level string from the designer.
        /// </summary>
        /// <param name="filable">The filable.</param>
        /// <returns>The string representing the level</returns>
        protected string MakeLvlString(IDesign filable)
        {
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
            return sb.ToString();
        }

        /// <summary>
        /// Gets all levels.
        /// </summary>
        /// <returns>Array containing all the levels from the /levels/</returns>
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

        /// <summary>
        /// Appends the state to the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="stateName">Name of the state.</param>
        /// <param name="theState">The state.</param>
        public void AppendState(string fileName, string stateName, State theState)
        {
            List<Position> blocks = theState.Blocks;

            XDocument xml = XDocument.Load(AddDirExt(fileName));
            XElement level = xml.Element(LEVEL);
            XElement states = level.Element(STATES);
            states.Add(new XElement(STATE,
                      new XElement(NAME, stateName),
                      new XElement(MOVES, theState.Moves),
                      new XElement(PLAYER, theState.Player.AsString()),
                      new XElement(BLOCKS, MakeBlocksString(blocks))));
            xml.Save(AddDirExt(fileName));
        }

        /// <summary>
        /// Makes a string of block positions from the state.
        /// </summary>
        /// <param name="blocks">The block positions list.</param>
        /// <returns>string to save in state</returns>
        protected string MakeBlocksString(List<Position> blocks)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Position p in blocks)
            {
                sb.Append(p.AsString());
                sb.Append(";");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        /// <summary>
        /// Adds the directory and extension to the file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>File name ready to use</returns>
        protected string AddDirExt(string fileName)
        {
            return DIR + fileName + EXTENSION;
        }

        /// <summary>
        /// Gets all states in the level file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Array of state names</returns>
        public string[] GetAllStates(string fileName)
        {
            XDocument xml = XDocument.Load(AddDirExt(fileName));
            var query = from states in xml.Root.Descendants(STATES).Descendants(STATE).Descendants(NAME)
                        select states.Value;
            return query.ToArray();
        }

        /// <summary>
        /// Loads the state as a State Object.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="stateName">Name of the state.</param>
        /// <returns>State</returns>
        public State LoadState(string fileName, string stateName)
        {
            XDocument xml = XDocument.Load(AddDirExt(fileName));
            var query = from state in xml.Root.Descendants(STATES).Descendants(STATE)
                        where state.Element(NAME).Value == stateName
                        select new
                        {
                            moves = state.Element(MOVES).Value,
                            player = state.Element(PLAYER).Value,
                            blocks = state.Element(BLOCKS).Value
                        };
            var thisQ = query.FirstOrDefault();
            int moves = int.Parse(thisQ.moves);
            Position player = new Position(thisQ.player);
            return new State(moves, player, thisQ.blocks);
            
        }

        /// <summary>
        /// Does the state exist.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="stateName">Name of the state.</param>
        /// <returns><c>true</c> if the state exists in the file; otherwise, <c>false</c></returns>
        public bool StateExists(string fileName, string stateName)
        {
            XDocument xml = XDocument.Load(AddDirExt(fileName));
            var query = from state in xml.Root.Descendants(STATES).Descendants(STATE)
                        where state.Element(NAME).Value == stateName
                        select state;
            return (query.Count() > 0);
        }

        /// <summary>
        /// Overwrites an existing state.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="stateName">Name of the state.</param>
        /// <param name="theState">The state.</param>
        public void ReplaceState(string fileName, string stateName, State theState)
        {
            XDocument xml = XDocument.Load(AddDirExt(fileName));
            XElement stateQ = (from state in xml.Root.Descendants(STATES).Descendants(STATE)
                                 where state.Descendants(NAME).Any(n => n.Value == stateName)
                                 select state).FirstOrDefault();

            stateQ.Element(MOVES).Value = theState.Moves.ToString();
            stateQ.Element(PLAYER).Value = theState.Player.AsString();
            stateQ.Element(BLOCKS).Value = MakeBlocksString(theState.Blocks);
            xml.Save(AddDirExt(fileName));
        }

        /// <summary>
        /// Appends the user's name and move count from a won game.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="playerName">Name of the player.</param>
        /// <param name="moves">The moves.</param>
        public void AppendStat(string fileName, string playerName, int moves)
        {
            XDocument xml = XDocument.Load(AddDirExt(fileName));
            XElement level = xml.Element(LEVEL);
            XElement stats = level.Element(STATS);
            stats.Add(new XElement(STAT,
                      new XElement(NAME, playerName),
                      new XElement(MOVES, moves)));
            xml.Save(AddDirExt(fileName));
        }

        /// <summary>
        /// Deletes the state from the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="stateName">Name of the state.</param>
        public void DeleteState(string fileName, string stateName)
        {
            XDocument xml = XDocument.Load(AddDirExt(fileName));
            XElement stateQ = (from state in xml.Root.Descendants(STATES).Descendants(STATE)
                               where state.Descendants(NAME).Any(n => n.Value == stateName)
                               select state).FirstOrDefault();
            stateQ.Remove();
            xml.Save(AddDirExt(fileName));
        }

        /// <summary>
        /// Gets the best x won game stats.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="x">The x.</param>
        /// <returns>Array of x Stat objects</returns>
        public Stat[] GetBestX_Stats(string fileName, int x)
        {
            List<Stat> statList = new List<Stat>();
            XDocument xml = XDocument.Load(AddDirExt(fileName));

            var unsorted_stats = (from stats in xml.Root.Element(STATS).Elements(STAT)
                                 select new
                                 {
                                     Name = stats.Element(NAME).Value,
                                     Moves = stats.Element(MOVES).Value
                                 }).Take(x);
            var sorted_stats = (from stats in unsorted_stats
                                orderby int.Parse(stats.Moves), (string)stats.Name descending
                                select new Stat
                                {
                                    Name = stats.Name,
                                    Moves = stats.Moves
                                }).ToArray();
            
            return sorted_stats;
        }
    }
}
