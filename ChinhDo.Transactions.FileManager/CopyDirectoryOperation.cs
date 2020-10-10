using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChinhDo.Transactions
{
    /// <summary>
    ///  Copy a directory
    /// </summary>
    sealed class CopyDirectoryOperation : IoOperation, IRollbackableOperation, IDisposable
    {
        private readonly string _srcDir;
        private readonly string _destDir;
        private bool _disposed;

        public CopyDirectoryOperation(string tempPath, string srcDir, string destDir): base(tempPath)
        {
            _srcDir = srcDir;
            _destDir = destDir;
        }

        ~CopyDirectoryOperation()
        {
            InnerDispose();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            InnerDispose();
            GC.SuppressFinalize(this);
        }

        public void Execute()
        {
            if (Directory.Exists(_destDir))
            {
                throw new Exception(string.Format("Destination folder {0} already exists.", _destDir));
            }

            Directory.CreateDirectory(_destDir);

            foreach (string path in Directory.GetDirectories(_srcDir, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(path.Replace(_srcDir, _destDir));
            }

            foreach (string newPath in Directory.GetFiles(_srcDir, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(_srcDir, _destDir), true);
            }
        }

        public void Rollback()
        {
            Directory.Delete(_destDir, true);
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
