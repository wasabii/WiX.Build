namespace WiX.Build.Sdk.Tasks
{

    /// <summary>
    /// Describes a discovered custom action.
    /// </summary>
    public class WixCustomActionItem
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public WixCustomActionItem()
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sourceFile"></param>
        /// <param name="dllEntry"></param>
        /// <param name="execute"></param>
        /// <param name="return"></param>
        public WixCustomActionItem(string id, string sourceFile, string dllEntry, string execute, string @return)
        {
            Id = id;
            SourceFile = sourceFile;
            DllEntry = dllEntry;
            Execute = execute;
            Return = @return;
        }

        public string Id { get; set; }

        public string SourceFile { get; set; }

        public string DllEntry { get; set; }

        public string Execute { get; set; }

        public string Return { get; set; }

    }

}
