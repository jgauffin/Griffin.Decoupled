using System;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// Uses a pipeline to dispatch the messages.
    /// </summary>
    /// <remarks>A pipeline allows us to add several features to the dispatcher in an order manner. Both for failures (upstream) and the command dispatching (downstream).</remarks>
    public class PipelineDispatcher : ICommandDispatcher, IUpstreamHandler, IDisposable
    {
        private readonly IPipeline _pipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineDispatcher" /> class.
        /// </summary>
        /// <param name="pipeline">The pipeline to use. This class will assign itself as the destination.</param>
        public PipelineDispatcher(IPipeline pipeline)
        {
            if (pipeline == null) throw new ArgumentNullException("pipeline");
            _pipeline = pipeline;
            _pipeline.SetDestination(this);
        }

        #region ICommandDispatcher Members

        /// <summary>
        /// Dispatch the command to the handler
        /// </summary>
        /// <typeparam name="T">Type of command</typeparam>
        /// <param name="command">Command to execute</param>
        /// <remarks>Implementations should throw exceptions unless they are asynchronous or will attempt to retry later.</remarks>
        public void Dispatch<T>(T command) where T : class, ICommand
        {
            if (command == null) throw new ArgumentNullException("command");

            var msg = new DispatchCommand(command);
            _pipeline.Send(msg);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            _pipeline.Send(new Shutdown());
        }

        #endregion

        #region IUpstreamHandler Members

        /// <summary>
        /// Send a message to the next handler
        /// </summary>
        /// <param name="context">My context</param>
        /// <param name="message">Message received</param>
        public void HandleUpstream(IUpstreamContext context, object message)
        {
            Console.WriteLine(message);
        }

        #endregion
    }
}