namespace ChinhDo.Transactions
{
    /// <summary>
    /// Classes implementing this interface provide methods to work with files.
    /// </summary>
    public interface IFileManager : IFileOperations
    {
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
    }
}
