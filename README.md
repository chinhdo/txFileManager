Transactional File Manager is a .NET Standard library that supports including file system operations such
as file copy, move, delete, append, etc. in a transaction. It's an implementation of
System.Transaction.IEnlistmentNotification.

This library allows you to wrap file system operations in transactions like this:

``` csharp
// Wrap a file copy and a database insert in the same transaction
IFileManager fm = new TxFileManager();
using (TransactionScope scope1 = new TransactionScope())
{
    // Copy a file
    fm.Copy(srcFileName, destFileName);

    // Insert a database record
    db.ExecuteNonQuery(insertSql);

    scope1.Complete();
} 
```

# Current features

Support the following file operations in transactions:
* AppendAllText: Appends the specified string the file, creating the file if it doesn't already exist.
* Copy: Copy a file to another file.
* Delete: Delete a file.
* Move: Move a file.
* CreateDirectory: Create a directory.
* DeleteDirectory: Delete a directory.
* MoveDirectory: Move a directory.
* Snapshot: Take a snapshot of the specified file. The snapshot is used to rollback the file later if needed.
* WriteAllBytes: Write the specified bytes to the file.
* WriteAllText: Write the specified text content to the file.

If you have any suggestions for enhancements or bug reports please use the Issues list. Better yet, if possible join this project and contribute.

This library is available as a [NuGet](https://www.nuget.org/packages/TxFileManager) package. The current/latest release is 1.4.

This started out as a [blog post](http://www.chinhdo.com/20080825/transactional-file-manager/). It was hosted on [CodePlex](https://archive.codeplex.com/?p=transactionalfilemgr) and migrated to GitHub in 3/2020.

Additional contributors: @gvas, AirBreather.

# Quick Start

1. Add a reference to TxFileManager

```
dotnet add package TxFileManager
```

2. Start writing code

``` csharp
IFileManager fm = new TxFileManager();
using (TransactionScope scope1 = new TransactionScope())
{
    // Copy a file
    fm.Copy(srcFileName, destFileName);

    scope1.Complete();
} 
```

# Frequently Asked Questions
## How do I run tests?

In a PowerShell/Command window:
```
dotnet test
```

In Visual Studio, open Test/Text Explorer and choose Run All Tests (or CTRL-R, A)

## How do I reference this library?

The recommended method is to add a NuGet reference:

```
dotnet add package TxFileManager
```

Or Use Visual Studio's Manage NuGet Packages to add. Search for "TxFileManager".

## Can I reuse instances of TxFileManager?

It's not expensive to create new instances of TxFileManager as needed. There's a bit of overhead (like creating instances of any small class) but not much.

On the other hand, you can also re-use the same instance for multiple transactions, even nested transactions.

## Is TxFileManager Thread Safe?

Yes - it's been tested for that.

## Which IsolationLevel's are supported?

Regardless of the specified IsolationLevel, the effective IsolationLevel is ReadUncommitted.

## How does TxFileManager work?

See Chinh's blog post: [Include File Operations in Your Transactions Today with IEnlistmentNotification](https://www.chinhdo.com/20080825/transactional-file-manager/)

## Where are temporary files/directories kept?

By default, the path returned by Path.GetTempPath() is used to keep temporary files/directories used by TxFileManager. However, you can override that and have TxFileManager use another temp path:

```csharp
IFileManager fm = new TxFileManager(myTempPath);
```
## What is the future release roadmap?

Looking for feature suggestions to be included in the next version. Please suggest your features using the [Issues](https://github.com/chinhdo/txFileManager/issues) list.

[Issues planned for version 1.5](https://github.com/chinhdo/txFileManager/issues?q=is%3Aopen+is%3Aissue+project%3Achinhdo%2FtxFileManager%2F1)

# Release Notes
## Version 1.4
* Convert to xUnit tests
* Add support for custom temp paths to address issues with file/dir operations accross filesystems
* Fix for resource leak in TxFileManager._txEnlistment
* Target .NET Standard 2.0
* Additional testing for .NET Core on Ubuntu
* Additional stress testing both on Windows and Ubuntu
* Created Github workflow to automatically build/test on ubuntu
* Added FxCop static analysis
