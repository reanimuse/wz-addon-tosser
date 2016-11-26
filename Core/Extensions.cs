using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using WzAddonTosser.Core.Interfaces;

namespace WzAddonTosser.Core
{
    static class Extensions
    {
        public static DirectoryInfo GetDirectory(this DirectoryInfo dir, string name)
        {
            return dir.GetDirectories(name, SearchOption.TopDirectoryOnly).FirstOrDefault();
        }

        public static void CopyTo(this DirectoryInfo dir, string targetDirectory, bool overWriteExisting = true)
        {
            if (!dir.Exists) throw new DirectoryNotFoundException("the source directory does not exist");

            var target = new DirectoryInfo(targetDirectory);

            CopyAll(dir, target, overWriteExisting);
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
    }
}


