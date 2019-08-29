using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace WzAddonTosser.Core
{
    public class AddOnModule
    {
        public string Name { get; protected set;  }

        public AddOnTOC TOC { get; protected set;  }

        public DirectoryInfo WorkingDir { get; protected set; }

        public string InstallPath { get; protected set; }

        public bool Processed { get;  set;}

        public ITosserConfigFolders ConfigFolders { get; set; }


        public AddOnModule(DirectoryInfo source)
        {
            Name = source.Name;
            WorkingDir = source;

            TOC = new AddOnTOC(source);

            if (TosserConfig.Current.WoWVariationFolders.ContainsKey(TOC.Variation))
            {
                this.ConfigFolders = TosserConfig.Current.WoWVariationFolders[TOC.Variation];

                InstallPath = Path.Combine(this.ConfigFolders.AddonsFolder.FullName, Name);
            }
            else
            {
                throw new ApplicationException($"the addon '{source.Name}' is for a WoW variation '{TOC.Variation}' which is not handled in the config");
            }
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
