using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WzAddonTosser.Core.Interfaces
{
    public enum EntryType : int  // the type of events raised
    {
        Information,
        Normal,
        Expected,   // an unusual but expected condition
        Severe,
        Warning,
        Unexpected,
        Critical,
        Fatal
    }


    public interface IWzLogger
    {
        void Log(string msg, params object[] args);
        void Log(EntryType entryType, string msg, params object[] args);
        void LogException(Exception ex, string msg, params object[] args);
        void LogException(Exception ex, EntryType entryType, string msg, params object[] args);
    }
}
