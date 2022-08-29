using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WzAddonTosser.Core
{
    public class AddOnTOC
    {
        public string Version { get; protected set; }
        public string WoWVersion { get; protected set; }
        public string WowMinVersion { get; protected set; }
        public string Title { get; protected set; }
        public string Notes { get; protected set; }
        public string SavedVariables { get; protected set; }
        public string Website { get; protected set; }
        public string Author { get; protected set; }
        public string SavedVariablesPerCharacter { get; protected set; }
        public string RequiredMods { get; protected set; }
        public string CurseProjectId { get; protected set; }
        public string WoWiProjectId { get; protected set; }
        public string WagoProjectId { get; protected set; }
        public WoWVariation WowProgramVariation { get; protected set; }
        public List<string> Files { get; protected set; }

        protected Dictionary<string, string> _tocValues = null;

        public AddOnTOC(DirectoryInfo addOnFolder)
        {
            _tocValues = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            Files = new List<string>();

            if (addOnFolder != null && addOnFolder.Exists)
            {
                var name = addOnFolder.Name;
                var tocFile = addOnFolder.GetFiles(name + ".toc").FirstOrDefault();

                ReadTOCFile(tocFile);

                this.WoWVersion = GetValue("Interface");
                this.WowMinVersion = GetValue("X-Min-Interface");
                this.Version = GetValue("Version", "X-Curse-Packaged-Version");
                this.Title = GetValue("title");
                this.Notes = GetValue("notes");
                this.Author = GetValue("author");
                this.SavedVariables = GetValue("savedvariables");
                this.SavedVariablesPerCharacter = GetValue("savedvariablesPercharacter");
                this.Website = GetValue("website", "X-Website");
                this.RequiredMods = GetValue("RequiredDeps");
                this.WowProgramVariation = WoWVariation.Retail;

                this.CurseProjectId = GetValue("X-Curse-Project-ID");
                this.WoWiProjectId = GetValue("X-WoWI-ID");
                this.WagoProjectId = GetValue("X-Wago-ID");

                if (!string.IsNullOrWhiteSpace(this.WoWVersion))
                {
                    var majorVer = this.WoWVersion.Substring(0, 1);
                    if (majorVer == "1" || majorVer == "2" || majorVer == "3")
                        this.WowProgramVariation = WoWVariation.Classic;
                }
            }
        }


        public string GetValue(params string[] keys)
        {
            foreach (var key in keys)
            {
                if (_tocValues.ContainsKey(key))
                {
                    return _tocValues[key];
                }
            }

            return null;
        }


        protected void ReadTOCFile(FileInfo tocFile)
        {
            if (tocFile == null || !tocFile.Exists) return;

            var data = File.ReadAllText(tocFile.FullName);
            var lines = data.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach(var line in lines)
            {
                ParseLine(line);
            }
        }


        protected void ParseLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return;

            if (!line.StartsWith("##"))
            {
                // directive and comment lines are not files
                if (!line.StartsWith("#"))
                {
                    Files.Add(line.Trim());
                }
                return;
            }

            var seg = line.Substring(2).TrimStart();
            var split = seg.IndexOf(":");
            if (split>0)
            {
                var token = seg.Substring(0, split).Trim();
                var value = seg.Substring(split + 1).Trim();
                if (_tocValues.ContainsKey(token))
                {
                    var newVal = _tocValues[token] + ", " + value;
                    _tocValues[token] = newVal;
                }
                else
                {
                    _tocValues.Add(token, StripColorCodes(value));
                }
            }
        }

        protected string StripColorCodes(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;

            if (value.IndexOf('|') < 0) return value;

            string newVal = value.Replace("|r", "");

            var pos = newVal.IndexOf("|");
            var validcolorcodechars = "|01234567890abcdef".ToCharArray();

            while (pos>=0)
            {
                string code = null;
                try
                {
                    code = newVal.Substring(pos, 10);
                } catch
                {
                    code = newVal.Substring(pos);
                }

                var codeChars = code.ToCharArray();
                var isNotColorCode = codeChars.Any(x => validcolorcodechars.Contains(x) == false);

                if (isNotColorCode)
                {
                    pos = newVal.IndexOf("|", pos + 1);
                }
                else
                {
                    newVal = newVal.Replace(code, "");
                    pos = newVal.IndexOf("|");
                }
            }

            return newVal;
        }
    }
}
 