using System.IO;

namespace WzAddonTosser.Core
{
    public interface ITosserConfigFolders : IWowFolders
    {
        DirectoryInfo ZipFileHistoryFolder { get; set; }
        DirectoryInfo BackupFolder { get; set; }
    }

    public class TosserConfigFolders :WowFolders, ITosserConfigFolders
    {
        public DirectoryInfo ZipFileHistoryFolder { get; set; }
        public DirectoryInfo BackupFolder { get; set; }

        public TosserConfigFolders(DirectoryInfo wowFolder, WoWVariation variation = WoWVariation.Retail) : base(wowFolder, variation)
        {

        }
    }
}
