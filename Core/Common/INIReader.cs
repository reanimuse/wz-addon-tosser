using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace WzAddonTosser.Core.Common
{

    public class INIFile
    {
        public INISectionsCollection Sections { get; protected set; }
        public string PathToIniFile { get; protected set; }

        public INIFile(string pathToIni)
        {
            Sections = new INISectionsCollection();

            if (string.IsNullOrWhiteSpace(pathToIni))
                throw new ArgumentNullException("INI Path cannot be blank");

            if (!File.Exists(pathToIni))
                throw new ArgumentException($"the path to the INI file does not exist: {pathToIni}");

            PathToIniFile = pathToIni;
            Read();
        }

        protected void Read()
        {
            var allLines = File.ReadLines(PathToIniFile);

            INISection currentSection = null;

            foreach (var line in allLines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var curLine = line.Trim();
                var currentSectionName = string.Empty;

                // comment lines
                if (curLine.StartsWith("//")) continue;
                if (curLine.StartsWith("#")) continue;
                if (curLine.StartsWith("--")) continue;

                if (curLine.StartsWith("["))
                {
                    currentSectionName = curLine.Trim('[', ' ', ']', '\t');
                    var tempSection = new INISection(currentSectionName);

                    Sections.Add(currentSectionName, tempSection);
                    currentSection = Sections[currentSectionName];
                    continue;
                }

                // at this point, it is a key value pair
                var splitPos = curLine.IndexOf('=');
                if (splitPos > 0 && currentSection != null)
                {
                    var key = curLine.Substring(0, splitPos).Trim(' ', '\t');
                    var value = curLine.Substring(splitPos + 1).Trim(' ', '\t');

                    if (string.IsNullOrWhiteSpace(key))
                        throw new ArgumentOutOfRangeException($"Unidentified key/value pair in config section '{currentSectionName}' - {curLine}");

                    currentSection.Add(key, value);
                }
            }

        }
    }



    public class INISection : IEnumerable<KeyValuePair<string, string>>
    {
        public string Name { get; protected set; }

        protected Dictionary<string, string> _values = null;

        public INISection(string name)
        {
            _values = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            this.Name = name;
        }

        public string this[string key]
        {
            get
            {
                return ContainsKey(key) ? _values[key] : null;
            }
        }

        public bool ContainsKey(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
                return _values.ContainsKey(key);

            return false;
        }

        public void Add(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key)) return;

            if (ContainsKey(key))
                _values[key] = value;
            else
                _values.Add(key, value);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        //public IDictionary<string,string> Lines { get { return _values; } }
        
    }

    public class INISectionsCollection : Dictionary<string, INISection>
    {
        public INISectionsCollection() : base(StringComparer.InvariantCultureIgnoreCase)
        {

        }

        public new INISection this[string name]
        {
            get
            {
                return this.ContainsKey(name) ? base[name] : null;
            }
        }

        public new bool ContainsKey(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
                return base.ContainsKey(key);

            return false;
        }
    }

    public class INIEntry
    {
        public int LineNumber { get; protected set; }
        public bool IsComment { get; protected set; }
        public bool IsBlank { get; protected set; }
        public bool IsSectionName { get; protected set; }
        public string InlineComment { get; set; }
        public string TrimLine { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        protected string _rawLine = null;

        public INIEntry(string line, int lineNumber = -1)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                this.IsBlank = true;
                this.LineNumber = LineNumber;
                _rawLine = line;
                return;
            }

            var curLine = line.Trim();
            TrimLine = curLine;
            if (curLine.StartsWith("\\") || curLine.StartsWith("#") || curLine.StartsWith("--"))
            {
                this.IsComment = true;
                return;
            }

            // ~~ not going to support inline comments right now
            ////var commentPos = curLine.IndexOf("\\*");
            ////// inline comment
            ////if (commentPos >= 0)
            ////{
            ////    this.InlineComment = curLine.Substring(commentPos + 1);
            ////    var endPos = this.InlineComment.IndexOf("*\\");
            ////    if (endPos > 0)
            ////        this.InlineComment = this.InlineComment.Substring(0, endPos);

            ////    if (commentPos == 0)
            ////        curLine = null;
            ////    else 
            ////        curLine = curLine.Substring(0, commentPos).Trim();

            ////    if (string.IsNullOrWhiteSpace(curLine))
            ////    {
            ////        this.IsComment = true;
            ////        return;
            ////    }
            ////}

            if (curLine.StartsWith("["))
            {
                this.IsSectionName = true;
                this.TrimLine = curLine.Trim('[', ' ', '\t', ']');
                return;
            }

            // at this point, it is a key value pair
            var splitPos = curLine.IndexOf('=');
            if (splitPos > 0)
            {
                this.Key = curLine.Substring(0, splitPos).Trim(' ', '\t');
                this.Value = curLine.Substring(splitPos + 1).Trim(' ', '\t');
            }
        }
    }
}
