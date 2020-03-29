using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Transactions;

namespace ChinhDo.Transactions.TestConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Started.");

            RunStressTest();

            Console.WriteLine("Exiting.");
        }

        static void RunStressTest()
        {
            {
                // Pre-test checks
                string tempDir;
                IFileManager fm = new TxFileManager();
                using (TransactionScope s1 = new TransactionScope())
                {
                    tempDir = (new DirectoryInfo(fm.CreateTempDirectory())).Parent.FullName;
                }

                string[] directories = Directory.GetDirectories(tempDir);
                string[] files = Directory.GetFiles(tempDir);
                if (directories.Length > 0 || files.Length > 0)
                {
                    Console.WriteLine(string.Format("ERROR  Please ensure temp path {0} has no children before running this test.", tempDir));
                    return;
                }

            }

            // Start each test in its own thread and repeat for a few interations
            const int numThreads = 10;
            const int iterations = 1000;
            const string text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

            long count = 0;

            IList<Thread> threads = new List<Thread>();
            for (int i = 0; i < numThreads; i++)
            {
                Thread t = new Thread(() =>
                {
                    IFileManager fm = new TxFileManager();
                    for (int j = 0; j < iterations; j++)
                    {
                        using (TransactionScope s1 = new TransactionScope())
                        {
                            TransactionOptions to = new TransactionOptions();
                            to.Timeout = TimeSpan.FromMinutes(30);

                            long myCount = Interlocked.Increment(ref count);
                            if (myCount % 250 == 0)
                            {
                                Console.WriteLine(myCount + " (" + myCount * 100 / (numThreads * iterations) + " %)");
                            }

                            string f1 = fm.CreateTempFileName();
                            string f2 = fm.CreateTempFileName();
                            string d1 = fm.CreateTempDirectory();

                            if (i % 100 == 0) { Console.WriteLine(i); }

                            fm.AppendAllText(f1, text);
                            fm.Copy(f1, f2, false);

                            fm.CreateDirectory(d1);
                            fm.Delete(f2);
                            fm.DeleteDirectory(d1);
                            bool b1 = fm.DirectoryExists(d1);
                            bool b2 = fm.FileExists(f1);
                            string f3 = fm.CreateTempFileName();
                            fm.Move(f1, f3);
                            string f4 = fm.CreateTempFileName();
                            fm.Snapshot(f4);
                            fm.WriteAllBytes(f4, new byte[] { 64, 65 });
                            string f5 = fm.CreateTempFileName();
                            fm.WriteAllText(f5, text);

                            fm.Delete(f1);
                            fm.Delete(f2);
                            fm.Delete(f3);
                            fm.Delete(f4);
                            fm.Delete(f5);
                            fm.DeleteDirectory(d1);
                            s1.Complete();
                        }
                    }
                });

                threads.Add(t);
            }

            foreach (Thread t in threads)
            {
                t.Start();
            }

            Console.WriteLine("All threads started.");

            foreach (Thread t in threads)
            {
                t.Join();
            }

            Console.WriteLine("All threads joined.");

        }
    }
}
