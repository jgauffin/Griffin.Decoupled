using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.Commands.Pipeline.Messages
{
    /// <summary>
    /// Used by transactional data stores to tell the next handler that a new command has been stored.
    /// </summary>
    /// <remarks>The purpose of this command is to make sure that all commands are stored before doing
    /// anything else. Just to make sure that all commands can get loaded again in case of system/dispatching failure.</remarks>
    public class CommandStored : IUpstreamMessage
    {
    }
}