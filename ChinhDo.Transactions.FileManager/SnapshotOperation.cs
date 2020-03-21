using System.IO;

namespace ChinhDo.Transactions
{
    /// <summary>
    /// Rollbackable operation which takes a snapshot of a file. The snapshot is used to rollback the file later if needed.
    /// </summary>
    sealed class SnapshotOperation: SingleFileOperation
    {
        /// <summary>Instantiates the class.</summary>
        /// <param name="tempPath">Path to temp directory.</param>
        /// <param name="path">The file to take a snapshot for.</param>
        public SnapshotOperation(string tempPath, string path) : base(tempPath, path)
        {
        }

        public override void Execute()
        {
            if (File.Exists(path))
            {
                string temp = GetTempPathName(Path.GetExtension(path));
                File.Copy(path, temp);
                backupPath = temp;
            }
        }
    }
}
