using Griffin.Decoupled.Commands.Pipeline.Messages;

namespace Griffin.Decoupled.Commands.Pipeline
{
    /// <summary>
    /// Used to send messages downstream
    /// </summary>
    /// <remarks>Used when transporting messages from the invoker down to the handler.</remarks>
    public interface IDownstreamHandler
    {
        /// <summary>
        /// Send a message to the command handler
        /// </summary>
        /// <param name="context">my context</param>
        /// <param name="message">Message to send, typically <see cref="SendCommand"/>.</param>
        void HandleDownstream(IDownstreamContext context, object message);
    }
}