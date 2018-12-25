using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WzAddonTosser.Core
{
    public class WoWBuildInfo
    {
        public IReadOnlyDictionary<string, string> BuildInfoParams;

        public string Version { get; protected set; }

        public bool IsValid
        {
            get
            {
                return BuildInfoParams.Count > 0;
            }
        }

        public WoWBuildInfo(WowFolders wowFolder)
        {

            var buildFile = wowFolder.GameRootFolder.GetFiles(".build.info").FirstOrDefault();
            if (buildFile == null) return;

            var rawData = File.ReadAllText(buildFile.FullName).Trim();
            ParseData(rawData);
        }

        protected void ParseData(string data)
        {
            //Branch!STRING:0|Active!DEC:1|Build Key!HEX:16|CDN Key!HEX:16|Install Key!HEX:16|IM Size!DEC:4|CDN Path!STRING:0|CDN Hosts!STRING:0|CDN Servers!STRING:0|Tags!STRING:0|Armadillo!STRING:0|Last Activated!STRING:0|Version!STRING:0|Product!STRING:0|Build Complete!DEC:1|BGDL Complete!DEC:1
            //us | 1 | 1a0b0cbe3ed11143a44804dc8009b564 | d1f253b54f939ae242c91a7f52435b5e | 1ad5f2f0f6b209e4aa1fb4c84aebf825 || tpr / wow | us.cdn.blizzard.com level3.blizzard.com | http://level3.blizzard.com/?maxhosts=4 http://us.cdn.blizzard.com/?maxhosts=4 https://blzddist1-a.akamaihd.net/?fallback=1&maxhosts=4 https://level3.ssl.blizzard.com/?fallback=1&maxhosts=4 https://us.cdn.blizzard.com/?fallback=1&maxhosts=4|Windows x86_64 US? enUS speech?:Windows x86_64 US? enUS text?||2018-12-25T15:10:27Z|8.1.0.28833|wow|1|1


            if (string.IsNullOrEmpty(data)) return;

            var lines = data.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length != 2) return;

            var fields = lines[0].Split('|');
            var values = lines[1].Split('|');

            var buildInfo = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            for (var i = 0; i< fields.Length; i++)
            {
                var item = EvaluatePair(fields[i], values[i]);
                buildInfo.Add(item.Key, item.Value);
            }

            BuildInfoParams = new ReadOnlyDictionary<string, string>(buildInfo);

            if (BuildInfoParams.ContainsKey("version")) this.Version = BuildInfoParams["version"];
        }

        protected KeyValuePair<string, string> EvaluatePair(string header, string value)
        {
            // for now, just treat everything as strings.

            var name = header;
            var pos = header.IndexOf("!");
            if (pos >= 0) name = header.Substring(0, pos);

            var result = new KeyValuePair<string, string>(name, value);

            return result;
        }
    }
}
