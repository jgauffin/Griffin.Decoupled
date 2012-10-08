using System;

namespace Griffin.Decoupled.Commands.Pipeline.Messages
{
    /// <summary>
    /// Want to dispatch a command through the pipeline
    /// </summary>
    public class DispatchCommand
    {
        public DispatchCommand(ICommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            Command = command;
        }

        public DispatchCommand(ICommand command, int attempts)
        {
            if (command == null) throw new ArgumentNullException("command");
            Command = command;
            Attempts = attempts;
        }

        protected DispatchCommand()
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