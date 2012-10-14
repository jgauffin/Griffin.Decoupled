using System;
using Griffin.Decoupled.Commands;
using Griffin.Decoupled.Commands.Pipeline.Messages;

namespace Griffin.Decoupled.RavenDb.Commands
{
    /// <summary>
    /// Document used to save the commands in Raven
    /// </summary>
    public class StoredCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoredCommand" /> class.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <exception cref="System.ArgumentNullException">msg</exception>
        public StoredCommand(DispatchCommand msg)
        {
            if (msg == null) throw new ArgumentNullException("msg");
            Command = msg.Command;
            Id = Command.CommandId;
            Attempts = msg.Attempts;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredCommand" /> class.
        /// </summary>
        protected StoredCommand()
        {
        }

        /// <summary>
        /// Gets when we started processing the command
        /// </summary>
        /// <remarks>null = not being processed yet</remarks>
        public DateTime? ProcessedAt { get; set; }

        /// <summary>
        /// Gets the command id
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets or sets actual command
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// Gets or sets number of attempts to execute this command
        /// </summary>
        /// <remarks>Default = 0</remarks>
        public int Attempts { get; set; }
    }
}