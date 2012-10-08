using System;

namespace Griffin.Decoupled.Commands.Pipeline.Messages
{
    /// <summary>
    /// A command could not be delivered and we'll therefore give up on it.
    /// </summary>
    public class CommandAborted
    {
        public CommandAborted(DispatchCommand command, Exception exception)
        {
        }
    }
}