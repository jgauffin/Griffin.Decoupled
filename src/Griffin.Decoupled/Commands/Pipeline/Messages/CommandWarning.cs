    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffin.Decoupled.Commands.Pipeline.Messages
{
    /// <summary>
    /// A command could not be delivered and we'll therefore give up on it.
    /// </summary>
    public class CommandAborted
    {
        public CommandAborted(SendCommand command, Exception exception)
        {
            
        }
    }
}
