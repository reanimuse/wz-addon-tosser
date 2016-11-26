using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WzAddonTosser.Core;
using WzAddonTosser.Core.Common;

namespace WzAddonTosser.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            //INIFile reader;
            //if (args.Length < 0)
            //{
            //    reader = new WzAddonTosser.Core.Common.INIFile(@"D:\Dev\TestApps\Windows\WowModtosser\ModTosserMain\Config_Sample.ini");
            //}


            var logger = new ConsoleLogger();

            logger.Log(ConsoleColor.Green, "Starting");

            var t = new AddOnTosser(logger);

            t.Process();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\r\nPress any key to continue ");
            Console.ReadKey();
        }
    }
}
