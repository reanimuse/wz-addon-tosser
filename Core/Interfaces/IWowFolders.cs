using System.IO;

namespace WzAddonTosser.Core
{
    public interface IWowFolders
    {
        DirectoryInfo AddonDataFolder { get; }
        DirectoryInfo AddonsFolder { get; }
        DirectoryInfo GameRootFolder { get; }
        DirectoryInfo VariationRootFolder { get; }

        bool ContainsVariation(WoWVariation variation);
    }
}