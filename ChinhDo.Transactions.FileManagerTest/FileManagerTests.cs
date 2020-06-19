using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Transactions;
using Xunit;

namespace ChinhDo.Transactions.FileManagerTest
{
    public sealed class FileManagerTests : IDisposable
    {
        private readonly IFileManager _target;
        private IList<string> _tempPaths;

        public FileManagerTests()
        {
            _target = new TxFileManager();
            _tempPaths = new List<string>();
            // TODO delete any temp files
        }

        public void Dispose()
        {
            // Delete temp files/dirs
            foreach (string item in _tempPaths)
            {
                if (File.Exists(item))
                {
                    File.Delete(item);
                }

                if (Directory.Exists(item))
                {
                    Directory.Delete(item);
                }
            }

            int numTempFiles = Directory.GetFiles(Path.Combine(Path.GetTempPath(), "TxFileMgr-fc4eed76ee9b")).Length;
            Assert.Equal(0, numTempFiles);
        }

        #region Operations

        [Fact]
        public void CanAppendAllText()
        {
            string f1 = GetTempPathName();
            const string contents = "123";

            using (TransactionScope scope1 = new TransactionScope())
            {
                _target.AppendAllText(f1, contents);
                scope1.Complete();
            }

            Assert.Equal(contents, File.ReadAllText(f1));
        }

        [Fact]
        public void AppendAllTexHandlesException()
        {
            string f1 = GetTempPathName();
            const string contents = "123";

            using (TransactionScope scope1 = new TransactionScope())
            {
                using (FileStream fs = File.Open(f1, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None))
                {
                    Exception ex = Assert.Throws<IOException>(() => _target.AppendAllText(f1, contents));
                    Assert.Contains("The process cannot access the file", ex.Message, StringComparison.CurrentCulture);
                }
            }
        }

        [Fact]
        public void AppendAllTextCanRollback()
        {
            string f1 = GetTempPathName();
            const string contents = "qwerty";
            using (TransactionScope sc1 = new TransactionScope())
            {
                _target.AppendAllText(f1, contents);
                // without specifically committing, this should rollback
            }

            Assert.False(File.Exists(f1), f1 + " should not exist.");
        }

        [Fact]
        public void CanCopy()
        {
            string sourceFileName = GetTempPathName();
            string destFileName = GetTempPathName();

            const string expectedText = "Test 123.";
            using (TransactionScope scope1 = new TransactionScope())
            {
                File.WriteAllText(sourceFileName, expectedText);
                _target.Copy(sourceFileName, destFileName, false);
                scope1.Complete();
            }

            Assert.Equal(expectedText, File.ReadAllText(sourceFileName));
            Assert.Equal(expectedText, File.ReadAllText(destFileName));
        }

        [Fact]
        public void CanCopyAndRollback()
        {
            string sourceFileName = GetTempPathName();
            const string expectedText = "Hello 123.";
            File.WriteAllText(sourceFileName, expectedText);
            string destFileName = GetTempPathName();

            using (TransactionScope scope1 = new TransactionScope())
            {
                _target.Copy(sourceFileName, destFileName, false);
                // without specifically committing, this should rollback
            }

            Assert.False(File.Exists(destFileName), destFileName + " should not exist.");
        }

