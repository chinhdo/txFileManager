using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Transactions;
using System.Xml;
using NUnit.Framework;

namespace ChinhDo.Transactions.FileManagerTest
{
    [TestFixture]
    class FileManagerTest
    {
        private int _numTempFiles;
        private IFileManager _target;

        [SetUp]
        public void TestInitialize()
        {
            _target = new TxFileManager();
            _numTempFiles = Directory.GetFiles(Path.Combine(Path.GetTempPath(), "CdFileMgr")).Length;
        }

        [TearDown]
        public void TestCleanup()
        {
            int numTempFiles = Directory.GetFiles(Path.Combine(Path.GetTempPath(), "CdFileMgr")).Length;
            Assert.AreEqual(_numTempFiles, numTempFiles, "Unexpected value for numTempFiles.");
        }

        #region Operations
        [Test]
        public void CanAppendText()
        {
            string f1 = _target.GetTempFileName();
            const string contents = "123";

            try
            {
                using (TransactionScope scope1 = new TransactionScope())
                {
                    _target.AppendAllText(f1, contents);
                    scope1.Complete();
                }
                Assert.AreEqual(contents, File.ReadAllText(f1), "Incorrect value for ReadAllText.");
            }
            finally
            {
                File.Delete(f1);
            }
        }

        [Test, ExpectedException(typeof(IOException))]
        public void CannotAppendText()
        {
            string f1 = _target.GetTempFileName();
            const string contents = "123";

            try
            {
                using (TransactionScope scope1 = new TransactionScope())
                {
                    using (FileStream fs = File.Open(f1, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None))
                    {
                        _target.AppendAllText(f1, contents);
                    }
                }
            }
            finally
            {
                File.Delete(f1);
            }
        }

        [Test]
        public void CanAppendTextAndRollback()
        {
            string f1 = _target.GetTempFileName();
            const string contents = "qwerty";
            using (TransactionScope sc1 = new TransactionScope())
            {
                _target.AppendAllText(f1, contents);
            }

            Assert.IsFalse(File.Exists(f1), f1 + " should not exist.");
        }

        [Test]
        public void CanCopy()
        {
            string sourceFileName = _target.GetTempFileName();
            string destFileName = _target.GetTempFileName();

            try
            {
                const string expectedText = "Test 123.";
                using (TransactionScope scope1 = new TransactionScope())
                {
                    File.WriteAllText(sourceFileName, expectedText);
                    _target.Copy(sourceFileName, destFileName, false);
                    scope1.Complete();
                }

                Assert.AreEqual(expectedText, File.ReadAllText(sourceFileName), sourceFileName + " doesn't contain expected text.");
                Assert.AreEqual(expectedText, File.ReadAllText(destFileName), destFileName + " doesn't contain expected text.");
            }
            finally
            {
                File.Delete(sourceFileName);
                File.Delete(destFileName);
            }
        }

        [Test]
        public void CanCopyAndRollback()
        {
            string sourceFileName = _target.GetTempFileName();
            const string expectedText = "Hello 123.";
            File.WriteAllText(sourceFileName, expectedText);
            string destFileName = _target.GetTempFileName();

            try
            {
                using (TransactionScope scope1 = new TransactionScope())
                {
                    _target.Copy(sourceFileName, destFileName, false);
                    // rollback
                }

                Assert.IsFalse(File.Exists(destFileName), destFileName + " should not exist.");
            }
            finally
            {
                File.Delete(sourceFileName);
                File.Delete(destFileName);
            }
        }

        [Test]
        public void CanCreateDirectory()
        {
            string d1 = _target.GetTempFileName();
            try
            {
                using (TransactionScope scope1 = new TransactionScope())
                {
                    _target.CreateDirectory(d1);
                    scope1.Complete();
                }
                Assert.IsTrue(Directory.Exists(d1), d1 + " should exist.");
            }
            finally
            {
                Directory.Delete(d1);
            }
        }

        /// <summary>
        /// Validate that we are able to create nested directotories and roll them back.
        /// </summary>
        [Test]
        public void CanRollbackNestedDirectories()
        {
            string baseDir = _target.GetTempFileName(string.Empty);
            string nested1 = Path.Combine(baseDir, "level1");
            using (new TransactionScope())
            {
                _target.CreateDirectory(nested1);
            }
            Assert.IsFalse(Directory.Exists(baseDir), baseDir + " should not exist.");
        }

        [Test]
        public void CanCreateDirectoryAndRollback()
        {
            string d1 = _target.GetTempFileName();
            using (TransactionScope scope1 = new TransactionScope())
            {
                _target.CreateDirectory(d1);
            }
            Assert.IsFalse(Directory.Exists(d1), d1 + " should not exist.");
        }

