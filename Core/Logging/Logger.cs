using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WzAddonTosser.Core.Interfaces;

namespace WzAddonTosser.Core.Logging
{
    class Logger : IWzLogger
    {
        public static IWzLogger Current { get; protected set; }

        protected static List<IWzLogger> _loggers;

        static Logger()
        {
            Current = new Logger();
            _loggers = new List<IWzLogger>();
        }

        public static void RegisterLogger(IWzLogger logger)
        {
            if (logger == null) throw new ArgumentNullException("specified logger cannot be null");

            _loggers.Add(logger);
        }

        public void Log(string msg, params object[] args)
        {
            foreach (var item in _loggers)
                item.Log(msg, args);
        }

        public void Log(EntryType entryType, string msg, params object[] args)
        {
            foreach (var item in _loggers)
                item.Log(entryType, msg, args);
        }

        public void LogException(Exception ex, string msg, params object[] args)
        {
            foreach (var item in _loggers)
                item.LogException(ex, msg, args);
        }

        public void LogException(Exception ex, EntryType entryType, string msg, params object[] args)
        {
            foreach (var item in _loggers)
                item.LogException(ex, entryType, msg, args);
        }
    }
}
