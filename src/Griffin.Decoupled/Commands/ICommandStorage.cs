using System;

namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// Contract for command storage
    /// </summary>
    /// <remarks>Storage must be thread safe.
    /// <para>
    /// Storage is used to be able to dispatch commands at a later point and/or to continue later if the application shuts down/crashes. Each implementation should be thread safe
    /// since it will be invoked from all dispatcher threads. You are also reponsible of keeping the connection
    /// to the database open.
    /// </para>
    /// </remarks>
    public interface ICommandStorage
    {
        /// <summary>
        /// Enqueue a command
        /// </summary>
        /// <param name="command">Get the command which was </param>
        void Enqueue(CommandState command);

        /// <summary>
        /// Get command which was stored first.
        /// </summary>
        /// <returns>Command if any; otherwise <c>null</c>.</returns>
        CommandState Dequeue();
    }

    public interface ITransactionalCommandStorage : ICommandStorage
    {
        ISimpleTransaction BeginTransaction();
    }

    public interface ISimpleTransaction : IDisposable
    {
        void Commit();
    }
}