using System;
using System.Threading;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.Commands.Pipeline
{
    /// <summary>
    /// Will enqueue messages and send them asynchronously
    /// </summary>
    /// <remarks>It's recommended that the data storage implements the <see cref="ITransactionalCommandStorage"/> interface
    /// so that we can use transactions during command invocation. It helps us to keep the commands in the database
    /// if something fails or the application is crashing.</remarks>
    internal class AsyncHandler : IDownstreamHandler
    {
        private readonly ManualResetEventSlim _closingEvent = new ManualResetEventSlim(false);
        private readonly int _maxConcurrentTasks;
        private readonly ICommandStorage _storage;
        private bool _closing;
        private IDownstreamContext _context;
        private long _currentWorkers;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncHandler" /> class.
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
                EnqueueCommand(sendCmd);
                return;
            }
            if (message is Shutdown)
            {
                Close();
            }
            if (message is Started)
            {
                StartWorker();
            }

            context.SendDownstream(message);
        }

        #endregion

        /// <summary>
        /// Close the dispatcher gracefully.
        /// </summary>
        /// <remarks>Should make sure that all non-persitent commands are executed or stored before exeting.</remarks>
        public void Close()
        {
            _closing = true;
            if (Interlocked.Read(ref _currentWorkers) > 0)
                _closingEvent.Wait(TimeSpan.FromSeconds(10));
        }

        /// <summary>
        /// Dispatch all available commands.
        /// </summary>
        private void DispatchCommands()
        {
            try
            {
                while (DispatchCommand())
                {
                    // nothing in here,
                    // we just want to dispach all messages
                }
            }
            catch (Exception err)
            {
                _context.SendUpstream(new PipelineFailure(this, "AsyncHandler failed to dispatch commands.", err));
            }
            finally
            {
                Interlocked.Decrement(ref _currentWorkers);
                if (_currentWorkers == 0 && _closing)
                    _closingEvent.Set();
            }
        }

        /// <summary>
        /// Dispatch a single command
        /// </summary>
        private bool DispatchCommand()
        {
            var command = _storage.Dequeue();
            if (command == null)
                return false;

            _context.SendDownstream(command);
            _storage.Delete(command.Command);
            return true;
        }


        private void EnqueueCommand(SendCommand sendCmd)
        {
            _storage.Add(sendCmd);

            if (_closing)
                return;

            StartWorker();
        }

        private void StartWorker()
        {
            // Not very much thread safe.
            if (Interlocked.Read(ref _currentWorkers) < _maxConcurrentTasks)
            {
                Interlocked.Increment(ref _currentWorkers);
                ThreadPool.QueueUserWorkItem(x => DispatchCommands());
            }
            else
            {
                Console.WriteLine("Can't max workers... :(");
            }
        }
    }
}