        [Test]
        public void CanDeleteFile()
        {
            string f1 = _target.GetTempFileName();
            try
            {
                const string contents = "abc";
                File.WriteAllText(f1, contents);

                using (TransactionScope scope1 = new TransactionScope())
                {
                    _target.Delete(f1);
                    scope1.Complete();
                }

                Assert.IsFalse(File.Exists(f1), f1 + " should no longer exist.");
            }
            finally
            {
                File.Delete(f1);
            }
        }

        [Test]
        public void CanDeleteFileAndRollback()
        {
            string f1 = _target.GetTempFileName();
            try
            {
                const string contents = "abc";
                File.WriteAllText(f1, contents);

                using (TransactionScope scope1 = new TransactionScope())
                {
                    _target.Delete(f1);
                }

                Assert.IsTrue(File.Exists(f1), f1 + " should exist.");
                Assert.AreEqual(contents, File.ReadAllText(f1), "Unexpected value from ReadAllText.");
            }
            finally
            {
                File.Delete(f1);
            }
        }

        [Test]
        public void CanMoveFile()
        {
            const string contents = "abc";
            string f1 = _target.GetTempFileName();
            string f2 = _target.GetTempFileName();
            try
            {
                File.WriteAllText(f1, contents);

                using (TransactionScope scope1 = new TransactionScope())
                {
                    Assert.IsTrue(File.Exists(f1), "{0} should exist.", f1);
                    Assert.IsFalse(File.Exists(f2), "{0} should not exist", f2);
                    _target.Move(f1, f2);
                    scope1.Complete();
                }
            }
            finally
            {
                File.Delete(f1);
                File.Delete(f2);
            }
        }

        [Test]
        public void CanMoveFileAndRollback()
        {
            const string contents = "abc";
            string f1 = _target.GetTempFileName();
            string f2 = _target.GetTempFileName();
            try
            {
                File.WriteAllText(f1, contents);

                using (TransactionScope scope1 = new TransactionScope())
                {
                    Assert.IsTrue(File.Exists(f1), "{0} should exist.", f1);
                    Assert.IsFalse(File.Exists(f2), "{0} should not exist", f2);
                    _target.Move(f1, f2);
                }

                Assert.AreEqual(contents, File.ReadAllText(f1), "ReadAllText returns unexpected value.");
                Assert.IsFalse(File.Exists(f2), "{0} should not exist.", f2);
            }
            finally
            {
                File.Delete(f1);
                File.Delete(f2);
            }
        }

        [Test]
        public void CanSnapshot()
        {
            string f1 = _target.GetTempFileName();

            using (TransactionScope scope1 = new TransactionScope())
            {
                _target.Snapshot(f1);

                _target.AppendAllText(f1, "<test></test>");
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(f1);
            }

            Assert.IsFalse(File.Exists(f1), f1 + " should not exist.");
        }

        [Test]
        public void CanWriteAllText()
        {
            string f1 = _target.GetTempFileName();
            try
            {
                const string contents = "abcdef";
                File.WriteAllText(f1, "123");

                using (TransactionScope scope1 = new TransactionScope())
                {
                    _target.WriteAllText(f1, contents);
                    scope1.Complete();
                }

                Assert.AreEqual(contents, File.ReadAllText(f1), "Unexpected value from ReadAllText.");
            }
            finally
            {
                File.Delete(f1);
            }
        }

        [Test]
        public void CanWriteAllTextAndRollback()
        {
            string f1 = _target.GetTempFileName();
            try
            {
                const string contents1 = "123";
                const string contents2 = "abcdef";
                File.WriteAllText(f1, contents1);

                using (TransactionScope scope1 = new TransactionScope())
                {
                    _target.WriteAllText(f1, contents2);
                }

                Assert.AreEqual(contents1, File.ReadAllText(f1), "Unexpected value from ReadAllText.");
            }
            finally
            {
                File.Delete(f1);
            }
        }

        #endregion

        #region Error Handling

        [Test]
        public void CanHandleCopyErrors()
        {
            string f1 = _target.GetTempFileName();
            string f2 = _target.GetTempFileName();

            var fs = new FileStream(f2, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);

            try
            {
                const string expectedText = "Test 123.";
                using (TransactionScope scope1 = new TransactionScope())
                {
                    File.WriteAllText(f1, expectedText);

                    try
                    {
                        _target.Copy(f1, f2, false);
                    }
                    catch (System.IO.IOException)
                    {
                        // Ignore IOException
                    }

                    //rollback
                }

            }
            finally
            {
                File.Delete(f1);
                fs.Close();
                File.Delete(f2);
            }
        }

