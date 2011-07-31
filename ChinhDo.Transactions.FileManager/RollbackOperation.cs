namespace ChinhDo.Transactions
{
    /// <summary>
    /// Represents a transactional file operation.
    /// </summary>
    abstract class RollbackOperation
    {
        public abstract void Rollback();
        public abstract void CleanUp();
    }
}