        [Fact]
        public void CanHandleCopyErrors()
        {
            string f1 = GetTempPathName();
            string f2 = GetTempPathName();

            using (var fs = new FileStream(f2, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                using (TransactionScope scope1 = new TransactionScope())
                {
                    _target.WriteAllText(f1, "test");
                    Assert.Throws<IOException>(() => _target.Copy(f1, f2, false));

                    //rollback
                }
            }

            Assert.False(File.Exists(f1));
        }

        [Fact]
        public void CanCreateDirectory()
        {
            string d1 = GetTempPathName();
            using (TransactionScope scope1 = new TransactionScope())
            {
                _target.CreateDirectory(d1);
                scope1.Complete();
            }
            Assert.True(Directory.Exists(d1), d1 + " should exist.");
        }

        /// <summary>
        /// Validate that we are able to create nested directotories and roll them back.
        /// </summary>
        [Fact]
        public void CanRollbackNestedDirectories()
        {
            string baseDir = GetTempPathName();
            Directory.CreateDirectory(baseDir);
            string nested = Path.Combine(baseDir, "level1");
            nested = Path.Combine(nested, "level2");
            using (new TransactionScope())
            {
                _target.CreateDirectory(nested);
                Assert.True(Directory.Exists(nested));
            }
            Assert.False(Directory.Exists(nested), nested + " should not exists.");
            Assert.True(Directory.Exists(baseDir), baseDir + " should exist.");
            Directory.Delete(baseDir);
        }

        [Fact]
        public void CanCreateDirectoryAndRollback()
        {
            string d1 = GetTempPathName();
            using (TransactionScope scope1 = new TransactionScope())
            {
                _target.CreateDirectory(d1);
            }
            Assert.False(Directory.Exists(d1), d1 + " should not exist.");
        }

        [Fact]
        public void CanDeleteDirectory()
        {
            string f1 = GetTempPathName();
            Directory.CreateDirectory(f1);

            using (TransactionScope scope1 = new TransactionScope())
            {
                _target.DeleteDirectory(f1);
                scope1.Complete();
            }

            Assert.False(Directory.Exists(f1), f1 + " should no longer exist.");
        }

        [Fact]
        public void CanDeleteDirectoryAndRollback()
        {
            string f1 = GetTempPathName();
            Directory.CreateDirectory(f1);

            using (TransactionScope scope1 = new TransactionScope())
            {
                _target.DeleteDirectory(f1);
            }

            Assert.True(Directory.Exists(f1), f1 + " should exist.");
        }

        [Fact]
        public void CanDeleteFile()
        {
            string f1 = GetTempPathName();
            const string contents = "abc";
            File.WriteAllText(f1, contents);

            using (TransactionScope scope1 = new TransactionScope())
            {
                _target.Delete(f1);
                scope1.Complete();
            }

            Assert.False(File.Exists(f1), f1 + " should no longer exist.");
        }

        [Fact]
        public void CanDeleteFileAndRollback()
        {
            string f1 = GetTempPathName();
            const string contents = "abc";
            File.WriteAllText(f1, contents);

            using (TransactionScope scope1 = new TransactionScope())
            {
                _target.Delete(f1);
            }

            Assert.True(File.Exists(f1), f1 + " should exist.");
            Assert.Equal(contents, File.ReadAllText(f1));
        }

        [Fact]
        public void CanMoveFile()
        {
            const string contents = "abc";
            string f1 = GetTempPathName();
            string f2 = GetTempPathName();
            File.WriteAllText(f1, contents);

            using (TransactionScope scope1 = new TransactionScope())
            {
                Assert.True(File.Exists(f1));
                Assert.False(File.Exists(f2));
                _target.Move(f1, f2);
                scope1.Complete();
            }
        }

        [Fact]
        public void CanMoveFileAndRollback()
        {
            const string contents = "abc";
            string f1 = GetTempPathName();
            string f2 = GetTempPathName();
            File.WriteAllText(f1, contents);

            using (new TransactionScope())
            {
	            Assert.True(File.Exists(f1));
	            Assert.False(File.Exists(f2));
	            _target.Move(f1, f2);
            }

            Assert.Equal(contents, File.ReadAllText(f1));
            Assert.False(File.Exists(f2));
        }

        [Fact]
        public void CanMoveDirectory() {
	        string f1 = GetTempPathName();
	        string f2 = GetTempPathName();
	        Directory.CreateDirectory(f1);

	        using (TransactionScope scope1 = new TransactionScope()) {
		        Assert.True(Directory.Exists(f1));
		        Assert.False(Directory.Exists(f2));
		        _target.MoveDirectory(f1, f2);
		        scope1.Complete();
	        }

	        Assert.False(Directory.Exists(f1));
	        Assert.True(Directory.Exists(f2));
        }

        [Fact]
        public void CanMoveDirectoryAndRollback() {
	        string f1 = GetTempPathName();
	        string f2 = GetTempPathName();
	        Directory.CreateDirectory(f1);

	        using (new TransactionScope()) {
		        Assert.True(Directory.Exists(f1));
		        Assert.False(Directory.Exists(f2));
		        _target.MoveDirectory(f1, f2);
	        }

	        Assert.True(Directory.Exists(f1));
	        Assert.False(Directory.Exists(f2));
        }

        [Fact]
        public void CanSnapshot()
        {
            string f1 = GetTempPathName();

            using (TransactionScope scope1 = new TransactionScope())
            {
                _target.Snapshot(f1);
                _target.AppendAllText(f1, "<test></test>");
            }

            Assert.False(File.Exists(f1));
        }

        [Fact]
        public void CanWriteAllText()
        {
            string f1 = GetTempPathName();
            const string contents = "abcdef";
            File.WriteAllText(f1, "123");

            using (TransactionScope scope1 = new TransactionScope())
            {
                _target.WriteAllText(f1, contents);
                scope1.Complete();
            }

            Assert.Equal(contents, File.ReadAllText(f1));
        }

        [Fact]
        public void CanWriteAllTextAndRollback()
        {
            string f1 = GetTempPathName();
            const string contents1 = "123";
            const string contents2 = "abcdef";
            File.WriteAllText(f1, contents1);

            using (TransactionScope scope1 = new TransactionScope())
            {
                _target.WriteAllText(f1, contents2);
            }

            Assert.Equal(contents1, File.ReadAllText(f1));
        }

        #endregion

        #region Transaction Support

        [Fact]
        public void ThrowExceptionIfCannotRollback()
        {
            // Run this test on Windows only
            // This test doesn't work on Ubuntu/Unix because setting file attribute to read-only does not 
            // prevent this code from deleting the file

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string f1 = GetTempPathName(".txt");
                string f2 = GetTempPathName(".txt");

                try
                {
                    Exception ex = Assert.Throws<TransactionException>(() =>
                    {
                        using (TransactionScope scope1 = new TransactionScope())
                        {
                            _target.WriteAllText(f1, "Test.");
                            _target.WriteAllText(f2, "Test.");

                            FileInfo fi1 = new FileInfo(f1);
                            fi1.Attributes = FileAttributes.ReadOnly;

                            // rollback
                        }
                    });

                    Assert.Contains("Failed to roll back.", ex.Message, StringComparison.CurrentCulture);
                }
                finally
                {
                    FileInfo fi1 = new FileInfo(f1);
                    if (fi1.Exists)
                    {
                        fi1.Attributes = FileAttributes.Normal;
                        File.Delete(f1);
                    }
                }
            }
        }

