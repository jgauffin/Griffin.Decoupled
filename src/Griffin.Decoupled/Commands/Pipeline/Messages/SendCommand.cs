using System;

namespace Griffin.Decoupled.Commands.Pipeline.Messages
{
    /// <summary>
    /// Command which is stored
    /// </summary>
    public class SendCommand
    {
        public SendCommand(ICommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            Command = command;
        }

        protected SendCommand()
        {
            
        }

        /// <summary>
        /// Gets or sets actual command
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// Gets or sets number of attempts to execute this command
        /// </summary>
        /// <remarks>Default = 0</remarks>
        public int Attempts { get; set; }

        /// <summary>
        /// Gets or sets why last attempt failed.
        /// </summary>
        public string LastException { get; set; }
    }
}