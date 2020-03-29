using System;
using System.Collections.Generic;
using System.IO;
using System.Transactions;

namespace ChinhDo.Transactions
{
    /// <summary>
    /// File Resource Manager. Allows inclusion of file system operations in transactions (<see cref="System.Transactions"/>).
    /// http://www.chinhdo.com/20080825/transactional-file-manager/
    /// </summary>
    public class TxFileManager : IFileManager
    {
        /// <summary>Create a new instance of <see cref="TxFileManager"/> class. Feel free to create new instances or re-use existing instances./// </summary>
        /// ///<param name="tempPath">Path to temp directory.</param>
        public TxFileManager() : this(Path.GetTempPath())
        {

        }

        /// <summary>Create a new instance of <see cref="TxFileManager"/> class. Feel free to create new instances or re-use existing instances./// </summary>
        ///<param name="tempPath">Path to temp directory.</param>
        public TxFileManager(string tempPath)
        {
            this._tempPath = Path.Combine(tempPath, "TxFileMgr-fc4eed76ee9b");
            Directory.CreateDirectory(_tempPath); // This will create folder if neccessary
        }

        #region IFileOperations

        public void AppendAllText(string path, string contents)
        {
            if (IsInTransaction())
            {
                EnlistOperation(new AppendAllTextOperation(this.GetTempPath(), path, contents));
            }
            else
            {
                File.AppendAllText(path, contents);
            }
        }

        public void Copy(string sourceFileName, string destFileName, bool overwrite)
        {
            if (IsInTransaction())
            {
                EnlistOperation(new CopyOperation(this.GetTempPath(), sourceFileName, destFileName, overwrite));
            }
            else
            {
                File.Copy(sourceFileName, destFileName, overwrite);
            }
        }

        public void CreateDirectory(string path)
        {
            if (IsInTransaction())
            {
                EnlistOperation(new CreateDirectoryOperation(GetTempPath(), path));
            }
            else
            {
                Directory.CreateDirectory(path);
            }
        }

        public void Delete(string path)
        {
            if (IsInTransaction())
            {
                EnlistOperation(new DeleteFileOperation(this.GetTempPath(), path));
            }
            else
            {
                File.Delete(path);
            }
        }

        public void DeleteDirectory(string path)
        {
            if (IsInTransaction())
            {
                EnlistOperation(new DeleteDirectoryOperation(GetTempPath(), path));
            }
            else
            {
                Directory.Delete(path, true);
            }
        }

        public void Move(string srcFileName, string destFileName)
        {
            if (IsInTransaction())
            {
                EnlistOperation(new MoveFileOperation(this.GetTempPath(), srcFileName, destFileName));
            }
            else
            {
                File.Move(srcFileName, destFileName);
            }
        }

        public void Snapshot(string fileName)
        {
            if (IsInTransaction())
            {
                EnlistOperation(new SnapshotOperation(this.GetTempPath(), fileName));
            }
        }

        public void WriteAllText(string path, string contents)
        {
            if (IsInTransaction())
            {
                EnlistOperation(new WriteAllTextOperation(this.GetTempPath(), path, contents));
            }
            else
            {
                File.WriteAllText(path, contents);
            }
        }

        public void WriteAllBytes(string path, byte[] contents)
        {
            if (IsInTransaction())
            {
                EnlistOperation(new WriteAllBytesOperation(this.GetTempPath(), path, contents));
            }
            else
            {
                File.WriteAllBytes(path, contents);
            }
        }

        #endregion

        /// <summary>Determines whether the specified path refers to a directory that exists on disk.</summary>
        /// <param name="path">The directory to check.</param>
        /// <returns>True if the directory exists.</returns>
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>Determines whether the specified file exists.</summary>
        /// <param name="path">The file to check.</param>
        /// <returns>True if the file exists.</returns>
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>Gets the files in the specified directory.</summary>
        /// <param name="path">The directory to get files.</param>
        /// <param name="handler">The <see cref="FileEventHandler"/> object to call on each file found.</param>
        /// <param name="recursive">if set to <c>true</c>, include files in sub directories recursively.</param>
        public void GetFiles(string path, FileEventHandler handler, bool recursive)
        {
            foreach (string fileName in Directory.GetFiles(path))
            {
                bool cancel = false;
                handler(fileName, ref cancel);
                if (cancel)
                {
                    return;
                }
            }

            // Check subdirs
            if (recursive)
            {
                foreach (string folderName in Directory.GetDirectories(path))
                {
                    GetFiles(folderName, handler, recursive);
                }
            }
        }

        public string CreateTempFileName(string extension)
        {
            Guid g = Guid.NewGuid();
            string tempFolder = GetTempPath();
            string ret = Path.Combine(tempFolder, g.ToString().Substring(0, 16)) + extension;
            Snapshot(ret);
            return ret;
        }

        public string CreateTempFileName()
        {
            return CreateTempFileName(".tmp");
        }

        public string CreateTempDirectory()
        {
            return CreateTempDirectory(GetTempPath(), string.Empty);
        }

        public string CreateTempDirectory(string parentDirectory, string prefix)
        {
            Guid g = Guid.NewGuid();
            string dirName = Path.Combine(parentDirectory, prefix + g.ToString().Substring(0, 16));

            // TODO SnapShot Directory
            CreateDirectory(dirName);

            return dirName;
        }

        /// <summary>
        /// Gets the folder where we should store temporary files and folders. Override this if you want your own impl.
        /// </summary>
        /// <returns></returns>
        public string GetTempPath()
        {
            return this._tempPath;
        }

        /// <summary>Get the count of _enlistments</summary>
        /// <returns></returns>
        public static int GetEnlistmentCount()
        {
            return _enlistments.Count;
        }

        #region Private

        /// <summary>Dictionary of transaction enlistment objects for the current thread.</summary>
        [ThreadStatic]
        internal static Dictionary<string, TxEnlistment> _enlistments;
        internal static readonly object _enlistmentsLock = new object();
        private string _tempPath = null;

        private static bool IsInTransaction()
        {
            return Transaction.Current != null;
        }

        private static void EnlistOperation(IRollbackableOperation operation)
        {
            Transaction tx = Transaction.Current;
            TxEnlistment enlistment;

            lock (_enlistmentsLock)
            {
                if (_enlistments == null)
                {
                    _enlistments = new Dictionary<string, TxEnlistment>();
                }

                if (!_enlistments.TryGetValue(tx.TransactionInformation.LocalIdentifier, out enlistment))
                {
                    enlistment = new TxEnlistment(tx);
                    _enlistments.Add(tx.TransactionInformation.LocalIdentifier, enlistment);
                }

                enlistment.EnlistOperation(operation);
            }
        }

        #endregion
    }
}
