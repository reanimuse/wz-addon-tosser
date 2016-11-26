using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using WzAddonTosser.Core.Interfaces;

namespace WzAddonTosser.Core
{
    public static class wzExtensions
    {
        public static DirectoryInfo GetDirectory(this DirectoryInfo dir, string name)
        {
            return dir.GetDirectories(name, SearchOption.TopDirectoryOnly).FirstOrDefault();
        }

        public static void CopyTo(this DirectoryInfo dir, string targetDirectory, bool overWriteExisting = true)
        {
            if (!dir.Exists) throw new DirectoryNotFoundException("the source directory does not exist");

            var target = new DirectoryInfo(targetDirectory);

            CopyTo(dir, target, overWriteExisting);
        }

        public static void CopyTo(this DirectoryInfo dir, DirectoryInfo targetDirectory, bool overWriteExisting = true)
        {
            if (!dir.Exists) throw new DirectoryNotFoundException("the source directory does not exist");

            CopyAll(dir, targetDirectory, overWriteExisting);
        }

        private static void CopyAll(DirectoryInfo source, DirectoryInfo target, bool overWriteExisting)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), overWriteExisting);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir, overWriteExisting);
            }
        }


        /// <summary>
        /// Compares the current value to a target value and returns -1 (less than target) 0 (equal to target) or 1 (greater than target)
        /// </summary>
        /// <param name="entry">the item being compared</param>
        /// <param name="compare">the value to compare the item to</param>
        /// <returns>0 if the two values are equal, -1 if the current value is less than the compare value or 1 if the current value is greater than the compare value</returns>
        public static int Compare(this EntryType entry, EntryType compare)
        {
            int val1 = (int)entry;
            int val2 = (int)compare;

            if (val1 < val2) return -1;
            if (val1 > val2) return 1;

            return 0;
        }

        /// <summary>
        /// Works similar to Path.Combine() except that it does not allow folders higher than 'dir'.  that is, if a path specified starts with \, it is limited to the folder specified in the 'dir' variable.
        /// parent paths (i.e. dot notation or "..\") are not allowed by default but can be enabled using allowParentPaths
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string Combine(this DirectoryInfo dir, params string[] paths)
        {
            return dir.Combine(false, paths);
        }
        public static string Combine(this DirectoryInfo dir, bool allowParentPaths, params string[] paths)
        {
            string curPath = dir.FullName;
            var allPaths = new string[paths.Length + 1];

            allPaths[0] = dir.FullName;

            if (paths.Length > 0)
            {
                for(int pos = 0; pos < paths.Length; pos++)
                {
                    string val = paths[pos];
                    if (!allowParentPaths)
                    {
                        val = val.Replace("..\\", "\\");
                        val = val.Replace(".\\", "\\");
                    }
                    allPaths[pos + 1] = val.Trim('\\');
                }
            }

            var result = Path.Combine(allPaths);

            // resolve any parent paths if specified
            if (result.IndexOf(".\\") >= 0)
                result = Path.GetFullPath(result);

            return result;
        }
    }
}


