using System.Linq;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using WiX.Build.Sdk.Tasks;
using WiX.Build.Sdk.Tasks.Tests.Actions;

namespace Wix.Build.Sdk.Tasks.Tests
{

    [TestClass]
    public class WixResolveCustomActionsTests
    {

        [TestMethod]
        public void Resolve_should_extract_customaction_for_decorated_method()
        {
            var f = typeof(TestCustomAction).Assembly.Location;
            var a = WixResolveCustomActions.ResolveCustomActions(f).ToList();
            a.Should().HaveCount(1);
            a[0].Id.Should().Be("CustomAction_WiX_Build_Sdk_Tasks_Tests_Actions_TestCustomAction_Test");
            a[0].SourceFile.Should().Be(f);
            a[0].DllEntry.Should().Be(nameof(TestCustomAction.Test));
            a[0].Execute.Should().Be("immediate");
            a[0].Return.Should().Be("check");
        }

    }

}
