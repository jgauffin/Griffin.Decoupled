using Griffin.Decoupled.Commands.Pipeline.Messages;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.Tests.Pipeline
{
    public class ForwardingDownHandler : IDownstreamHandler
    {
        /// <summary>
        /// Send a message to the command handler
        /// </summary>
        /// <param name="context">my context</param>
        /// <param name="message">Message to send, typically <see cref="DispatchCommand"/>.</param>
        public void HandleDownstream(IDownstreamContext context, IDownstreamMessage message)
        {
            Invoked = true;
            context.SendDownstream(message);
        }

        public bool Invoked { get; set; }
    }
}