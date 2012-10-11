using System;

namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// Command contract
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Get command id
        /// </summary>
        Guid CommandId { get; }
    }
}