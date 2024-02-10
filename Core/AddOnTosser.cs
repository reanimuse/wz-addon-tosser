using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using WzAddonTosser.Core.Interfaces;
using WzAddonTosser.Core.Logging;

namespace WzAddonTosser.Core
{
    /// <summary>
    /// top level entrypoint that starts the processing of all addon zip files in the specified folder
    /// </summary>
    public class AddOnTosser
    {
        public AddOnTosser(IWzLogger logger)
        {
            var Logfile = new Logfile(TosserConfig.Current.LogFolder.FullName);
            Logger.RegisterLogger(Logfile);

            Logger.RegisterLogger(logger);
        }
        public virtual void Process()
        {
            foreach (var installedVariation in TosserConfig.Current.WoWVariationFolders)
            {
                ProcessVariation(installedVariation.Key, installedVariation.Value);
            }
        }

        protected void ProcessVariation(WoWVariation variation, ITosserConfigFolders variationFolders) 
        {

            Logger.Current.Log(EntryType.Information, "Downloads for variation : {0}", variation);

            Logger.Current.Log(EntryType.Information, "   Downloads Folder  : {0}", variationFolders.SourceFolder.FullName);
            Logger.Current.Log(EntryType.Information, "   WoW Folder        : {0}", variationFolders.VariationRootFolder);
            Logger.Current.Log(EntryType.Information, "   Backup Folder     : {0}", variationFolders.BackupFolder);

            var files = variationFolders.SourceFolder.GetFiles("*.zip", SearchOption.TopDirectoryOnly);

            if (files.Length < 1)
            {
                Logger.Current.Log(EntryType.Warning, "   No addons found!");
                return;
            }

            foreach (var addonFile in files)
            {
                var handler = new AddOnHandler(addonFile);

                if (!handler.IsValidAddonArchive)
                {
                    Logger.Current.Log(EntryType.Unexpected, "   The archive '{0}' is not a valid AddOn archive", addonFile.FullName);
                }
                else
                {
                    handler.Process();
                }
            }
        }

        public virtual void History()
        {

            foreach (var installedVariation in TosserConfig.Current.WoWVariationFolders)
            {
                Logger.Current.Log("Variation: {0}", installedVariation.Key);

                var files = installedVariation.Value.ZipFileHistoryFolder.GetFiles("*.zip", SearchOption.TopDirectoryOnly);

                foreach (var addonFile in files)
                {
                    Logger.Current.Log("   {0}", addonFile.Name);
                }
            }
        }
    }
}
