using System;
using Griffin.Decoupled.Commands.Pipeline.Messages;

namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// Contract for command storage
    /// </summary>
    /// <remarks>Storage must be thread safe.</remarks>
    public interface ICommandStorage
    {
        /// <summary>
        /// Enqueue a command
        /// </summary>
        /// <param name="command">Get the command which was </param>
        void Enqueue(SendCommand command);

        /// <summary>
        /// Get command which was stored first.
        /// </summary>
        /// <returns>Command if any; otherwise <c>null</c>.</returns>
        SendCommand Dequeue();
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