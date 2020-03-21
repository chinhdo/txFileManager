using System.IO;

namespace ChinhDo.Transactions
{
    /// <summary>
    /// Rollbackable operation which deletes a file. An exception is not thrown if the file does not exist.
    /// </summary>
    sealed class DeleteFileOperation : SingleFileOperation
    {
        /// <summary>Instantiates the class.</summary>
        /// <param name="tempPath">Path to temp directory.</param>
        /// <param name="path">The file to be deleted.</param>
        public DeleteFileOperation(string tempPath, string path) : base(tempPath, path)
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

            File.Delete(path);
        }
    }
}
