using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WzAddonTosser.Core.Interfaces;
using System.IO;

namespace WzAddonTosser.Core.Logging
{
    class Logfile : IWzLogger
    {
        protected string _logFileName = null;
        

        public Logfile(string logfolder)
        {
            var fileName = string.Format("WzTrace_{0:yyyyMMdd}.csv", DateTime.Now);

            _logFileName = System.IO.Path.Combine(logfolder, fileName);
        }

        public void Log(string msg, params object[] args)
        {
            this.Log(EntryType.Normal, msg, args);
        }

        public void Log(EntryType entryType, string msg, params object[] args)
        {
            var outMsg = string.Format(msg ?? "", args);
            WriteLogEntry(entryType, outMsg);
        }

        public void LogException(Exception ex, string msg, params object[] args)
        {
            this.LogException(ex, EntryType.Unexpected, msg, args);
        }

        public void LogException(Exception ex, EntryType entryType, string msg, params object[] args)
        {
            var outMsg = string.Format(msg ?? "", args);
            var stamp = DateTime.Now;

            WriteLogEntry(stamp, entryType, outMsg);

            if (ex != null)
            {
                var tempEx = ex;
                while (tempEx != null)
                {
                    WriteLogEntry(stamp, EntryType.Severe, "EXCEPTION: " + ex.Message);
                    tempEx = tempEx.InnerException;
                }

                if (!string.IsNullOrEmpty(ex.StackTrace ))
                {
                    WriteLogEntry(stamp, EntryType.Information, "StackTrace: " + ex.Message);
                }

            }
        }


        protected void WriteLogEntry(EntryType entryType, string msg)
        {
            WriteLogEntry(DateTime.Now, entryType, msg);
        }

        protected void WriteLogEntry(DateTime timestamp, EntryType entryType, string msg)
        {
            WriteHeaders();

            var line = string.Format("{0:MM/dd/yyyy HH:mm:ss.fff}, {1}, \"{2}\"\r\n", timestamp, entryType, Utils.MakeCSVSafe(msg));
            File.AppendAllText(_logFileName, line);
        }


        void WriteHeaders()
        {
            if (File.Exists(_logFileName)) return;
            File.WriteAllText(_logFileName, "Time,Type,Message\r\n");
        }
    }
}
