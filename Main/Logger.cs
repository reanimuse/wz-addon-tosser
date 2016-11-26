using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WzAddonTosser.Core.Interfaces;

namespace WzAddonTosser.Main
{
    class ConsoleLogger : IWzLogger
    {
        protected static Dictionary<EntryType, ConsoleColor> _colorDictionary = null;

        static ConsoleLogger()
        {
            _colorDictionary = new Dictionary<EntryType, ConsoleColor>();

            _colorDictionary.Add(EntryType.Information, ConsoleColor.DarkGray);
            _colorDictionary.Add(EntryType.Normal, ConsoleColor.Gray);
            _colorDictionary.Add(EntryType.Expected, ConsoleColor.White);
            _colorDictionary.Add(EntryType.Severe, ConsoleColor.Green);
            _colorDictionary.Add(EntryType.Warning, ConsoleColor.Yellow);
            _colorDictionary.Add(EntryType.Unexpected, ConsoleColor.Red);
        }


        public void Log(string msg, params object[] args)
        {
            Log(EntryType.Normal, msg, args);
        }

        public void Log(EntryType entryType, string msg, params object[] args)
        {
            Log(_colorDictionary[entryType], msg, args);
        }

        public void Log(ConsoleColor color, string msg, params object[] args)
        {
            var message = msg ?? string.Empty;
            if (args.Length > 0) message = string.Format(message, args);

            Console.ForegroundColor = color;
            Console.WriteLine(message);
        }

        public void LogException(Exception ex, string msg, params object[] args)
        {
            LogException(ex, EntryType.Unexpected, msg, args);
        }

        public void LogException(Exception ex, EntryType entryType, string msg, params object[] args)
        {
            var message = string.Format(msg ?? string.Empty, args);
            string errInfo = string.Empty;
            string stacktrace = string.Empty;
            

            if (ex != null)
            {
                errInfo = string.Format(" | {0} - {1}", ex.GetType().Name, ex.Message);

                if (!string.IsNullOrWhiteSpace(ex.StackTrace))
                {
                    stacktrace = string.Format("\r\nStackTrace: {0}", ex.StackTrace);
                }
            }
            Log(entryType, "{0}{1}{2}", message, errInfo, stacktrace);
        }
    }
}
