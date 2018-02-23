using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using WzWoWLib.interfaces;

namespace WzWoWLib.Main
{
    public class WzFolderInfo : IWzFolderInfo
    {
        public string Name { get; protected set; }
        public string FullName { get; protected set; }

        public WzFolderInfo(string path)
        {
            if (!WzFolderInfo.FolderExists(path)) throw new DirectoryNotFoundException($"'{path}' does not exist");
            var info = new DirectoryInfo(path);

            this.FullName = info.FullName;
            this.Name = info.Name;
        }


        public bool Exists {
            get
            {
                return WzFolderInfo.FolderExists(this.FullName);
            }
        }

        public IWzFolderInfo GetSubfolder(params string[] paths)
        {
            var newPaths = new string[paths.Length + 1];
            newPaths[0] = this.FullName;

            paths.CopyTo(newPaths, 1);

            var newPath = Path.Combine(newPaths);
            return new WzFolderInfo(newPath);
        }
        
        public IWzFileInfo GetFile(string fileName, bool recurse = false)
        {
            var doRecurse = recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            if (string.IsNullOrWhiteSpace(fileName)) return null;


            var folder = new DirectoryInfo(this.FullName);

            var files = folder.GetFiles(fileName, doRecurse);

            if (files == null || files.Length == 0) return null;

            return new WzFileInfo(files[0].FullName);

        }



        public static bool FolderExists(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;
            return Directory.Exists(path);
        }
    }
}
