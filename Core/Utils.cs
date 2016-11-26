using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WzAddonTosser.Core
{
    public class Utils
    {
        protected static char[] UnsafeCSVChars = new char[] { '\n', '\r', '\"' };

        public string MakeFileSystemSafeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("a file name is required");

            var badchars =      @"?*\/:|<>""";
            var replaceChars =   "!~__;]{}'".ToCharArray();

            var curChars = name.Trim().ToCharArray();

            for (int pos = 0; pos < curChars.Length; pos++)
            {
                char ch = curChars[pos];
                var found = badchars.IndexOf(ch);
                if (found>=0)
                    curChars[pos] = replaceChars[found];
            }

            return new string(curChars);
        }


        public static string MakeCSVSafe(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            bool needsEscape = value.IndexOfAny(UnsafeCSVChars) >= 0;
            if (!needsEscape) return value;

            var result = value.Replace("\r\n", " ");
            result = result.Replace("\"", "'");
            result = result.Replace("\n", " ");
            result = result.Replace("\r", " ");

            return result;
        }
    }
}
