# Summary
.NET Transactional File Manager is a .NET API that supports including file system operations such
as file copy, move, delete, append, etc. in a transaction. It's an implementation of
System.Transaction.IEnlistmentNotification.

This project was originally on [CodePlex](https://archive.codeplex.com/?p=transactionalfilemgr) and
was migrated to GitHub using [fast-export](https://github.com/frej/fast-export) by Frej Drejhammar.

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
* Delete
* DeleteDirectory
* Move
* Snapshot
* WriteAllBytes
* WriteAllText

This started out as a [blog post](http://www.chinhdo.com/20080825/transactional-file-manager/).

Feedback is welcome. Also if you have any suggestions for enhancements or bug reports please use the
discussions area. Better yet, join this project and contribute yourself.

This library is available as a [NuGet](https://www.nuget.org/packages/TxFileManager) package.

# Frequently Ased Questions
## How do I run tests?

In a PowerShell/Command window:
* cd <path to ChinhDo.Transactions.FileManagerTest> folder
* dotnet test

In Visual Studio, open Test/Text Explorer and choose Run All Tests (or CTRL-R, A)

## Can I reuse instances of TxFileManager?

It's not expensive to create new instances of TxFileManager as eeded. There's a bit of overhead (like
creating instances of any small class) but not much.

On the other hand, you can also re-use the same instance for multiple transactions, even nested
transactions.

## Is TxFileManager Thread Safe?

Yes - it's been tested for that.

## Which IsolationLevel's are supported?

Regardless of the specified IsolationLevel, the effective IsolationLevel is ReadUncommitted.

## What does the release roadmap look like?

I am working on version 1.4 with a few bug fixes and enhancements. See the Issues list.

## TODO's
* Licencing/Expression in Package
* Use IFileManager in tests and ensure all ops are in IFileManager
* Stress test - long running test for leaks