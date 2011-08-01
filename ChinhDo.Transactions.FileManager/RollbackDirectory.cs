using System;
using System.Diagnostics;
using System.IO;

namespace ChinhDo.Transactions
{
    class RollbackDirectory : RollbackOperation
    {
        public RollbackDirectory(string path)
        {
            _path = path;
            _existed = Directory.Exists(path);
        }

        public override void Rollback()
        {
            if (!_existed)
            {
                if (Directory.GetFiles(_path).Length == 0 && Directory.GetDirectories(_path).Length == 0)
                {
                    // Delete the dir only if it's empty
                    Directory.Delete(_path);
                }
                else
                {
                    throw new Exception("Failed to delete directory " + _path + ". Directory was not empty.");
                }
            }
        }

        public override void CleanUp()
        {
            // Nothing to do
        }

        public override string ToString()
        {
            return GetType().Name + "-" + _path;
        }

        private readonly bool _existed;
        private readonly string _path;
    }
}