        [Fact]
        public void CanReuseManager()
        {
            {
                string f1 = GetTempPathName();
                File.WriteAllText(f1, "Hello.");
                string f2 = GetTempPathName();

                using (TransactionScope scope1 = new TransactionScope())
                {
                    _target.Copy(f1, f2, false);

                    // rollback
                }

                Assert.False(File.Exists(f2));
            }

            {
                string f1 = GetTempPathName();
                File.WriteAllText(f1, "Hello.");

                using (TransactionScope scope1 = new TransactionScope())
                {
                    _target.Delete(f1);

                    // rollback
                }

                Assert.True(File.Exists(f1));
            }
        }

        [Fact]
        public void CanSupportTransactionScopeOptionSuppress()
        {
            const string contents = "abc";
            string f1 = GetTempPathName(".txt");
            using (TransactionScope scope1 = new TransactionScope(TransactionScopeOption.Suppress))
            {
                _target.WriteAllText(f1, contents);
            }

            // With TransactionScopeOption.Suppress - commit is implicit so our change should have been committed
            // without us doing a scope.Complete()
            Assert.Equal(contents, File.ReadAllText(f1));
        }

        [Fact]
        public void CanNestTransactions()
        {
            string f1 = GetTempPathName(".txt");
            const string f1Contents = "f1";
            string f2 = GetTempPathName(".txt");
            const string f2Contents = "f2";
            string f3 = GetTempPathName(".txt");
            const string f3Contents = "f3";

            using (TransactionScope sc1 = new TransactionScope())
            {
                _target.WriteAllText(f1, f1Contents);

                using (TransactionScope sc2 = new TransactionScope())
                {
                    _target.WriteAllText(f2, f2Contents);
                    sc2.Complete();
                }

                using (TransactionScope sc3 = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    _target.WriteAllText(f3, f3Contents);
                    sc3.Complete();
                }

                sc1.Dispose();
            }

            Assert.False(File.Exists(f1));
            Assert.False(File.Exists(f2));
            Assert.True(File.Exists(f3));
        }

