using System.IO;

namespace ChinhDo.Transactions
{
    /// <summary>
    /// Rollbackable operation which moves a file to a new location.
    /// </summary>
    sealed class MoveOperation : IRollbackableOperation
    {
        private readonly string sourceFileName;
        private readonly string destFileName;

        /// <summary>
        /// Instantiates the class.
        /// </summary>
        /// <param name="sourceFileName">The name of the file to move.</param>
        /// <param name="destFileName">The new path for the file.</param>
        public MoveOperation(string sourceFileName, string destFileName)
        {
            this.sourceFileName = sourceFileName;
            this.destFileName = destFileName;
        }

        public void Execute()
        {
            File.Move(sourceFileName, destFileName);
        }

        public void Rollback()
        {
            File.Move(destFileName, sourceFileName);
        }
    }
}
