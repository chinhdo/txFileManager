using System;
using System.IO;

namespace ChinhDo.Transactions
{
    static class FileUtils
    {
        private static readonly string tempFolder = Path.Combine(Path.GetTempPath(), "TxFileMgr-fc4eed76ee9b");

        /// <summary>
        /// Ensures that the folder that contains the temporary files exists.
        /// </summary>
        public static void EnsureTempFolderExists()
        {
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }
        }

        /// <summary>
        /// Returns a unique temporary file name.
        /// </summary>
        /// <param name="extension"></param>
        public static string GetTempFileName(string extension)
        {
            Guid g = Guid.NewGuid();
            string retVal = Path.Combine(tempFolder, g.ToString().Substring(0, 16)) + extension;

            return retVal;
        }
    }
}
