using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using WzAddonTosser.Core.Interfaces;
using WzAddonTosser.Core.Logging;

namespace WzAddonTosser.Core
{
    public class AddOnHandlerModule : AddOnModule
    {
        public DirectoryInfo FinalBackupFolder { get; set; }
        public AddOnHandlerModule(DirectoryInfo source) : base(source) 
        {
            
        }
    }

    public class AddOnHandler
    {
        protected FileInfo _sourceFile = null;
        protected string _backupFolder = null;

        public List<AddOnHandlerModule> Modules { get; protected set; }
        public string Name { get; protected set; }
        public string BaseName { get; protected set; }
        public bool IsValidAddonArchive { get; protected set; }
        public bool Processed { get; protected set; }
        public WoWVariation WowProgramVariation { get; protected set; }

        public AddOnHandler(string addonZipFilePath)
        {
            WowProgramVariation = WoWVariation.Unknown;
            var archiveFile = new FileInfo(addonZipFilePath);
            Init(archiveFile);
        }

        public AddOnHandler(FileInfo addonZipFilePath)
        {
            Init(addonZipFilePath);
        }

        protected void Init(FileInfo modArchiveFile)
        {
            Modules = new List<AddOnHandlerModule>();

            if (modArchiveFile == null)
                throw new ArgumentException("Path to addon file cannot be empty");

            if (modArchiveFile.Exists && modArchiveFile.Name.ToLower().EndsWith(".zip"))
            {
                _sourceFile = modArchiveFile;
                Name = _sourceFile.Name;
                BaseName = Name.Substring(0, Name.Length - _sourceFile.Extension.Length);
            }

            IsValidAddonArchive = ValidateAddonZip(modArchiveFile);
        }


        private bool ValidateAddonZip(FileInfo source)
        {
            try
            {
                using (var archive = ZipFile.OpenRead(source.FullName))
                {
                    if (archive == null || archive.Entries == null || archive.Entries.Count < 1) return false;

                    // it must have at least one .toc file
                    var tocs = archive.Entries.Where(x => x.Name.EndsWith(".toc", StringComparison.InvariantCultureIgnoreCase));
                    if (tocs == null || tocs.Count() < 1)
                    {
                        Logger.Current.Log(EntryType.Unexpected, "   Archive has no .toc file defined: '{0}'", source.Name);
                        return false;
                    }

                    // it must have at least one .toc file
                    var luas = archive.Entries.Where(x => x.Name.EndsWith(".lua", StringComparison.InvariantCultureIgnoreCase));
                    if (luas == null || luas.Count() < 1)
                    {
                        Logger.Current.Log(EntryType.Unexpected, "   Archive has no .lua files defined: '{0}'", source.Name);
                        return false;
                    }


                }
            }
            catch (Exception ex)
            {
                Logger.Current.LogException(ex, EntryType.Information, "Unable to open archive '{0}'", source.Name);
                return false;
            }

            return true;
        }

        
        protected void ExpandArchive()
        {
            Logger.Current.Log(EntryType.Expected, "Expanding: {0}", _sourceFile.Name);
            TosserConfig.Current.CleanWorkingDir();

            if (!IsValidAddonArchive)
            {
                Logger.Current.Log(EntryType.Unexpected, "The archive '{0}' is not a valid AddOn archive", _sourceFile.Name);
                return;
            }

            ExpandZip();

            var modules = TosserConfig.Current.WorkingFolder.GetDirectories();

            this.WowProgramVariation = WoWVariation.Retail;

            foreach (var modName in modules)
            {
                var mod = new AddOnHandlerModule(modName);
                Modules.Add(mod);
                if (mod.WowProgramVariation != WoWVariation.Retail) this.WowProgramVariation = mod.WowProgramVariation;
            }
            FixFolderDates(TosserConfig.Current.WorkingFolder, true);
        }


