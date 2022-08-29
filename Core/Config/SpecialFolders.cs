using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WzAddonTosser.Core.Config
{
    public class SpecialFolders
    {
        private static Dictionary<string, bool> _invalidFolders = null;

        static SpecialFolders() {
            _invalidFolders = new Dictionary<string, bool>(StringComparer.InvariantCultureIgnoreCase);
            BuildSpecialFolderList();
        }

        protected static void BuildSpecialFolderList()
        {
            // since this process deletes folder paths, this serves as a safeguard
            // so that no system paths are inadvertantly cleaned.

            // No paths in the PATH variable can be used for this process
            var paths = Environment.GetEnvironmentVariable("PATH").Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();

            foreach (var curPath in paths)
            {
                AddInvalidFolder(curPath);
            }

            // Likewise, no 'special folders' can be used either.
            var otherKeys = Enum.GetValues(typeof(Environment.SpecialFolder));
            foreach (Environment.SpecialFolder key in otherKeys)
            {
                var newPath = Environment.GetFolderPath(key);
                AddInvalidFolder(newPath);
            }
        }


        protected static void AddInvalidFolder(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath)) return;

            var checkPath = folderPath.TrimEnd('\\') + "\\";
            if (!_invalidFolders.ContainsKey(checkPath)) _invalidFolders.Add(checkPath, true);
        }


        public static bool IsSpecialFolder(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath)) return true;

            var checkPath = folderPath.TrimEnd('\\') + "\\";

            if (_invalidFolders.ContainsKey(checkPath))
                return true;

            // make sure the path specified isn't a parent of a special folder path
            foreach (var specialPath in _invalidFolders.Keys)
                if (specialPath.StartsWith(checkPath, StringComparison.CurrentCultureIgnoreCase))
                    return true;

            return false;
        }

    }
}
