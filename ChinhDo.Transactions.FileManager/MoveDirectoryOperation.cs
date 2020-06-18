using System.IO;

namespace ChinhDo.Transactions
{
	/// <summary>Rollbackable operation which moves a directory to a new location.</summary>
	sealed class MoveDirectoryOperation : IRollbackableOperation
	{
		private readonly string sourceDirName;
		private readonly string destDirName;

		/// <summary>Instantiates the class.</summary>
		/// <param name="sourceDirName">The name of the directory to move.</param>
		/// <param name="destDirName">The new path for the directory.</param>
		public MoveDirectoryOperation(string sourceDirName, string destDirName)
		{
			this.sourceDirName = sourceDirName;
			this.destDirName = destDirName;
		}

		public void Execute()
		{
			Directory.Move(sourceDirName, destDirName);
		}

		public void Rollback()
		{
			Directory.Move(destDirName, sourceDirName);
		}
	}
}