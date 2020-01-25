using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace WiX.Build.Sdk.Tasks
{

    /// <summary>
    /// Resolves custom action items from a given set of assemblies.
    /// </summary>
    public partial class WixResolveCustomActions : Task
    {

        /// <summary>
        /// Resolves the set of custom actions in the specified source assembly.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static IEnumerable<WixCustomActionItem> ResolveCustomActions(string source)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            if (File.Exists(source) == false)
                throw new FileNotFoundException("Unable to locate source assembly.", source);

            using var stream = File.OpenRead(source);
            using var reader = new PEReader(stream);
            var metadata = reader.GetMetadataReader();

            foreach (var type in metadata.TypeDefinitions.Select(metadata.GetTypeDefinition))
            {
                foreach (var func in type.GetMethods().Select(metadata.GetMethodDefinition))
                {
                    foreach (var attr in func.GetCustomAttributes().Select(metadata.GetCustomAttribute))
                    {
                        // obtain attribute information
                        var attrCtor = metadata.GetMemberReference((MemberReferenceHandle)attr.Constructor);
                        var attrType = metadata.GetTypeReference((TypeReferenceHandle)attrCtor.Parent);

                        // bail out if not a custom action attribute
                        if (attrType.Name.IsNil || metadata.GetString(attrType.Name) != "CustomActionAttribute")
                            continue;

                        // bail out if custom action attribute is not from the correct namespace
                        if (attrType.Namespace.IsNil || metadata.GetString(attrType.Namespace) != "Microsoft.Deployment.WindowsInstaller")
                            continue;

                        // assemble full name of custom action
                        var name = metadata.GetString(type.Name) + "." + metadata.GetString(func.Name);
                        if (type.Namespace.IsNil == false)
                            name = metadata.GetString(type.Namespace) + "." + name;

                        // return up new item
                        yield return new WixCustomActionItem(
                            $"CustomAction_{name}".Replace(".", "_"),
                            source,
                            metadata.GetString(func.Name),
                            "immediate",
                            "check");
                    }
                }
            }

            yield break;
        }

        /// <summary>
        /// Source assembly files.
        /// </summary>
        public ITaskItem[] Assemblies { get; set; }

        /// <summary>
        /// Output custom action items.
        /// </summary>
        public ITaskItem[] CustomActions { get; set; }

        /// <summary>
        /// Resolves the set of custom actions in the specified source assembly.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        IEnumerable<ITaskItem> ResolveCustomActions(ITaskItem source)
        {
            foreach (var i in ResolveCustomActions(source.GetMetadata("FullPath")))
            {
                // create a new task item with copied metadata
                var t = new TaskItem(i.DllEntry);
                source.CopyMetadataTo(t);

                // apply custom metadata
                t.SetMetadata("SourceFile", i.SourceFile);
                t.SetMetadata("DllEntry", i.DllEntry);
                t.SetMetadata("Execute", i.Execute);
                t.SetMetadata("Return", i.Return);

                yield return t;
            }
        }

        public override bool Execute()
        {
            CustomActions = Assemblies.SelectMany(i => ResolveCustomActions(i)).ToArray();
            return true;
        }

    }

}
