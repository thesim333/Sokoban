using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Filer
{
    public class Converter : IConverter
    {
        public string Compress(string uncompressed)
        {
            string replaced = ReplaceWhiteSpace(uncompressed);
            List<string> groupsOfSame = MakeGroupsOfSame(replaced);
            return MakeCompressedFromGroups(groupsOfSame);
        }

        public string Expand(string compressed)
        {
            StringBuilder sb = new StringBuilder();

            int index = 0;
            while (index < compressed.Length)
            {
                int newIndex = GetIndexNextNonNumeric(compressed, index);

                if (index == newIndex)
                {
                    sb.Append(compressed.Substring(index, 1));
                    index++;
                }
                else
                {
                    int x = int.Parse(compressed.Substring(index, newIndex - index));
                    for (int j = 0; j < x; j++)
                    {
                        sb.Append(compressed.Substring(newIndex, 1));
                    }
                    index = newIndex + 1;
                }
            }

            return sb.ToString();
        }

        protected string ReplaceWhiteSpace(string uncompressed)
        {
            Regex r = new Regex("\\s");
            return r.Replace(uncompressed, "-");
        }

        protected List<string> MakeGroupsOfSame(string uncompressed)
        {
            List<string> groups = new List<string>();
            groups.Add(uncompressed.Substring(0, 1)); //makes loop simpler 

            for (int i = 1; i < uncompressed.Length; i++)
            {
                string x = uncompressed.Substring(i, 1);

                if (x == groups.Last().Substring(0, 1))
                {
                    groups[groups.Count - 1] += x;
                }
                else
                {
                    groups.Add(x);
                }
            }

            return groups;
        }

        protected string MakeCompressedFromGroups(List<string> groups)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string x in groups)
            {
                if (x.Length == 1)
                {
                    sb.Append(x);
                }
                else
                {
                    sb.Append(x.Length.ToString());
                    sb.Append(x.Substring(0, 1));
                }
            }

            return sb.ToString();
        }

        protected int GetIndexNextNonNumeric(string compressed, int index)
        {
            for (int i = 0; i + index < compressed.Length; i++)
            {
                int x;
                if (!int.TryParse(compressed.Substring(i + index, 1), out x))
                {
                    return i + index;
                }
            }
            return compressed.Length;
        }
    }
}
