using System;
using System.Threading;
using System.Transactions;

namespace ChinhDo.Transactions.TestConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test .NET Core app");

            RunStressTest();
        }

        static void RunStressTest()
        {
            // Start each test in its own thread and repeat for a few interations
            const int iterations = 100000;
            const string text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

            IFileManager fm = new TxFileManager();
            string f1 = fm.GetTempFileName();
            for (int i = 0; i < iterations; i++)
            {
                using (TransactionScope s1 = new TransactionScope())
                {
                    if (i % 100 == 0) {Console.WriteLine(i);}
                    
                    fm.AppendAllText(f1, text);
                    s1.Complete();
                }

                Thread.Sleep(25);

            }
        }

    }
}
