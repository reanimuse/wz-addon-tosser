using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WzWoWLib.interfaces;
using System.IO;

namespace WzWoWLib.Main
{
    public class WzFileInfo : IWzFileInfo
    {
        protected string ext { get; set; }
        public string Name { get; protected set; }
        public string FullName { get; protected set; }

        public WzFileInfo(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath)) throw new ArgumentException("Full path to file cannot be null or whitespace");
            if (!WzFileInfo.FileExists(fullPath)) throw new FileNotFoundException($"'{fullPath}' does not exist");

            var info = new FileInfo(fullPath);

            this.FullName = info.FullName;
            this.Name = info.Name;
            this.ext = info.Extension?.Trim('.').ToLower();
        }

        public bool Exists {
            get
            {
                return WzFileInfo.FileExists(this.FullName);
            }
        }

        public IWzVersionInfo GetVersion()
        {
            WzVersionInfo result = null;

            if (ext == "exe" || ext == "dll")
            {
                var version = System.Diagnostics.FileVersionInfo.GetVersionInfo(this.FullName);
                result = new WzVersionInfo(version);
            }
            return result;
        }

        public static bool FileExists(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;

            return File.Exists(path);
        }
    }
}
