using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace WzAddonTosser.Core
{
    class AddOnModule
    {
        public string Name { get; protected set;  }
        public AddOnTOC TOC { get; protected set;  }

        public DirectoryInfo WorkingDir { get; protected set; }

        public string InstallPath { get; protected set; }

        public bool Processed { get;  set;}


        public AddOnModule(DirectoryInfo source)
        {
            Name = source.Name;
            WorkingDir = source;

            InstallPath = Path.Combine(TosserConfig.Current.WoWFolder.AddonsFolder.FullName, Name);

            TOC = new AddOnTOC(source);
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
