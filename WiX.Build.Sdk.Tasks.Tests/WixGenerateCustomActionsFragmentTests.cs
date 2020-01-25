
using Microsoft.VisualStudio.TestTools.UnitTesting;

using WiX.Build.Sdk.Tasks;

namespace Wix.Build.Sdk.Tasks.Tests
{

    [TestClass]
    public class WixGenerateCustomActionsFragmentTests
    {

        [TestMethod]
        public void Should_generate_proper_xml()
        {
            var xml = WixGenerateCustomActionsFragment.Generate(new[]
            {
                new WixCustomActionItem("CustomAction1", @"C:\Foo\Bar1.dll", "CustomAction1", "immediate", "check"),
                new WixCustomActionItem("CustomAction2", @"C:\Foo\Bar2.dll", "CustomAction2", "immediate", "check"),
            });
        }

    }

}
