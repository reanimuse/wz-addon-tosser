using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using WzWoWLib.Main;
using WzWoWLib.interfaces;

namespace WzWoWLib
{
    public class WzWoWLibMain
    {
        public IWzFolderInfo WoWFolder { get; }
        public IWzFolderInfo AddonsFolder { get; }
        public IWzFolderInfo WTFFolder { get; }

        public IWzVersionInfo InstalledWoWVersion { get;  }

        public WzWoWLibMain(string pathToWowDir)
        {
            if (string.IsNullOrWhiteSpace(pathToWowDir)) throw new ArgumentException("the pathToWowDir must not be null or blank");

            this.WoWFolder = new WzFolderInfo(pathToWowDir);
            this.AddonsFolder = this.WoWFolder.GetSubfolder("interface", "addons");
            this.WTFFolder = this.WoWFolder.GetSubfolder("WTF");

            var wowExePath = this.WoWFolder.GetFile("wow.exe");

            if (wowExePath == null) throw new FileNotFoundException($"The Wow.exe file was not found in the path: {pathToWowDir}");

            // at this point we probably have a valid wow install location

            this.InstalledWoWVersion = wowExePath.GetVersion();
        }
    }
}
