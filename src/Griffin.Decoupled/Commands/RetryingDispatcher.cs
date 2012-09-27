using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// A dispatcher that will retry a command if it fails.
    /// </summary>
    /// <remarks>Decorater pattern for those who are interested.</remarks>
    public class RetryingDispatcher : ICommandDispatcher
    {
        private readonly ICommandDispatcher _inner;
        private readonly int _numberOfAttempts;

        public RetryingDispatcher(ICommandDispatcher inner, int numberOfAttempts)
        {
            _inner = inner;
            _numberOfAttempts = numberOfAttempts;
        }

        /// <summary>
        /// Dispatch the command to the handler
        /// </summary>
        /// <typeparam name="T">Type of command</typeparam>
        /// <param name="command">Command to execute</param>
        /// <remarks>Implementations should throw exceptions unless they are asynchronous or will attempt to retry later.</remarks>
        public void Dispatch<T>(T command) where T : class, ICommand
        {
            
        }
    }

    /// <summary>
    /// Command which is stored
    /// </summary>
    public class StoredCommand
    {
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
