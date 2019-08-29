using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WzAddonTosser.Core
{

    // for handling WoW Folders
    public class WowFolders : IWowFolders
    {
        public const string ROOT_EXE_NAME = "World of Warcraft Launcher.exe";
        public const string APP_EXE_NAME = "WoW.exe";
        public DirectoryInfo GameRootFolder { get; protected set; }
        public DirectoryInfo AddonsFolder { get; protected set; }
        public DirectoryInfo AddonDataFolder { get; protected set; }
        public DirectoryInfo VariationRootFolder { get; protected set; }


        public WoWVariation Variation { get; protected set; }


        public WowFolders(DirectoryInfo wowfolder, WoWVariation variation = WoWVariation.Retail)
        {
            bool isValid = wowfolder.GetFiles(ROOT_EXE_NAME).Length == 1;

            if (!isValid)
                throw new ArgumentException("the path '" + wowfolder.FullName + "' does not contain WoW");

            GameRootFolder = wowfolder;

            GetVariationFolder(variation);

            try
            {
                GetWoWFolders();
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"the path '{wowfolder.FullName}' {ex.Message}");
            }
        }

        protected string BuildVariationSubfolderPath(WoWVariation variation)
        {
            var baseFolderName = variation == WoWVariation.Classic ? "_classic_" : "_retail_";
            return Path.Combine(GameRootFolder.FullName, baseFolderName);
        }

        public bool ContainsVariation(WoWVariation variation)
        {
            return Directory.Exists(BuildVariationSubfolderPath(variation));
        }

        protected void GetVariationFolder(WoWVariation variation)
        {
            var refFolder = BuildVariationSubfolderPath(variation);
            if (!Directory.Exists(refFolder))
                throw new ArgumentException($"Game folder does not contain the '{refFolder}' folder and might be an outdated version of wow");

            var tempDir = new DirectoryInfo(refFolder);

            var isVariationValid = tempDir.GetFiles(APP_EXE_NAME).Length == 1;
            if (!isVariationValid)
                throw new ArgumentException($"the path '{refFolder}' does not contain the '{APP_EXE_NAME}' executable");
            VariationRootFolder = tempDir;
            this.Variation = variation;
        }

        protected void GetWoWFolders()
        {
            var interfaceFolder = VariationRootFolder.GetDirectory("Interface");
            if (interfaceFolder == null)
                throw new ArgumentException($"does not contain an {VariationRootFolder.Name}\\Interface\\ subfolder");

            AddonsFolder = interfaceFolder.GetDirectory("Addons");
            if (AddonsFolder == null)
                throw new ArgumentException($"does not contain an {VariationRootFolder.Name}\\Interface\\Addons\\ subfolder");

            AddonDataFolder = VariationRootFolder.GetDirectory("WTF");
            if (AddonDataFolder == null)
                throw new ArgumentException($"does not contain an {VariationRootFolder.Name}\\WTF\\ subfolder");
        }
    }
}
