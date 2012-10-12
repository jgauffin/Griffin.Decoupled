using System;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.Commands.Pipeline.Messages
{
    /// <summary>
    /// Want to dispatch a command through the pipeline
    /// </summary>
    public class DispatchCommand : IDownstreamMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DispatchCommand" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <exception cref="System.ArgumentNullException">command</exception>
        public DispatchCommand(ICommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            Command = command;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatchCommand" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="attempts">Number of attempts which have already been made.</param>
        /// <exception cref="System.ArgumentNullException">command</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">attempts;Really? I do not think that you have retried the command that many times. 0-10 is reasonable.</exception>
        public DispatchCommand(ICommand command, int attempts)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (attempts < 0 || attempts > 10)
                throw new ArgumentOutOfRangeException("attempts", attempts, "Really? I do not think that you have retried the command that many times. 0-10 is reasonable.");
            Command = command;
            Attempts = attempts;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatchCommand" /> class.
        /// </summary>
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