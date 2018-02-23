using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WzWoWLib.interfaces;

namespace WzWoWLib.Main
{
    public class WzVersionInfo : IWzVersionInfo
    {
        public string FullVersion { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Revision { get; set; }
        public int Build { get; set; }
        public int Details { get; set; }

        public WzVersionInfo(System.Diagnostics.FileVersionInfo info)
        {
            this.Major = info.FileMajorPart;
            this.Minor = info.FileMinorPart;
            this.Revision = info.FileBuildPart;
            this.Build = info.FilePrivatePart;
        }
    }
}
