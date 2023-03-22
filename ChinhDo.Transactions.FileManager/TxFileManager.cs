using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Transactions;

namespace ChinhDo.Transactions
{
    /// <summary>
    /// Allows inclusion of file system operations in transactions (<see cref="System.Transactions"/>).
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
                EnlistOperation(new AppendAllTextOperation(GetTempPath(), path, contents, null));
            }
            else
            {
                File.AppendAllText(path, contents);
            }
        }

        public void AppendAllText(string path, string contents, Encoding encoding)
        {
            if (IsInTransaction())
            {
                EnlistOperation(new AppendAllTextOperation(GetTempPath(), path, contents, encoding));
            }
            else
            {
                File.AppendAllText(path, contents, encoding);
            }
        }

        public void Copy(string sourceFileName, string destFileName, bool overwrite)
        {
            if (IsInTransaction())
            {
                EnlistOperation(new CopyOperation(GetTempPath(), sourceFileName, destFileName, overwrite));
            }
            else
            {
                File.Copy(sourceFileName, destFileName, overwrite);
            }
        }

        public void CopyDirectory(string srcDir, string destDir)
        {
            if (IsInTransaction())
            {
                EnlistOperation(new CopyDirectoryOperation(GetTempPath(), srcDir, destDir));
            }
            else
            {
                CopyAll(new DirectoryInfo(srcDir), new DirectoryInfo(destDir));
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

            CreateDirectory(dirName);

            return dirName;
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

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

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

        public string GetTempPath()
        {
            return this._tempPath;
        }

        public void Move(string srcFileName, string destFileName)
        {
            if (IsInTransaction())
            {
                EnlistOperation(new MoveFileOperation(srcFileName, destFileName));
            }
            else
            {
                File.Move(srcFileName, destFileName);
            }
        }

        public void MoveDirectory(string srcDirName, string destDirName)
        {
            if (IsInTransaction())
            {
                EnlistOperation(new MoveDirectoryOperation(srcDirName, destDirName));
            }
            else
            {
                File.Move(srcDirName, destDirName);
            }
        }

        public void Snapshot(string fileName)
        {
            if (IsInTransaction())
            {
                EnlistOperation(new SnapshotOperation(GetTempPath(), fileName));
            }
        }

        public void WriteAllText(string path, string contents)
        {
            if (IsInTransaction())
            {
                EnlistOperation(new WriteAllTextOperation(GetTempPath(), path, contents, null));
            }
            else
            {
                File.WriteAllText(path, contents);
            }
        }

        public void WriteAllText(string path, string contents, Encoding encoding)
        {
            if (IsInTransaction())
            {
                EnlistOperation(new WriteAllTextOperation(GetTempPath(), path, contents, encoding));
            }
            else
            {
                File.WriteAllText(path, contents, encoding);
            }
        }

        public void WriteAllBytes(string path, byte[] contents)
        {
            if (IsInTransaction())
            {
                EnlistOperation(new WriteAllBytesOperation(GetTempPath(), path, contents));
            }
            else
            {
                File.WriteAllBytes(path, contents);
            }
        }

        #endregion

        #region Other Ops

        /// <summary>Get the count of _enlistments</summary>
        /// <returns></returns>
        public static int GetEnlistmentCount()
        {
            return _enlistments.Count;
        }

        #endregion

        #region Private

        /// <summary>Dictionary of transaction enlistment objects for the current thread.</summary>
        //[ThreadStatic] <-- Is this needed?
        internal static Dictionary<string, TxEnlistment> _enlistments;
        internal static readonly object _enlistmentsLock = new object();
        private string _tempPath = null;

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

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
