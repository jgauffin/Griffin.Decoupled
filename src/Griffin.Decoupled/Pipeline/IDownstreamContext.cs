namespace Griffin.Decoupled.Pipeline
{
    /// <summary>
    /// Context for each handler.
    /// </summary>
    /// <remarks>The context will remain the same for the handler during the handlers lifetime, so it can safely be assigned as a member variable.</remarks>
    public interface IDownstreamContext
    {
        /// <summary>
        /// Send a message back up the chain, typically an error message
        /// </summary>
        /// <param name="message">Message to send</param>
        void SendUpstream(IUpstreamMessage message);

        /// <summary>
        /// Send a message towards the command handler
        /// </summary>
        /// <param name="message">Message to forward</param>
        void SendDownstream(IDownstreamMessage message);
    }
}