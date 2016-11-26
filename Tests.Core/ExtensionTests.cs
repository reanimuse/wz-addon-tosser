using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using WzAddonTosser.Core;

namespace Tests.Core
{
    [TestClass]
    public class ExtensionTests
    {
        [TestMethod]
        public void Ext_DirectoryInfo_Combine_Normal()
        {
            var rootPath = "F:\\Junk";
            var basePath = rootPath + "\\Path";
            var info = new DirectoryInfo(basePath);

            Assert.IsNotNull(info);

            var newPath = info.Combine();
            Assert.IsTrue(newPath == basePath, "Unexpected result '{0}' when combining zero parameters", newPath);


            newPath = info.Combine("One", "Two", "Three");
            Assert.IsTrue(newPath == basePath + @"\One\Two\Three", "Unexpected result '{0}' when combining three parameters", newPath);

            newPath = info.Combine(@"One\Two", "Three");
            Assert.IsTrue(newPath == basePath + @"\One\Two\Three", "Unexpected result '{0}' when combining partial parameters", newPath);

            newPath = info.Combine(@"\One\Two", "Three");
            Assert.IsTrue(newPath == basePath + @"\One\Two\Three", "Unexpected result '{0}' when combining partial parameters with root path", newPath);

            newPath = info.Combine(@".\One\Two", "Three");
            Assert.IsTrue(newPath == basePath + @"\One\Two\Three", "Unexpected result '{0}' when combining partial parameters with current root", newPath);

            newPath = info.Combine(@"..\One\Two", "Three");
            Assert.IsTrue(newPath == basePath + @"\One\Two\Three", "Unexpected result '{0}' when combining partial parameters with parent path", newPath);

            newPath = info.Combine(true, @"..\One\Two", "Three");
            Assert.IsTrue(newPath == rootPath + @"\One\Two\Three", "Unexpected result '{0}' when combining partial parameters with parent paths allowed", newPath);

            newPath = info.Combine(true, @"..\..\One\Two", "Three");
            Assert.IsTrue(newPath == @"F:\One\Two\Three", "Unexpected result '{0}' when combining partial parameters with two parent paths", newPath);

            newPath = info.Combine(true, @"..\..\..\One\Two", "Three");
            Assert.IsTrue(newPath == @"F:\One\Two\Three", "Unexpected result '{0}' when combining partial parameters with parent paths past the root", newPath);

        }
    }
}
