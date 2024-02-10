using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WzAddonTosser.Main.ConsoleIO
{
    internal class ConsoleColorPair
    {
        public ConsoleColor OriginalForegroundColor { get; protected set; }
        public ConsoleColor OriginalBackgroundColor { get; protected set; }
        public ConsoleColor ForegroundColor { get; protected set; }
        public ConsoleColor BackgroundColor { get; protected set; }
        public ConsoleColorPair()
        {
            OriginalBackgroundColor = Console.BackgroundColor;
            OriginalForegroundColor = Console.ForegroundColor;

            ForegroundColor = OriginalForegroundColor;
            BackgroundColor = OriginalBackgroundColor;
        }
        public ConsoleColorPair(ConsoleColor foregroundColor) : this()
        {
            ForegroundColor = foregroundColor;
        }
        public ConsoleColorPair(ConsoleColor foregroundColor, ConsoleColor backgroundColor) : this(foregroundColor)
        {
            BackgroundColor = backgroundColor;
        }

        public void SetToPairColors()
        {
            Console.ForegroundColor = ForegroundColor;
            Console.BackgroundColor = BackgroundColor;
        }

        public void ResetToOriginalColors()
        {
            Console.ForegroundColor = OriginalForegroundColor;
            Console.BackgroundColor = OriginalBackgroundColor;
        }
    }
}
