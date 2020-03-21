# Summary
.NET Transactional File Manager is a .NET library that supports including file system operations such
as file copy, move, delete, append, etc. in a transaction. It's an implementation of
System.Transaction.IEnlistmentNotification.

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

Feedback is welcome. Also if you have any suggestions for enhancements or bug reports please use the
discussions area. Better yet, join this project and contribute yourself.

This library is available as a [NuGet](https://www.nuget.org/packages/TxFileManager) package.

This started out as a [blog post](http://www.chinhdo.com/20080825/transactional-file-manager/). It was hosted on [CodePlex](https://archive.codeplex.com/?p=transactionalfilemgr) and migrated to GitHub in 3/2020.


# Frequently Ased Questions
## How do I run tests?

In a PowerShell/Command window:
* dotnet test

In Visual Studio, open Test/Text Explorer and choose Run All Tests (or CTRL-R, A)

## How do I reference this library?

You can run this command:

```
dotnet add package TxFileManager --version <version>
```

Or Use Visual Studio's Manage Nuget Packages to add. Search for "TxFileManager".

## Can I reuse instances of TxFileManager?

It's not expensive to create new instances of TxFileManager as eeded. There's a bit of overhead (like
creating instances of any small class) but not much.

On the other hand, you can also re-use the same instance for multiple transactions, even nested
transactions.

## Is TxFileManager Thread Safe?

Yes - it's been tested for that.

## Which IsolationLevel's are supported?

Regardless of the specified IsolationLevel, the effective IsolationLevel is ReadUncommitted.