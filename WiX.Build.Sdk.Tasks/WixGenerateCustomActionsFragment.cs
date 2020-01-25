using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace WiX.Build.Sdk.Tasks
{

    /// <summary>
    /// Generates a WiX Fragment containing Binary and CustomAction declarations for the specified custom action items.
    /// </summary>
    public class WixGenerateCustomActionsFragment : Task
    {

        static readonly XNamespace wix = "http://schemas.microsoft.com/wix/2006/wi";

        /// <summary>
        /// Creates a MD5 hash for the specified byte array.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        static byte[] GetHash(byte[] d)
        {
            using (var md5 = MD5.Create())
                return md5.ComputeHash(d);
        }

        /// <summary>
        /// Gets a hash for a string value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static string GetHashForString(string value)
        {
            return BitConverter.ToString(GetHash(Encoding.UTF8.GetBytes(value))).Replace("-", "");
        }

        /// <summary>
        /// Generates the XML for a set of custom actions.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        internal static XDocument Generate(IEnumerable<WixCustomActionItem> items)
        {
            if (items is null)
                throw new ArgumentNullException(nameof(items));

            return new XDocument(
                new XElement(wix + "Wix",
                    new XElement(wix + "Fragment",
                        items
                            .GroupBy(i => i.SourceFile)
                            .Select(i =>
                                new XElement(wix + "Binary",
                                    new XAttribute("Id", "_" + GetHashForString(i.Key)),
                                    new XAttribute("SourceFile", i.Key))),
                        items
                            .Select(i =>
                                new XElement(wix + "CustomAction",
                                    new XAttribute("Id", i.DllEntry),
                                    new XAttribute("BinaryKey", "_" + GetHashForString(i.SourceFile)),
                                    new XAttribute("DllEntry", i.DllEntry),
                                    new XAttribute("Execute", i.Execute),
                                    new XAttribute("Return", i.Return))))));
        }

        /// <summary>
        /// Gets or sets the set of custom action items.
        /// </summary>
        public ITaskItem[] CustomActions { get; set; }

        /// <summary>
        /// Gets or sets the final output path of the generated fragment.
        /// </summary>
        public string OutputPath { get; set; }

        public override bool Execute()
        {
            // build XML
            var xml = Generate(CustomActions.Select(i => new WixCustomActionItem()
            {
                SourceFile = i.GetMetadata("FullPath"),
                DllEntry = i.GetMetadata("DllEntry"),
                Execute = i.GetMetadata("Execute"),
                Return = i.GetMetadata("Return"),
            }));

            // save to output path
            xml.Save(OutputPath);
            return true;
        }

    }

}
