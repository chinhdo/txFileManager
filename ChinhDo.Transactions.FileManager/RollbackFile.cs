using System.IO;

namespace ChinhDo.Transactions
{
    class RollbackFile : RollbackOperation
    {
        public RollbackFile(string fileName)
        {
            _originalFileName = fileName;

            if (File.Exists(fileName))
            {
                _backupFileName = FileUtils.GetTempFileName(Path.GetExtension(fileName));
                File.Copy(_originalFileName, _backupFileName);
            }
        }

        public override void Rollback()
        {
            if (_backupFileName != null)
            {
                string strDirectory = Path.GetDirectoryName(_originalFileName);
                if (!Directory.Exists(strDirectory))
                    Directory.CreateDirectory(strDirectory);
                File.Copy(_backupFileName, _originalFileName, true);
            }
            else
            {
                if (File.Exists(_originalFileName))
                    File.Delete(_originalFileName);
            }
        }

        public override void CleanUp()
        {
            if (_backupFileName != null)
            {
                FileInfo fi = new FileInfo(_backupFileName);
                if (fi.IsReadOnly)
                {
                    fi.Attributes = FileAttributes.Normal;
                }
                File.Delete(_backupFileName);
            }
        }

        public override string ToString()
        {
            return GetType().Name + "-" + _originalFileName;
        }

        private readonly string _originalFileName;
        private readonly string _backupFileName;
    }
}
