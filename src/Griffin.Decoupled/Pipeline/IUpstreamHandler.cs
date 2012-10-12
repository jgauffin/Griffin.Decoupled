namespace Griffin.Decoupled.Pipeline
{
    /// <summary>
    /// Used to send messages upstream
    /// </summary>
    /// <remarks>Used when transporting messages from the handler up to the invoker. Typically error messages</remarks>
    public interface IUpstreamHandler
    {
        /// <summary>
        /// Send a message to the next handler
        /// </summary>
        /// <param name="context">My context</param>
        /// <param name="message">Message received</param>
        void HandleUpstream(IUpstreamContext context, IUpstreamMessage message);
    }
}