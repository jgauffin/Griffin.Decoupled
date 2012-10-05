namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// A command storage which supports transactions
    /// </summary>
    public interface ITransactionalCommandStorage : ICommandStorage
    {
        /// <summary>
        /// Create a new transaction
        /// </summary>
        /// <returns>Newly created transaction</returns>
        ISimpleTransaction BeginTransaction();
    }
}