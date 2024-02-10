using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WzAddonTosser.Core.Extensions
{
    public static class DirInfoExtensions
    {
        public static bool ContainsVariation(this DirectoryInfo checkDir, WoWVariation variation)
        {
            var variationFolder = FindVariationFolderPath(checkDir, variation);
            return variationFolder != null;
        }

        private static DirectoryInfo FindVariationFolderPath(DirectoryInfo checkDir, WoWVariation variation)
        {
            var refFolder = BuildVariationSubfolderPath(checkDir, variation);
            if (!Directory.Exists(refFolder)) return null;

            var tempDir = new DirectoryInfo(refFolder);

            var isVariationValid = tempDir.GetFiles(WowFolders.APP_EXE_NAME).Length == 1;
            if (!isVariationValid) return null;

            return tempDir;
        }

        private static string BuildVariationSubfolderPath(DirectoryInfo checkDir, WoWVariation variation)
        {
            var baseFolderName = variation == WoWVariation.Classic ? "_classic_" : "_retail_";
            return Path.Combine(checkDir.FullName, baseFolderName);
        }

    }
}
