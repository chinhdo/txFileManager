using System.Text;
using System;

namespace ChinhDo.Transactions
{
    /// <summary>
    /// Classes implementing this interface provide methods to work with files.
    /// </summary>
    public interface IFileManager
    {
        /// <summary>
        /// Appends the specified string the file, creating the file if it doesn't already exist.
        /// </summary>
        /// <param name="path">The file to append the string to.</param>
        /// <param name="contents">The string to append to the file.</param>
        void AppendAllText(string path, string contents);

        /// <summary>
        /// Copies the specified <paramref name="sourceFileName"/> to <paramref name="destFileName"/>.
        /// </summary>
        /// <param name="sourceFileName">The file to copy.</param>
        /// <param name="destFileName">The name of the destination file.</param>
        /// <param name="overwrite">true if the destination file can be overwritten, otherwise false.</param>
        void Copy(string sourceFileName, string destFileName, bool overwrite);

        /// <summary>
        /// Copy a directory
        /// </summary>
        /// <param name="srcDir">Source directory</param>
        /// <param name="destDir">Destination directory</param>
        void CopyDirectory(string srcDir, string destDir);

        /// <summary>
        /// Creates all directories in the specified path.
        /// </summary>
        /// <param name="path">The directory path to create.</param>
        void CreateDirectory(string path);

        /// <summary>
        /// Creates a temporary file name. The file is not automatically created.
        /// </summary>
        /// <param name="extension">File extension (with the dot).</param>
        string CreateTempFileName(string extension);

        /// <summary>
        /// Creates a temporary filename. The file is not automatically created.
        /// </summary>
        string CreateTempFileName();

        /// <summary>Creates a temporary directory.</summary>
        /// <returns>The path to the newly created temporary directory.</returns>
        string CreateTempDirectory();

        /// <summary>Create a temporary directory name.</summary>
        /// <param name="parentDirectory">The parent directory.</param>
        /// <param name="prefix">The prefix of the directory name.</param>
        /// <returns>Path to the temporary directory. The temporary directory is created automatically.</returns>
        string CreateTempDirectory(string parentDirectory, string prefix);

        /// <summary>
        /// Deletes the specified file. An exception is not thrown if the file does not exist.
        /// </summary>
        /// <param name="path">The file to be deleted.</param>
        void Delete(string path);

        /// <summary>
        /// Deletes the specified directory and all its contents. An exception is not thrown if the directory does not exist.
        /// </summary>
        /// <param name="path">The directory to be deleted.</param>
        void DeleteDirectory(string path);

        /// <summary>
        /// Determines whether the specified path refers to a directory that exists on disk.
        /// </summary>
        /// <param name="path">The directory to check.</param>
        /// <returns>True if the directory exists.</returns>
        bool DirectoryExists(string path);

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="path">The file to check.</param>
        /// <returns>True if the file exists.</returns>
        bool FileExists(string path);

        /// <summary>Gets the files in the specified directory.</summary>
        /// <param name="path">The directory to get files.</param>
        /// <param name="handler">The <see cref="FileEventHandler"/> object to call on each file found.</param>
        /// <param name="recursive">if set to <c>true</c>, include files in sub directories recursively.</param>
        void GetFiles(string path, FileEventHandler handler, bool recursive);

        /// <summary>
        /// Gets the path to the temp folder.
        /// </summary>
        string GetTempPath();

        /// <summary>
        /// Moves the specified file to a new location.
        /// </summary>
        /// <param name="srcFileName">The name of the file to move.</param>
        /// <param name="destFileName">The new path for the file.</param>
        void Move(string srcFileName, string destFileName);

        /// <summary>
        /// Moves the specified directory to a new location.
        /// </summary>
        /// <param name="srcDirName">The name of the directory to move.</param>
        /// <param name="destDirName">The new path for the directory.</param>
        void MoveDirectory(string srcDirName, string destDirName);

        /// <summary>
        /// Take a snapshot of the specified file. The snapshot is used to rollback the file later if needed.
        /// </summary>
        /// <param name="fileName">The file to take a snapshot for.</param>
        void Snapshot(string fileName);

        /// <summary>
        /// Creates a file, write the specified <paramref name="contents"/> to the file.
        /// </summary>
        /// <param name="path">The file to write to.</param>
        /// <param name="contents">The string to write to the file.</param>
        void WriteAllText(string path, string contents);

        /// <summary>
        /// Creates a file, write the specified <paramref name="contents"/> to the file.
        /// </summary>
        /// <param name="path">The file to write to.</param>
        /// <param name="contents">The string to write to the file.</param>
        /// <param name="encoding">Encoding</param>
        void WriteAllText(string path, string contents, Encoding encoding);

        /// <summary>
        /// Creates a file, write the specified <paramref name="contents"/> to the file.
        /// </summary>
        /// <param name="path">The file to write to.</param>
        /// <param name="contents">The bytes to write to the file.</param>
        void WriteAllBytes(string path, byte[] contents);
    }
}
