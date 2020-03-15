# Summary
.NET Transactional File Manager is a .NET API that supports including file system operations such as file copy, move, delete, append, etc. in a transaction. It's an implementation of System.Transaction.IEnlistmentNotification (works with System.Transactions.TransactionScope).

This project was originally on [CodePlex](https://archive.codeplex.com/?p=transactionalfilemgr) and was migrated to GitHub using [fast-export](https://github.com/frej/fast-export) by Frej Drejhammar.

This library allows you to wrap file system operations in transactions like this:

```
// Wrap a file copy and a database insert in the same transaction
TxFileManager fileMgr = new TxFileManager();
using (TransactionScope scope1 = new TransactionScope())
{
    // Copy a file
    fileMgr.Copy(srcFileName, destFileName);

    // Insert a database record
    dbMgr.ExecuteNonQuery(insertSql);

    scope1.Complete();
} 
```

# Current features

Support the following file operations in transactions:
* AppendAllText
* Copy
* CreateDirectory
* DeleteDirectory
* DeleteFile
* Move
* Snapshot
* WriteAllText

This started out as a [blog post](http://www.chinhdo.com/20080825/transactional-file-manager/).

Feedback is very welcome. Also if you have any suggestions for enhancements or bug reports please use the discussions area. Better yet, join this project and contribute yourself.

This library is also available from [NuGet](https://www.nuget.org/packages/TxFileManager)

# Frequently Ased Questions
## How do I run tests?

In Visual Studio, open Test/Text Explorer and choose Run All Tests (or CTRL-R, A)

## Can I reuse instances of TxFileManager?

It's not expensive to create new instances of TxFileManager as eeded. There's a bit of overhead (like creating instances of any small class) but not much.

On the other hand, you can also re-use the same instance for multiple transactions, even nested transactions.

## Is TxFileManager Thread Safe?

Yes - it's been tested for that.

## Which IsolationLevel's are supported?

Regardless of the specified IsolationLevel, the effective IsolationLevel is ReadUncommitted.

## What does the release roadmap look like?

I am working on upgrading the library to the latest .NET Framework and also to make sure it works with .NET Core on Linux. Next will be some enhancement requests (like adding support for copyping/moving/deleting directories) that were pending from the CodePlex project.