        protected void ExpandZip()
        {

            try
            {
                // the method below works 99% of the time but will fail if the archive contains two files of the same name in the same folder
                // this first appeared in MogIt-3.6.1.zip
                ZipFile.ExtractToDirectory(_sourceFile.FullName, TosserConfig.Current.WorkingFolder.FullName);

                return;
            }
            catch (IOException ex) when (ex.Message.IndexOf("already exists.") > 0)
            {
                Logger.Current.Log("Unexpected Error unpacking addon file - using fallback method");
            }

            TosserConfig.Current.CleanWorkingDir();

            //ZipArchive arch = null;
            //using (var stream = File.OpenRead(_sourceFile.FullName)) {
            using (ZipArchive arch = ZipFile.Open(_sourceFile.FullName, ZipArchiveMode.Read))
            {
                //arch = new ZipArchive(stream, ZipArchiveMode.Read);

                foreach (var entry in arch.Entries)
                {
                    var targetName = Path.Combine(TosserConfig.Current.WorkingFolder.FullName, entry.FullName);
                    if (File.Exists(targetName))
                    {
                        Logger.Current.Log(EntryType.Warning, "The file {0} exists twice in the archive. Please notify the addon author", entry.FullName);
                        File.Delete(targetName);
                    }

                    Directory.CreateDirectory(new FileInfo(targetName).DirectoryName);

                    using (var target = File.OpenWrite(targetName))
                    {
                        var buffer = new byte[entry.Length];
                        using (var data = entry.Open())
                        {
                            data.Read(buffer, 0, buffer.Length);
                            data.Close();
                        }

                        target.Write(buffer, 0, buffer.Length);
                        target.Close();
                    }
                }

                // stream.Close();
            }
        }


