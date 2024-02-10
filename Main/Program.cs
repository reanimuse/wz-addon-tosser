using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WzAddonTosser.Core;
using WzAddonTosser.Core.Common;
using WzAddonTosser.Main.ConsoleIO;

namespace WzAddonTosser.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            var showHistory = false;


            foreach(string arg in args)
            {
                if (arg.StartsWith("-"))
                {
                    if (arg.ToLower() == "-history") showHistory = true;
                }
            }

            var logger = new ConsoleLogger();

#if DEBUG
            logger.Log(ConsoleColor.White, "{0} version {1} / build {2}", wzatVersionInfo.ProductName, wzatVersionInfo.Version, wzatVersionInfo.BuildVersion);
#else
            logger.Log(ConsoleColor.White, "{0} version {1}", wzatVersionInfo.ProductName, wzatVersionInfo.Version);
#endif
            logger.Log(ConsoleColor.Green, "Starting...");

            var t = new AddOnTosser(logger);

            if (showHistory)
                t.History();
            else 
                t.Process();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\r\nPress any key to continue ");
            Console.ReadKey();
        }
    }
}
