using System.IO;

namespace ChinhDo.Transactions
{
    /// <summary>
    /// Creates all directories in the specified path.
    /// </summary>
    sealed class CreateDirectoryOperation : IoOperation, IRollbackableOperation, System.IDisposable
    {
        private readonly string _path;
        private string _backupPath;
        private bool _disposed;

        /// <summary>Instantiates the class.</summary>
        /// <param name="tempPath">Path to temp directory.</param>
        /// <param name="path">The directory path to create.</param>
        public CreateDirectoryOperation(string tempPath, string path) : base(tempPath)
        {
            this._path = path;
        }

        /// <summary>
        /// Disposes the resources used by this class.
        /// </summary>
        ~CreateDirectoryOperation()
        {
            InnerDispose();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            InnerDispose();
            System.GC.SuppressFinalize(this);
        }

        public void Execute()
        {
            // find the topmost directory which must be created
            string child = Path.GetFullPath(_path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string parent = Path.GetDirectoryName(child);
            while (parent != null /* child is a root directory */
                && !Directory.Exists(parent))
            {
                child = parent;
                parent = Path.GetDirectoryName(child);
            }

            if (Directory.Exists(child))
            {
                // nothing to do
                return;
            }
            else
            {
                Directory.CreateDirectory(_path);
                _backupPath = child;
            }
        }

        public void Rollback()
        {
            if (_backupPath != null)
            {
                Directory.Delete(_backupPath, true);
            }
        }

        /// <summary>
        /// Disposes the resources of this class.
        /// </summary>
        private void InnerDispose()
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }
    }
}