        protected void InstallModules()
        {
            AddOnModule lastMod = null;

            // do backup first
            foreach (var mod in this.Modules)
            {
                Backup(mod);
            }

            try
            {
                // now remove the old ones
                foreach (var mod in this.Modules)
                {
                    lastMod = mod;
                    DeleteInstalled(mod);
                }

                // now install the new ones
                foreach (var mod in this.Modules)
                {
                    lastMod = mod;
                    Install(mod);
                    mod.Processed = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Current.LogException(ex, "Error installing mod '{0}': {1}", lastMod?.Name, ex.Message);
                // do restore
                // TODO: handle restore
            }


            this.Processed = true;
        }


        public void Process()
        {
            ExpandArchive();
            InstallModules();
            Cleanup();
        }


        protected void DeleteInstalled(AddOnModule mod)
        {
            if (!Directory.Exists(mod.InstallPath)) return;

            Directory.Delete(mod.InstallPath, true);
        }


        protected void Backup(AddOnHandlerModule mod)
        {
            // not installed, nothing to backup
            if (!Directory.Exists(mod.InstallPath)) return;

            Logger.Current.Log("Backing up: {0}", mod.Name);

            var installedMod = new DirectoryInfo(mod.InstallPath);

            var tempBackupFolder = Path.Combine(mod.ConfigFolders.BackupFolder.FullName, TosserConfig.Current.BatchID, BaseName);
            if (Directory.Exists(tempBackupFolder)) Directory.CreateDirectory(tempBackupFolder);
            mod.FinalBackupFolder = new DirectoryInfo(tempBackupFolder);

            var modBackupFolder = BackupFolderForMod(mod);

            installedMod.CopyTo(modBackupFolder, true);

            //backup data files

            var dataFiles = mod.ConfigFolders.AddonDataFolder.GetFiles(mod.Name + ".lua*", SearchOption.AllDirectories);

            if (dataFiles.Length > 0)
            {
                var dataFolder = BackupFolderForData(mod);

                var startPos = mod.ConfigFolders.AddonDataFolder.FullName.Length + 1;
                foreach (var dataFile in dataFiles)
                {
                    var relPath = dataFile.Directory.FullName.Substring(startPos);
                    var destPath = Path.Combine(dataFolder, relPath);
                    if (!Directory.Exists(destPath)) Directory.CreateDirectory(destPath);

                    destPath = Path.Combine(destPath, dataFile.Name);
                    dataFile.CopyTo(destPath);
                }
            }
        }

        protected void Restore()
        {
            var backup = new DirectoryInfo(BackupFolder);

            var allDirs = backup.GetDirectories("*.*", SearchOption.AllDirectories);
            var allFiles = backup.GetFiles("*.*", SearchOption.AllDirectories);

            var installPath = TosserConfig.Current.WoWFolder.AddonsFolder;

            var backupRootPos = BackupFolder.Length;

            foreach(var dir in allDirs)
            {
                string relPath = dir.FullName.Substring(backupRootPos + 1);
                string targetPath = Path.Combine(installPath.FullName, relPath);

                // TODO: ensure that all of the directories exist
            }

            // TODO: copy all of the files from the backup back to the wow addon folder for this addon

        }


        protected void Install(AddOnModule mod)
        {
            Logger.Current.Log("Installing: {0}", mod.Name);
            mod.WorkingDir.CopyTo(mod.InstallPath, true);
        }


        public void Cleanup()
        {
            if (this.Processed)
            {
                Logger.Current.Log(EntryType.Normal, "Cleaning up {0}", _sourceFile.Name);
                var archiveFolder = Path.Combine(TosserConfig.Current.ZipFileHistoryFolder.FullName, _sourceFile.Name);
                _sourceFile.CopyTo(archiveFolder, true);
                _sourceFile.Delete();
            }
            else
            {
                Logger.Current.Log(EntryType.Warning, "Archive cleanup skipped as '{0}' was not processed", _sourceFile.Name);
            }
        }


        /// <summary>
        /// Updates the timestamps on the folders to match the last modification time in the files they contain
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="skipCurrentFolder"></param>
        protected void FixFolderDates(DirectoryInfo folder, bool skipCurrentFolder)
        {
            var lastUpdate = DateTime.MinValue;
            var firstCreate = DateTime.MaxValue;

            var files = folder.GetFiles("*.*", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                if (file.LastWriteTime > lastUpdate) lastUpdate = file.LastWriteTime;
                if (file.CreationTime < firstCreate) firstCreate = file.CreationTime;
            }

            foreach (var dir in folder.GetDirectories("*.*", SearchOption.TopDirectoryOnly))
            {
                FixFolderDates(dir, false);
                var recheck = new DirectoryInfo(dir.FullName);
                if (recheck.LastWriteTime > lastUpdate) lastUpdate = recheck.LastWriteTime;
                if (recheck.CreationTime < firstCreate) firstCreate = recheck.CreationTime;
            }

            if (!skipCurrentFolder)
            {
                if (lastUpdate != DateTime.MinValue)
                    Directory.SetLastWriteTime(folder.FullName, lastUpdate);

                if (lastUpdate < firstCreate) firstCreate = lastUpdate;

                if (firstCreate != DateTime.MaxValue)
                    Directory.SetCreationTime(folder.FullName, firstCreate);
            }
        }

        protected string BackupFolder
        {
            get
            {
                if (_backupFolder == null)
                {
                    _backupFolder = Path.Combine(TosserConfig.Current.BackupFolderBase.FullName, TosserConfig.Current.BatchID, BaseName);
                    if (!Directory.Exists(_backupFolder)) Directory.CreateDirectory(_backupFolder);
                }

                return _backupFolder;
            }
        }

        protected string BackupFolderForMod(AddOnHandlerModule mod)
        {
            var modBackupFolder = Path.Combine(mod.ConfigFolders.BackupFolder.FullName, "Interface", "Addons", mod.Name);
            if (!Directory.Exists(modBackupFolder)) Directory.CreateDirectory(modBackupFolder);
            return modBackupFolder;
        }

        protected string BackupFolderForData(AddOnHandlerModule mod)
        {
            return Path.Combine(BackupFolder, "WTF");
        }
    }
}
