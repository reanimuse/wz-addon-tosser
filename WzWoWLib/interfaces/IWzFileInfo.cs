using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WzWoWLib.interfaces
{

    public interface IWzIOInfoBase
    {
        string Name { get; }
        string FullName { get; }
        bool Exists { get; }
    }

    public interface IWzFolderInfo : IWzIOInfoBase
    {
        IWzFolderInfo GetSubfolder(params string[] paths);
        IWzFileInfo GetFile(string fileName, bool recurse = false);
    }

    public interface IWzFileInfo : IWzIOInfoBase
    {
        IWzVersionInfo GetVersion();
    }


    public interface IWzVersionInfo
    {
        int Major { get; }
        int Minor { get; }
        int Revision { get; }
        int Build { get; }
    }
}
