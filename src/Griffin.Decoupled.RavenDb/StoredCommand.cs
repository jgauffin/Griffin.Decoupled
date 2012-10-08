using System;
using Griffin.Decoupled.Commands.Pipeline.Messages;

namespace Griffin.Decoupled.RavenDb
{
    /// <summary>
    /// Document used to save the commands in Raven
    /// </summary>
    public class StoredCommand
    {
        public StoredCommand(DispatchCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            Command = command;
            Id = command.Command.Id.ToString("D");
        }

        public DispatchCommand Command { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string Id { get; private set; }
    }
}