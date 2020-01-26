using System;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

public class ShowMetadata :
    Microsoft.Build.Utilities.Task
{

    /// <summary>
    /// References to include in the VBP.
    /// </summary>
    public ITaskItem[] Source { get; set; }

    /// <summary>
    /// Executes the task.
    /// </summary>
    /// <returns></returns>
    public override bool Execute()
    {
        if (Source != null)
            foreach (var item in Source)
                foreach (var name in item.MetadataNames)
                    Log.LogMessage("Metadata: {0} {1}={2}", item.ItemSpec, name, item.GetMetadata(name.ToString()));

        return true;
    }

}

