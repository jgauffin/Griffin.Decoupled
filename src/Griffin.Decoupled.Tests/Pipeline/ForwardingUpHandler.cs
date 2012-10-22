using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.Tests.Pipeline
{
    public class ForwardingUpHandler : IUpstreamHandler
    {
        /// <summary>
        /// Send a message to the next handler
        /// </summary>
        /// <param name="context">My context</param>
        /// <param name="message">Message received</param>
        public void HandleUpstream(IUpstreamContext context, IUpstreamMessage message)
        {
            Invoked = true;
            context.SendUpstream(message);
        }

        public bool Invoked { get; set; }
    }
}