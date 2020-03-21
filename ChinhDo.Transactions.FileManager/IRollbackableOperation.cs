namespace ChinhDo.Transactions
{
    /// <summary>
    /// Represents a transactional file operation.
    /// </summary>
    interface IRollbackableOperation
    {
        /// <summary>Executes the operation.</summary>
        void Execute();

        /// <summary>Rolls back the operation, restores the original state.</summary>
        void Rollback();
    }
}
