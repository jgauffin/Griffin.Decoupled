using System;
using Griffin.Decoupled.Commands;
using Griffin.Decoupled.Commands.Pipeline.Messages;

namespace Griffin.Decoupled.RavenDb
{
    /// <summary>
    /// Document used to save the commands in Raven
    /// </summary>
    public class StoredCommand
    {
        public StoredCommand(DispatchCommand msg)
        {
            if (msg == null) throw new ArgumentNullException("msg");
            Command = msg.Command;
            Id = Command.Id;
            Attempts = msg.Attempts;
        }

        protected StoredCommand()
        {
            
        }

        public DateTime? ProcessedAt { get; set; }
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

        public static string ToId(Guid guid)
        {
            return guid.ToString("D");
        }
    }
}