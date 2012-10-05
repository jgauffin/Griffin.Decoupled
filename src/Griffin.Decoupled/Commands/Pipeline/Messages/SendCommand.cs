using System;

namespace Griffin.Decoupled.Commands.Pipeline.Messages
{
    /// <summary>
    /// Want to dispatch a command
    /// </summary>
    public class SendCommand
    {
        public SendCommand(ICommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            Command = command;
        }

        public SendCommand(ICommand command, int attempts)
        {
            if (command == null) throw new ArgumentNullException("command");
            Command = command;
            Attempts = attempts;
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
        public int Attempts { get; private set; }

        /// <summary>
        /// Failed once more.
        /// </summary>
        public void AddFailedAttempt()
        {
            Attempts++;
        }

    }
}