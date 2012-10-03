using System;
using System.Threading;
using Griffin.Decoupled.Commands.Pipeline.Messages;

namespace Griffin.Decoupled.Commands.Pipeline
{
    /// <summary>
    /// Will enqueue messages and send them asynchronously
    /// </summary>
    internal class AsyncHandler : IDownstreamHandler
    {
        private readonly ManualResetEventSlim _closingEvent = new ManualResetEventSlim(false);
        private readonly ICommandDispatcher _inner;
        private readonly int _maxConcurrentTasks;
        private readonly ICommandStorage _storage;
        private bool _closing;
        private IDownstreamContext _context;
        private long _currentWorkers;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommandDispatcher" /> class.
        /// </summary>
        /// <param name="storage">Where we should store commands.</param>
        /// <param name="maxConcurrentTasks">Maximum number of concurrent tasks.</param>
        public AsyncHandler(ICommandStorage storage, int maxConcurrentTasks)
        {
            if (storage == null) throw new ArgumentNullException("storage");
            if (maxConcurrentTasks < 1 || maxConcurrentTasks > 100)
                throw new ArgumentOutOfRangeException("maxConcurrentTasks", maxConcurrentTasks,
                                                      "1 to 100 is somewhat reasonable.");

            _storage = storage;
            _maxConcurrentTasks = maxConcurrentTasks;
        }

        #region IDownstreamHandler Members

        public void HandleDownstream(IDownstreamContext context, object message)
        {
            _context = context;

            var sendCmd = message as SendCommand;
            if (sendCmd != null)
            {
                Sendcommand(sendCmd);
            }
            if (message is Shutdown)
            {
                Close();
            }
        }

        #endregion

        /// <summary>
        /// Close the dispatcher gracefully.
        /// </summary>
        /// <remarks>Should make sure that all non-persitent commands are executed or stored before exeting.</remarks>
        public void Close()
        {
            _closing = true;
            _closingEvent.Wait(TimeSpan.FromSeconds(10));
        }

        /// <summary>
        /// We should start dispatching commands.
        /// </summary>
        /// <param name="state"></param>
        private void DispatchCommandNow(object state)
        {
            try
            {
                while (DispatchCommand())
                {
                    
                }
            }
            catch (Exception err)
            {
                _context.SendUpstream(new PipelineError("AsyncHandler failed to dispatch commands.", err));
            }
            finally
            {
                Interlocked.Decrement(ref _currentWorkers);
            }
        }

        /// <summary>
        /// Dispatch a single command
        /// </summary>
        private bool DispatchCommand()
        {
            var transactional = _storage as ITransactionalCommandStorage;
            if (transactional != null)
            {
                using (var transaction = transactional.BeginTransaction())
                {
                    var command2 = _storage.Dequeue();
                    if (command2== null)
                    {
                        transaction.Commit();
                        return false;
                    }

                    _inner.Dispatch(command2);
                    transaction.Commit();
                }
            }
            else
            {
                var command2 = _storage.Dequeue();
                if (command2 == null)
                    return false;

                _inner.Dispatch(command2);
            }

            return true;
        }


        private void Sendcommand(SendCommand sendCmd)
        {
            _storage.Enqueue(sendCmd);

            if (_closing)
                return;

            // Not very much thread safe.
            if (Interlocked.Read(ref _currentWorkers) < _maxConcurrentTasks)
            {
                Interlocked.Increment(ref _currentWorkers);
                ThreadPool.QueueUserWorkItem(DispatchCommandNow);
            }
        }
    }
}