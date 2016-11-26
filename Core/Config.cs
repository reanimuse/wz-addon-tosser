using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using WzAddonTosser.Core.Logging;
using WzAddonTosser.Core.Interfaces;

namespace WzAddonTosser.Core
{
    public class TosserConfig
    {
        public string ProgramName = null;
        public string BatchID = null;
        public DirectoryInfo SourceFolder { get; protected set; }
        public DirectoryInfo WorkingFolder { get; protected set; }
        public DirectoryInfo WorkingFolderBase { get; protected set; }
        public DirectoryInfo BackupFolder { get; protected set; }
        public DirectoryInfo LogFolder { get; protected set; }
        public DirectoryInfo ZipFileHistoryFolder { get; protected set; }

        public WowFolders WoWFolder { get; protected set; }

        private static TosserConfig _current = null;

        private static Dictionary<string, bool> _invalidFolders = null;

        public static TosserConfig Current
        {
            // this contruct is needed so that 1) the config is loaded on demand and 2) any errors thrown do not get wrappered in a TypeInitializerException
            get
            {
                if (_current == null)
                {
                    BuildInvalidFolderList();
                    _current = new TosserConfig();
                }

                return _current;
            }
        }

        protected static void BuildInvalidFolderList()
        {
            var tempDirs = new Dictionary<string, bool>(StringComparer.InvariantCultureIgnoreCase);
            var paths = Environment.GetEnvironmentVariable("PATH").Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();

            foreach(var curPath in paths )
            {
                AddInvalidFolder(curPath);
            }

            var otherKeys = Enum.GetValues(typeof(Environment.SpecialFolder));
            foreach (Environment.SpecialFolder key in otherKeys)
            {
                var newPath = Environment.GetFolderPath(key);
                AddInvalidFolder(newPath);
            }
        }


        protected static void AddInvalidFolder(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath)) return;
            if (_invalidFolders == null)
                _invalidFolders = new Dictionary<string, bool>(StringComparer.InvariantCultureIgnoreCase);

            var checkPath = folderPath.TrimEnd('\\') + "\\";
            if (!_invalidFolders.ContainsKey(checkPath)) _invalidFolders.Add(checkPath, true);
        }


        protected static bool IsInvalidFolder(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath)) return true;

            var checkPath = folderPath.TrimEnd('\\') + "\\";

            return _invalidFolders.ContainsKey(checkPath);
        }



        protected TosserConfig()
        {
            ProgramName = "WowModTosser";
            BatchID = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            SourceFolder = GetDirInfo("SourceDir", true, true);
            WorkingFolderBase = GetDirInfo("WorkingDir", false, true);
            BackupFolder = GetDirInfo("BackupDir", false, true);

            var wowFolder = GetDirInfo("WoWDir", true, true);


            if (WorkingFolderBase == null)
            {
                WorkingFolderBase = MakeAppPath("Working");
            }

            ValidateDirIsNotSystem(WorkingFolderBase);
            CleanWorkingDirBase();

            // now create the temp path used for this run
            var thisBatchDirName = string.Format("WZAT_{0:yyyyMMdd_hhmmss_fff}", DateTime.Now);
            var newPath = Path.Combine(WorkingFolderBase.FullName, thisBatchDirName);
            WorkingFolder = Directory.CreateDirectory(newPath);



            if (BackupFolder == null)
            {
                BackupFolder = MakeAppPath("Backup");
            }

            LogFolder = MakeAppPath("Logs");
            ZipFileHistoryFolder = MakeAppPath("ZipHistory");
            

            WoWFolder = new WowFolders(wowFolder);

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


        protected DirectoryInfo MakeAppPath(string folderName)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            var path = Path.Combine(appData, this.ProgramName, folderName);
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

            foreach (var curPath in _invalidFolders.Keys)
            {
                if (curPath.StartsWith(workingPath, StringComparison.InvariantCultureIgnoreCase))
                    throw new ArgumentException("The directories specified cannot be a defined system path nor the parent of a system path : '" + workingPath + "'.  Please choose an empty folder as this folder will be cleared regularly");
            }
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
                    dir.Delete();
                } catch (Exception ex)
                {
                    Logger.Current.Log(EntryType.Unexpected, "Unable to delete folder '{0}', - {1}", dir.Name, ex.Message);
                }
            }
        }
    }
}
