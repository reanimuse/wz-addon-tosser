using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using WzAddonTosser.Core.Logging;
using WzAddonTosser.Core.Interfaces;
using WzAddonTosser.Core.Config;
using WzAddonTosser.Core.Extensions;

namespace WzAddonTosser.Core
{

    public class TosserConfig
    {
        public string ProgramName = null;
        public string BatchID = null;
        public DirectoryInfo WorkingFolder { get; protected set; }
        public DirectoryInfo WorkingFolderBase { get; protected set; }
        public DirectoryInfo BackupFolderBase { get; protected set; }

        public DirectoryInfo LogFolder { get; protected set; }

        // TODO: Zips should use the appropriate WoWVariationFolders
        [Obsolete]
        public DirectoryInfo ZipFileHistoryFolder { get; protected set; }

        // TODO: callers of this need to be updated to use the WoWVariationFolders for the right variation
        [Obsolete]
        public WowFolders WoWFolder { get; protected set; }
        protected WoWBuildInfo BuildInfo { get; set; }

        private static TosserConfig _current = null;

        public Dictionary<WoWVariation, ITosserConfigFolders> WoWVariationFolders { get; set; }

        public static TosserConfig Current
        {
            // this contruct is needed so that 1) the config is loaded on demand and 2) any errors thrown do not get wrappered in a TypeInitializerException
            get
            {
                if (_current == null)
                    _current = new TosserConfig();

                return _current;
            }
        }


        protected TosserConfig()
        {
            ProgramName = "WowModTosser";
            BatchID = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            this.WoWVariationFolders = new Dictionary<WoWVariation, ITosserConfigFolders>();

            var sourceFolder = GetDirInfo("SourceDir", true, true);
            WorkingFolderBase = BuildWorkingPathBase();
            BackupFolderBase = BuildBackupBase();
            LogFolder = MakeAppPath("Logs");
            ZipFileHistoryFolder = MakeAppPath("ZipHistory");

            var wowFolder = GetDirInfo("WoWDir", true, true);
            var classicWowFolder = GetDirInfo("ClassicWoWDir", false, true);

            CleanWorkingDirBase();


            // now create the temp path used for this run
            var thisBatchDirName = string.Format("WZAT_{0:yyyyMMdd_hhmmss_fff}", DateTime.Now);
            var newPath = Path.Combine(WorkingFolderBase.FullName, thisBatchDirName);
            WorkingFolder = Directory.CreateDirectory(newPath);

            
            WoWFolder = new WowFolders(wowFolder);
            if (wowFolder.ContainsVariation(WoWVariation.Retail)) {
                var wowTosserFolder = new TosserConfigFolders(wowFolder);
                this.WoWVariationFolders.Add(WoWVariation.Retail, wowTosserFolder);
            }

            if (classicWowFolder == null)
                classicWowFolder = wowFolder;

            if (classicWowFolder != null && classicWowFolder.ContainsVariation(WoWVariation.Classic))
            {
                var classicFolder = new TosserConfigFolders(classicWowFolder, WoWVariation.Classic);
                this.WoWVariationFolders.Add(WoWVariation.Classic, classicFolder);
            }

            foreach (var item in this.WoWVariationFolders)
            {
                var variationName = item.Key.ToString();
                item.Value.ZipFileHistoryFolder = MakeAppPath(variationName, "ZipHistory");
                item.Value.BackupFolder = MakeAppPath(BackupFolderBase, variationName, "Backup");
                item.Value.SourceFolder = sourceFolder;
            }

            BuildInfo = new WoWBuildInfo(WoWFolder);
        }


        protected DirectoryInfo BuildWorkingPathBase()
        {
            var basePath = GetDirInfo("WorkingDir", false, true);

            if (basePath == null)
                basePath = MakeAppPath("Working");

            ValidateDirIsNotSystem(basePath);

            return basePath;
        }

        protected DirectoryInfo BuildBackupBase()
        {
            var basePath = GetDirInfo("BackupDir", false, true);

            if (basePath == null)
                basePath = MakeAppPath("Backup");

            ValidateDirIsNotSystem(basePath);

            return basePath;
        }

        protected DirectoryInfo GetDirInfo(string key, bool keyMustExist, bool pathMustExist)
        {
            DirectoryInfo result = null;
            var path = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(path))
            {
                if (keyMustExist)
                    throw new ConfigurationErrorsException("Config path '" + key + "' must be specified");
                return result;
            }

            path = Environment.ExpandEnvironmentVariables(path);
            if (Directory.Exists(path))
            {
                result = new DirectoryInfo(path);
            }
            else
            {
                if (pathMustExist) 
                    throw new ConfigurationErrorsException(string.Format("the '{0}' resolved path '{1}' does not exist", key, path));
            }

                
            return result;

        }


        protected DirectoryInfo MakeAppPath(string folderName, string subFolder = null)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            var path = Path.Combine(appData, this.ProgramName, folderName, subFolder??"");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return new DirectoryInfo(path);
        }

        protected DirectoryInfo MakeAppPath(DirectoryInfo baseFolder, string folderName, string subFolder = null)
        {
            var path = Path.Combine(baseFolder.FullName, folderName, subFolder ?? "");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return new DirectoryInfo(path);
        }


        protected void ValidateDirIsNotSystem(DirectoryInfo folder)
        {
            if (folder.Parent == null)
                throw new ConfigurationErrorsException("Folder '" + folder.FullName + "' cannot be a root folder");

            // since this does recursive deletion, put a little bit of a safeguard in here 
            var workingPath = folder.FullName.TrimEnd('\\') + "\\";

            if (SpecialFolders.IsSpecialFolder(workingPath)) 
                throw new ArgumentException("The directories specified cannot be a defined system path nor the parent of a system path : '" + workingPath + "'.  Please choose an empty folder as this folder will be cleared regularly");
            
        }


        public void CleanWorkingDir()
        {
            var failed = false;
            var notValid = WorkingFolder.GetFiles().Length > 0;
            if (notValid)
                throw new ConfigurationErrorsException("Working folder '" + WorkingFolder.FullName + "' is not a valid target.  Please select an empty folder");

            var folders = WorkingFolder.GetDirectories();

            foreach (var folder in folders) {
                try
                {
                    folder.Delete(true);
                } catch (Exception ex)
                {
                    Logger.Current.LogException(ex, $"Unable to delete folder {folder.FullName}");
                    failed = true;
                }
            }

            if (failed) throw new ApplicationException("Unable to clean working folder");
        }


        public void CleanWorkingDirBase()
        {
            var dirs = WorkingFolderBase.GetDirectories("WZAT*", SearchOption.TopDirectoryOnly);

            if (dirs.Length < 0)
            {
                Logger.Current.Log(EntryType.Information, "Nothing to clean in the base working folder");
                return;
            }

            foreach(var dir in dirs)
            {
                try
                {
                    dir.Delete(true);
                } catch (Exception ex)
                {
                    Logger.Current.Log(EntryType.Unexpected, "Unable to delete folder '{0}', - {1}", dir.Name, ex.Message);
                }
            }
        }
    }
}
