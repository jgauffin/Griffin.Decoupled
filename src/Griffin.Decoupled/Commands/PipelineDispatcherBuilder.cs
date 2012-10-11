using System;
using Griffin.Decoupled.Commands.Pipeline;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.Commands
{
    /// <summary>Fluent pipeline builder
    /// </summary>
    /// <remarks>
    /// <para>Check the Pipeline namespace documention for information about a pipeline.</para>
    /// </remarks>
    public class PipelineDispatcherBuilder
    {
        private readonly IUpstreamHandler _errrorHandler;
        private IRootContainer _container;
        private IDownstreamHandler _lastHandler;
        private int _maxAttempts;
        private StorageHandler _storageHandler;
        private int _workers;

        public PipelineDispatcherBuilder(IUpstreamHandler errrorHandler)
        {
            _errrorHandler = errrorHandler;
        }

        /// <summary>
        /// Store commands in a custom data storage
        /// </summary>
        /// <param name="storage">The storage</param>
        /// <returns>this</returns>
        /// <remarks>The memory is used per default</remarks>
        public PipelineDispatcherBuilder StoreCommands(ICommandStorage storage)
        {
            if (storage == null) throw new ArgumentNullException("storage");

            _storageHandler = new StorageHandler(storage);
            return this;
        }

        /// <summary>
        /// Make the dispatching asynchronous
        /// </summary>
        /// <param name="maxConcurrentCommands">Number of commands that can be executed simultaneously.</param>
        /// <returns>this</returns>
        public PipelineDispatcherBuilder AsyncDispatching(int maxConcurrentCommands)
        {
            if (maxConcurrentCommands < 0 || maxConcurrentCommands > 10)
                throw new ArgumentOutOfRangeException("maxConcurrentCommands", maxConcurrentCommands,
                                                      "1 to 100 is what we deem reasonable :O");

            _workers = maxConcurrentCommands;
            return this;
        }

        /// <summary>
        /// Retry all failing commands before disposing them
        /// </summary>
        /// <param name="maxAttempts">Number of attempts for failing commands</param>
        /// <returns>this</returns>
        public PipelineDispatcherBuilder RetryCommands(int maxAttempts)
        {
            if (maxAttempts < 0 || maxAttempts > 10)
                throw new ArgumentOutOfRangeException("maxAttempts", maxAttempts, "Attempts should be between 1 and 10.");

            _maxAttempts = maxAttempts;
            return this;
        }

        /// <summary>
        /// Use an inversion of control container to execute the correct command handler.
        /// </summary>
        /// <param name="container">IoC adapter.</param>
        /// <returns>this</returns>
        public PipelineDispatcherBuilder UseContainer(IRootContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");
            _container = container;
            return this;
        }


        /// <summary>
        /// Build the dispatcher.
        /// </summary>
        /// <returns>Created dispatcher</returns>
        public ICommandDispatcher Build()
        {
            if (_lastHandler == null && _container == null)
                throw new InvalidOperationException(
                    "You must have specified a handler which can actually invoke the correct command handler. For instance the 'IocDispatcher'.");
            if (_container == null && _lastHandler == null)
                throw new InvalidOperationException(
                    "You must have specified a SINGLE handler that can invoke the correct command handler. Either use 'IocDispatcher' or another one, not both alternatives.");

            var builder = new PipelineBuilder();

            // must be registered before the storage since the storage listens on CommandAborted
            if (_maxAttempts > 0)
            {
                var handler = new RetryingHandler(_maxAttempts);
                builder.RegisterUpstream(handler);
            }

            // Must be registered before the async handler.
            if (_storageHandler != null)
            {
                builder.RegisterDownstream(_storageHandler);
                builder.RegisterUpstream(_storageHandler);
            }


            if (_workers > 0)
                builder.RegisterDownstream(new AsyncHandler(_workers));

            if (_container != null)
                builder.RegisterDownstream(new Pipeline.IocDispatcher(_container));
            else
                builder.RegisterDownstream(_lastHandler);

            builder.RegisterUpstream(_errrorHandler);

            var pipeline = builder.Build();
            pipeline.Send(new Started());
            var dispatcher = new PipelineDispatcher(pipeline);
            return dispatcher;
        }

        /// <summary>
        /// Add a dispatcher which actually dispatches the commands.
        /// </summary>
        /// <param name="lastHandler">Last handler in the pipeline. Should invoke the correct command handler.</param>
        public PipelineDispatcherBuilder Dispatcher(IDownstreamHandler lastHandler)
        {
            if (lastHandler == null) throw new ArgumentNullException("lastHandler");
            _lastHandler = lastHandler;
            return this;
        }
    }
}