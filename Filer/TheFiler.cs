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
        protected string CurrentDirectory;
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
        protected string CurrentFile; //whole path

        public TheFiler(IConverter c)
        {
            Convert = c;
        }

        /// <summary>
        /// Levels the exists as a saved file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        protected bool LevelExists(string fileName)
        {
            return File.Exists(fileName);
        }

        /// <summary>
        /// Loads the grid from the file.
        /// File is stored for 
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public string LoadGrid(string fileName) 
        {
            StoreFileInfo(fileName);
            XDocument xml = XDocument.Load(fileName);
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
        public string Save(string filePath, IDesign filable)
        {
            StoreFileInfo(filePath);
            XmlTextWriter writer = new XmlTextWriter(filePath, null);
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
            return Path.GetFileName(filePath);
        }

        protected void StoreFileInfo(string fileName)
        {
            CurrentFile = fileName;
            CurrentDirectory = Path.GetDirectoryName(fileName);
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
        /// Appends the state to the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="stateName">Name of the state.</param>
        /// <param name="theState">The state.</param>
        public void AppendState(string stateName, State theState)
        {
            List<Position> blocks = theState.Blocks;

            XDocument xml = XDocument.Load(CurrentFile);
            XElement level = xml.Element(LEVEL);
            XElement states = level.Element(STATES);
            states.Add(new XElement(STATE,
                      new XElement(NAME, stateName),
                      new XElement(MOVES, theState.Moves),
                      new XElement(PLAYER, theState.Player.AsString()),
                      new XElement(BLOCKS, MakeBlocksString(blocks))));
            xml.Save(CurrentFile);
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
        /// Gets all states in the level file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Array of state names</returns>
        public string[] GetAllStates()
        {
            XDocument xml = XDocument.Load(CurrentFile);
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
        public State LoadState(string stateName)
        {
            XDocument xml = XDocument.Load(CurrentFile);
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
        public bool StateExists(string stateName)
        {
            XDocument xml = XDocument.Load(CurrentFile);
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
        public void ReplaceState(string stateName, State theState)
        {
            XDocument xml = XDocument.Load(CurrentFile);
            XElement stateQ = (from state in xml.Root.Descendants(STATES).Descendants(STATE)
                                 where state.Descendants(NAME).Any(n => n.Value == stateName)
                                 select state).FirstOrDefault();

            stateQ.Element(MOVES).Value = theState.Moves.ToString();
            stateQ.Element(PLAYER).Value = theState.Player.AsString();
            stateQ.Element(BLOCKS).Value = MakeBlocksString(theState.Blocks);
            xml.Save(CurrentFile);
        }

        /// <summary>
        /// Appends the user's name and move count from a won game.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="playerName">Name of the player.</param>
        /// <param name="moves">The moves.</param>
        public void AppendStat(string playerName, int moves)
        {
            XDocument xml = XDocument.Load(CurrentFile);
            XElement level = xml.Element(LEVEL);
            XElement stats = level.Element(STATS);
            stats.Add(new XElement(STAT,
                      new XElement(NAME, playerName),
                      new XElement(MOVES, moves)));
            xml.Save(CurrentFile);
        }

        /// <summary>
        /// Deletes the state from the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="stateName">Name of the state.</param>
        public void DeleteState(string stateName)
        {
            XDocument xml = XDocument.Load(CurrentFile);
            XElement stateQ = (from state in xml.Root.Descendants(STATES).Descendants(STATE)
                               where state.Descendants(NAME).Any(n => n.Value == stateName)
                               select state).FirstOrDefault();
            stateQ.Remove();
            xml.Save(CurrentFile);
        }

        /// <summary>
        /// Gets the best x won game stats.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="x">The x.</param>
        /// <returns>Array of x Stat objects</returns>
        public Stat[] GetBestX_Stats(int x)
        {
            List<Stat> statList = new List<Stat>();
            XDocument xml = XDocument.Load(CurrentFile);

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

        public string GetCurrentPath()
        {
            return CurrentDirectory;
        }

        public void InsertApplicationPath(string path)
        {
            CurrentDirectory = path;
        }
    }
}
