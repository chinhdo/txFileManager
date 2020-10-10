using System.IO;
using System.Text;

namespace ChinhDo.Transactions
{
    /// <summary>
    /// Rollbackable operation which appends a string to an existing file, or creates the file if it doesn't exist.
    /// </summary>
    sealed class AppendAllTextOperation : SingleFileOperation
    {
        private readonly string contents;
        private readonly Encoding encoding;

        /// <summary>Instantiates the class.</summary>
        /// <param name="tempPath">Path to temp directory.</param>
        /// <param name="path">The file to append the string to.</param>
        /// <param name="contents">The string to append to the file.</param>
        /// <param name="encoding">The encoding to the file.</param>
        public AppendAllTextOperation(string tempPath, string path, string contents, Encoding encoding) : base(tempPath, path)
        {
            this.contents = contents;
            this.encoding = encoding;
        }

        public override void Execute()
        {
            if (File.Exists(path))
            {
                string temp = GetTempPathName(Path.GetExtension(path));
                File.Copy(path, temp);
                backupPath = temp;
            }

            if (encoding == null)
                File.AppendAllText(path, contents);
            else
                File.AppendAllText(path, contents, encoding);
        }
    }
}
