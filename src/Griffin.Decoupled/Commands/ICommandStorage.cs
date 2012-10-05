using System;
using System.Collections.Generic;
using Griffin.Decoupled.Commands.Pipeline.Messages;

namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// Contract for command storage
    /// </summary>
    /// <remarks><para>Storage must be thread safe. And you have to make sure that the command operations are atomic
    /// so that no other thread can work with the same command.</para>
    /// <para>The workflow is:
    /// <list type="number">
    /// <item>Dequeue</item>
    /// <item>Dispatch command</item>
    /// <item>Delete command</item>
    /// </list>
    /// Which means that any failing command will eventually be detected (by <c>FindFailedCommands()</c>) and processed accordingly.
    /// </para>
    /// <para>Important! The last downstream handler must remove the command upon complection</para>
    /// </remarks>
    public interface ICommandStorage
    {
        /// <summary>
        /// Add a new command
        /// </summary>
        /// <param name="command">Store the command in the DB. You can use the <see cref="ICommand.Id"/> as an identity.</param>
        void Add(SendCommand command);

        /// <summary>
        /// Dequeue a command (get and and mark it as being processed so that no other threads can access it)
        /// </summary>
        /// <returns>Command if any; otherwise <c>null</c>.</returns>
        SendCommand Dequeue();

        /// <summary>
        /// Re add a command which we've tried to invoke but failed.
        /// </summary>
        /// <param name="command">Command to add</param>
        void Update(SendCommand command);

        /// <summary>
        /// Delete a command
        /// </summary>
        /// <param name="command">Command to delete from storage</param>
        void Delete(ICommand command);

        /// <summary>
        /// Find commands which has been marked as processed but not deleted.
        /// </summary>
        /// <param name="markedAsProcessBefore">Get all commands that were marked as being processed before this date/time.</param>
        /// <returns>Any matching commands or an empty collection.</returns>
        IEnumerable<SendCommand> FindFailedCommands(DateTime markedAsProcessBefore);
    }
}