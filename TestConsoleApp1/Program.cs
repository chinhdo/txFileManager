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

            IFileManager fm = new TxFileManager();

            for (int i=0; i<10; i++)
            {
                using (TransactionScope scope1 = new TransactionScope())
                {
                    String f1 = "TestFile1.txt";
                    fm.AppendAllText(f1, "Hello");
                    Console.WriteLine("Appended some text to " + f1 + ".");
                    // scope1.Complete();
                }

                using (TransactionScope scope1 = new TransactionScope())
                {
                    String f1 = "TestFile2.txt";
                    fm.AppendAllText(f1, "Hello");
                    Console.WriteLine("Appended some text to " + f1 + ".");
                    scope1.Complete();
                }

                Thread.Sleep(250);
            }
        }

    }
}
