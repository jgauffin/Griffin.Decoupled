using Griffin.Decoupled.Commands.Pipeline.Messages;

namespace Griffin.Decoupled.Pipeline
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
        /// <param name="message">Message to send, typically <see cref="DispatchCommand"/>.</param>
        void HandleDownstream(IDownstreamContext context, IDownstreamMessage message);
    }
}