        #endregion

        #region Transaction Support

        [Test, ExpectedException(typeof(TransactionException))]
        public void CannotRollback()
        {
            string f1 = _target.GetTempFileName(".txt");
            string f2 = _target.GetTempFileName(".txt");

            try
            {
                using (TransactionScope scope1 = new TransactionScope())
                {
                    _target.WriteAllText(f1, "Test.");
                    _target.WriteAllText(f2, "Test.");

                    FileInfo fi1 = new FileInfo(f1);
                    fi1.Attributes = FileAttributes.ReadOnly;

                    // rollback
                }
            }
            finally
            {
                FileInfo fi1 = new FileInfo(f1);
                fi1.Attributes = FileAttributes.Normal;
                File.Delete(f1);
            }
        }

        [Test]
        public void CanReuseManager()
        {
            {
                string sourceFileName = _target.GetTempFileName();
                File.WriteAllText(sourceFileName, "Hello.");
                string destFileName = _target.GetTempFileName();

                try
                {
                    using (TransactionScope scope1 = new TransactionScope())
                    {
                        _target.Copy(sourceFileName, destFileName, false);

                        // rollback
                    }

                    Assert.IsFalse(File.Exists(destFileName), destFileName + " should not exist.");
                }
                finally
                {
                    File.Delete(sourceFileName);
                    File.Delete(destFileName);
                }
            }

            {
                string sourceFileName = _target.GetTempFileName();
                File.WriteAllText(sourceFileName, "Hello.");
                string destFileName = _target.GetTempFileName();

                try
                {
                    using (TransactionScope scope1 = new TransactionScope())
                    {
                        _target.Copy(sourceFileName, destFileName, false);

                        // rollback
                    }

                    Assert.IsFalse(File.Exists(destFileName), destFileName + " should not exist.");
                }
                finally
                {
                    File.Delete(sourceFileName);
                    File.Delete(destFileName);
                }
            }
        }

        [Test]
        public void CanSupportTransactionScopeOptionSuppress()
        {
            const string contents = "abc";
            string f1 = _target.GetTempFileName(".txt");
            try
            {
                using (TransactionScope scope1 = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    _target.WriteAllText(f1, contents);
                }

                Assert.AreEqual(contents, File.ReadAllText(f1), "ReadAllText returns incorrect value.");
            }
            finally
            {
                File.Delete(f1);
            }
        }

        [Test]
        public void CanDoMultiThread()
        {
            const int numThreads = 10;
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < numThreads; i++)
            {
                threads.Add(new Thread(CanAppendText));
                threads.Add(new Thread(CanAppendTextAndRollback));
                threads.Add(new Thread(CanCopy));
                threads.Add(new Thread(CanCopyAndRollback));
                threads.Add(new Thread(CanCreateDirectory));
                threads.Add(new Thread(CanCreateDirectoryAndRollback));
                threads.Add(new Thread(CanDeleteFile));
                threads.Add(new Thread(CanDeleteFileAndRollback));
                threads.Add(new Thread(CanMoveFile));
                threads.Add(new Thread(CanMoveFileAndRollback));
                threads.Add(new Thread(CanSnapshot));
                threads.Add(new Thread(CanWriteAllText));
                threads.Add(new Thread(CanWriteAllTextAndRollback));
            }

            foreach (Thread t in threads)
            {
                t.Start();
                t.Join();
            }
        }

        [Test]
        public void CanNestTransactions()
        {
            string f1 = _target.GetTempFileName(".txt");
            const string f1Contents = "f1";
            string f2 = _target.GetTempFileName(".txt");
            const string f2Contents = "f2";
            string f3 = _target.GetTempFileName(".txt");
            const string f3Contents = "f3";

            try
            {
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

                Assert.IsFalse(File.Exists(f1), "{0} should not exist.", f1);
                Assert.IsFalse(File.Exists(f2), "{0} should not exist.", f2);
                Assert.IsTrue(File.Exists(f3), "{0} should exist.", f3);
            }
            finally
            {
                File.Delete(f1);
                File.Delete(f2);
                File.Delete(f3);
            }
        }

        #endregion

        //[Test]
        //public void Scratch()
        //{
        //    using (var scope = new TransactionScope(TransactionScopeOption.Required))
        //    {
        //        //throw new Exception("Test.");
        //        // _target.CreateDirectory(@"c:\temp\1\a");
        //        _target.CreateDirectory(@"\\VPC01\Temp\2\a\b\c");
        //    }
        //}
    }
}