        [Fact]
        public void CanDoMultiThreads()
        {
            // Start each test in its own thread and repeat for a few interations
            const int iterations = 25;
            IList<Thread> threads = new List<Thread>();
            BlockingCollection<Exception> exceptions = new BlockingCollection<Exception>();

            Action[] actions = new Action[] { CanAppendAllText, AppendAllTextCanRollback, CanCopy, CanCopyAndRollback,
                CanCreateDirectory, CanCreateDirectoryAndRollback, CanDeleteFile, CanDeleteFileAndRollback, CanMoveFile,
                CanMoveFileAndRollback, CanSnapshot, CanWriteAllText, CanWriteAllTextAndRollback, CanNestTransactions,
                ThrowException
            };
            for (int i = 0; i < iterations; i++)
            {
                foreach (Action action in actions)
                {
                    Thread t = new Thread(() => { Launch(action, exceptions); });
                    threads.Add(t);
                }
            }

            foreach (Thread t in threads)
            {
                t.Start();
                t.Join();
            }

            Assert.Equal(iterations, exceptions.Count);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private static void Launch(Action action, BlockingCollection<Exception> exceptions)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        #endregion

        #region Other

        [Fact]
        public void ItRemovesCompletedEnlistments()
        {
            string f1 = GetTempPathName();
            const string contents = "123";

            using (TransactionScope scope1 = new TransactionScope())
            {
                _target.AppendAllText(f1, contents);
                scope1.Complete();
            }

            Assert.Equal(0, TxFileManager.GetEnlistmentCount());
        }

        [Fact]
        public void CanSetCustomTempPath()
        {
            IFileManager fm = new TxFileManager();
            string myTempPath = "\\temp-f8417ba5";

            string d1 = fm.CreateTempDirectory();
            Assert.DoesNotContain(myTempPath, d1, StringComparison.CurrentCulture);

            string f1 = fm.CreateTempFileName();
            Assert.DoesNotContain(myTempPath, f1, StringComparison.CurrentCulture);

            IFileManager fm2 = new TxFileManager(myTempPath);
            string d2 = fm2.CreateTempDirectory();
            Assert.Contains(myTempPath, d2, StringComparison.CurrentCulture);

            string f2 = fm2.CreateTempFileName();
            Assert.Contains(myTempPath, f2, StringComparison.CurrentCulture);

            Directory.Delete(d1);
            Directory.Delete(d2);

            File.Delete(f1);
            File.Delete(f2);
        }

        #endregion

        #region Private

        private string GetTempPathName(string extension = "")
        {
            string tempFile = _target.CreateTempFileName(extension);
            _tempPaths.Add(tempFile);
            return tempFile;
        }

        private void ThrowException()
        {
            throw new Exception("Test.");
        }

        #endregion

    }
}
