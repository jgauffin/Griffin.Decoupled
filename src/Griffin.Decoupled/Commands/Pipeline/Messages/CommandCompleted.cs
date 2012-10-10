using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffin.Decoupled.Commands.Pipeline.Messages
{
    /// <summary>
    /// A command has been successfully processed.
    /// </summary>
    public class CommandCompleted
    {
        public CommandCompleted(DispatchCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            Message = message;
        }

        /// <summary>
        /// Gets the dispatch message for the command which was completed.
        /// </summary>
        public DispatchCommand Message { get; private set; }
    }
}
