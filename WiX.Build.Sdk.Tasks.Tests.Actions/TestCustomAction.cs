using System;
using System.IO;

using Microsoft.Deployment.WindowsInstaller;

namespace WiX.Build.Sdk.Tasks.Tests.Actions
{
    public class TestCustomAction
    {

        [CustomAction]
        public static ActionResult Test(Session session)
        {
            try
            {
                File.AppendAllText(@"c:\tmp\time.txt", ": " + DateTime.Now.ToString());
            }
            catch (Exception)
            {
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

    }
}
