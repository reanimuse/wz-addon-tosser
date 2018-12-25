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
        public const string ROOT_EXE_NAME = "World of Warcraft Launcher.exe";
        public DirectoryInfo GameRootFolder { get; protected set; }
        public DirectoryInfo AddonsFolder { get; protected set; }
        public DirectoryInfo AddonDataFolder { get; protected set; }

        public WowFolders(DirectoryInfo wowfolder)
        {
            bool isValid = wowfolder.GetFiles(ROOT_EXE_NAME).Length == 1;

            if (!isValid)
                throw new ArgumentException("the path '" + wowfolder.FullName + "' does not contain WoW");

            GameRootFolder = wowfolder;

            try
            {
                GetWoWFolders();
            } catch(Exception ex)
            {
                throw new ArgumentException($"the path '{wowfolder.FullName}' {ex.Message}");
            }
        }

        protected void GetWoWFolders() { 

            var refFolder = Path.Combine(GameRootFolder.FullName, "_retail_");

            if (!Directory.Exists(refFolder))
                throw new ArgumentException("does not contain the '_retail' subfolder and might be an outdated version of wow");

            var retailFolder = new DirectoryInfo(refFolder);

            var interfaceFolder = retailFolder.GetDirectory("Interface");
            if (interfaceFolder == null)
                throw new ArgumentException($"does not contain an {retailFolder.Name}\\Interface\\ subfolder");

            AddonsFolder = interfaceFolder.GetDirectory("Addons");
            if (AddonsFolder == null)
                throw new ArgumentException($"does not contain an {retailFolder.Name}\\Interface\\Addons\\ subfolder");

            AddonDataFolder = retailFolder.GetDirectory("WTF");
            if (AddonDataFolder == null)
                throw new ArgumentException($"does not contain an {retailFolder.Name}\\WTF\\ subfolder");
        }
    }
}
