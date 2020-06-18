using System.IO;

namespace ChinhDo.Transactions
{
	/// <summary>Rollbackable operation which moves a directory to a new location.</summary>
	sealed class MoveDirectoryOperation : IRollbackableOperation
	{
		private readonly string sourceFileName;
		private readonly string destFileName;

		/// <summary>Instantiates the class.</summary>
		/// <param name="sourceFileName">The name of the directory to move.</param>
		/// <param name="destFileName">The new path for the directory.</param>
		public MoveDirectoryOperation(string sourceFileName, string destFileName)
		{
			this.sourceFileName = sourceFileName;
			this.destFileName = destFileName;
		}

		public void Execute()
		{
			Directory.Move(sourceFileName, destFileName);
		}

		public void Rollback()
		{
			Directory.Move(destFileName, sourceFileName);
		}
	}
}