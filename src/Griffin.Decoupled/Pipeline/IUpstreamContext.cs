using Griffin.Decoupled.Commands.Pipeline.Messages;

namespace Griffin.Decoupled.Pipeline
{
    /// <summary>
    /// Context for each handler.
    /// </summary>
    /// <remarks>The context will remain the same for the handler during the handlers lifetime, so it can safely be assigned as a member variable.</remarks>
    public interface IUpstreamContext
    {
        /// <summary>
        /// Send a message to the next handler
        /// </summary>
        /// <param name="message">Message to send, most commonly <see cref="SendCommand"/>.</param>
        void SendUpstream(object message);

        /// <summary>
        /// Try to send something down to the command handler again (or to a downstream handler on the way)
        /// </summary>
        /// <param name="message">Message to send</param>
        void SendDownstream(object message);
    }
}