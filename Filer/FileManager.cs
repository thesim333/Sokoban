using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Filer
{
    public class FileManager : IFileManager
    {
        protected IConverter Converter;
        public const string GRID = "grid";
        public const string STATE = "state";
        public const string STATS = "stats";

        public FileManager(IConverter converter)
        {
            Converter = converter;
        }

        public string GetFileWithNewStatInserted(string file, string player, int moves)
        {
            string stats = GetStats(file);
            List<Stat> theStats = MakeStatList(stats);
            theStats = InsertStat(theStats, new Stat(player, moves));
            return StatsToString(theStats);
        }

        protected string StatsToString(List<Stat> theStats)
        {
            string statString = string.Empty;

            foreach (Stat s in theStats)
            {
                statString += s.Name + "-" + s.Moves.ToString() + ",";
            }

            return statString.Substring(0, statString.Length - 2);
        }

        protected List<Stat> InsertStat(List<Stat> theStats, Stat toInsert)
        {
            if (theStats.Count == 0)
            {
                theStats.Add(toInsert);
            }
            else
            {
                for (int i = 0; i < theStats.Count; i++)
                {
                    if (toInsert.Moves < theStats[i].Moves)
                    {
                        theStats.Insert(i, toInsert);
                    }
                }
            }

            return theStats;
        }

        protected List<Stat> MakeStatList(string stats)
        {
            List<Stat> theStats = new List<Stat>();
            string[] statArray = stats.Split(',');

            foreach (string s in statArray)
            {
                Stat stat = new Stat(s);
                theStats.Add(stat);
            }

            return theStats;
        }

        public string GetLevel(string file)
        {
            return Converter.Expand(GetStuffFromTags(file, GRID));
        }
        
        public string[] GetStatesSaved(string file)
        {
            //returns an array of state names
            string pat = @"<state-(\w+)>";
            Regex r = new Regex(pat);
            Match m = r.Match(file);
            List<string> matches = new List<string>();

            while (m.Success)
            {
                matches.Add(GetStateName(m.Value));
            }

            return matches.ToArray();
        }

        protected string GetStateName(string tag)
        {
            //seperates the name from the tag
            return tag.Substring(7, tag.Length - 8);
        }

        public string GetStats(string file) //returns all stats
        {
            return GetStuffFromTags(file, STATS);
        }

        public bool StateExists(string file, string stateName)
        {
            return (GetStartOf(file, GetStateNameTags(stateName)) > 0);
        }

        public string GetBestStat(string file)
        {
            return GetStats(file).Split(',')[0];
        }

        public string InsertSaveState(string file, string stateName, string state) //appends to end of file
        {
            string[] tags = MakeStateTags(stateName);
            return file + tags[0] + state + tags[1];
        }

        public string OverwriteSavedState(string file, string stateName, string state)
        {
            string fullState = GetStateNameTags(stateName);
            int start = GetStartOf(file, fullState) + GetLengthOfTag(fullState);
            int end = GetEndOf(file, fullState);
            return file.Substring(0, start) + state + file.Substring(end, file.Length - end);
        }

        public string DeleteSavedState(string file, string stateName)
        {
            string fullStateTag = GetStateNameTags(stateName);
            string before = GetFileBeforeTag(file, fullStateTag);
            string after = GetFileAfterTag(file, fullStateTag);
            return before + after;
        }

        public string GetState(string file, string stateName)
        {
            return GetStuffFromTags(file, GetStateNameTags(stateName));
        }

        protected string GetFileBeforeTag(string file, string tag)
        {
            int start = file.IndexOf("<" + tag + ">");
            return file.Substring(0, start - 1);
        }

        protected string GetFileAfterTag(string file, string tag)
        {
            int end = GetEndOf(file, tag) + GetLengthOfTag(tag) + 1;
            return (end == file.Length - 1) ? string.Empty : file.Substring(end, file.Length - end - 1);
        }

        protected string GetStuffFromTags(string file, string tags)
        {
            int start = GetStartOf(file, tags);
            int end = GetEndOf(file, tags);
            return file.Substring(start, end - start);
        }

        protected int GetStartOf(string file, string tags)
        {
            return file.IndexOf("<" + tags + ">") + tags.Length + 2;
        }

        protected int GetEndOf(string file, string tags)
        {
            return file.IndexOf("</" + tags + ">");
        }

        public static string GetStateNameTags(string stateName)
        {
            return STATE + "-" + stateName;
        }

        protected int GetLengthOfTag(string tag)
        {
            return ("<" + tag + ">").Length;
        }

        protected string[] MakeStateTags(string name)
        {
            string tag = GetStateNameTags(name) + ">";
            return new string[] { "<" + tag, "</" + tag };
        }
    }
}