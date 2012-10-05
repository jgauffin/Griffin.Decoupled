using System;
using Griffin.Decoupled.Commands.Pipeline.Messages;

namespace Griffin.Decoupled.RavenDb
{
    /// <summary>
    /// Document used to save the commands in Raven
    /// </summary>
    public class StoredCommand
    {
        private readonly SendCommand _command;

        public StoredCommand(SendCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            _command = command;
            Id = command.Command.Id;
        }
        public SendCommand Command { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public Guid Id { get; private set; }
    }
}