using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WzAddonTosser.Core
{
    // for handling WoW Folders
    public class WowFolders
    {
        public DirectoryInfo AddonsFolder { get; protected set; }
        public DirectoryInfo AddonDataFolder { get; protected set; }

        public string FullName { get; protected set; }

        protected DirectoryInfo rootFolder = null;

        public WowFolders(DirectoryInfo wowfolder)
        {
            bool isValid = wowfolder.GetFiles("wow.exe").Length == 1;
            if (!isValid)
                throw new ArgumentException("the path '" + wowfolder.FullName + "' does not contain WoW");

            rootFolder = wowfolder;
            this.FullName = rootFolder.FullName;

            AddonsFolder = wowfolder.GetDirectory("Interface").GetDirectory("Addons");
            AddonDataFolder = wowfolder.GetDirectory("WTF");
        }
    }
}
