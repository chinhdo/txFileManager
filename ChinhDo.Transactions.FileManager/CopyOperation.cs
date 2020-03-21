using System.IO;

namespace ChinhDo.Transactions
{
    /// <summary>Rollbackable operation which copies a file.</summary>
    sealed class CopyOperation : SingleFileOperation
    {
        private readonly string sourceFileName;
        private readonly bool overwrite;

        /// <summary>Instantiates the class.</summary>
        /// <param name="sourceFileName">The file to copy.</param>
        /// <param name="tempPath">Path to temp directory.</param>
        /// <param name="destFileName">The name of the destination file.</param>
        /// <param name="overwrite">true if the destination file can be overwritten, otherwise false.</param>
        public CopyOperation(string tempPath, string sourceFileName, string destFileName, bool overwrite)
            : base(tempPath, destFileName)
        {
            this.sourceFileName = sourceFileName;
            this.overwrite = overwrite;
        }

        public override void Execute()
        {
            if (File.Exists(path))
            {
                string temp = GetTempPathName(Path.GetExtension(path));
                File.Copy(path, temp);
                backupPath = temp;
            }

            File.Copy(sourceFileName, path, overwrite);
        }
    }
}
