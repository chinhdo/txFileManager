using System;
using System.IO;

namespace ChinhDo.Transactions
{
    /// <summary>Represents an I/O operation (file or directory)</summary>
    abstract class IoOperation
    {
        /// <summary>Constructor</summary>
        /// <param name="tempPath">Path to temp directory</param>
        public IoOperation(string tempPath)
        {
            this._tempPath = tempPath;
        }

        /// <summary>Ensures that the folder that contains the temporary files exists.</summary>
        public void EnsureTempFolderExists()
        {
            if (!Directory.Exists(this._tempPath))
            {
                Directory.CreateDirectory(this._tempPath);
            }
        }

        /// <summary>Returns a unique temporary file/directory name.</summary>
        public string GetTempPathName()
        {
            return GetTempPathName(string.Empty);
        }

        /// <summary>Returns a unique temporary file/directory name.</summary>
        /// <param name="extension">File extension. Example: ".txt"</param>
        public string GetTempPathName(string extension)
        {
            Guid g = Guid.NewGuid();
            string retVal = Path.Combine(this._tempPath, g.ToString().Substring(0, 16)) + extension;

            return retVal;
        }

        private string _tempPath;
    }
}
