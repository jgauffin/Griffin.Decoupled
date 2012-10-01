using System;
using System.Threading;

namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// Will queue & store all commands and then invoke them in turn.
    /// </summary>
    public class AsyncCommandDispatcher : ICommandDispatcher
    {
        private readonly ManualResetEventSlim _closingEvent = new ManualResetEventSlim(false);
        private readonly ICommandDispatcher _inner;
        private readonly int _maxConcurrentTasks;
        private readonly ICommandStorage _storage;
        private bool _closing;
        private long _currentWorkers;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommandDispatcher" /> class.
        /// </summary>
        /// <param name="inner">Inner dispatcher.</param>
        /// <param name="storage">Where we should store commands.</param>
        /// <param name="maxConcurrentTasks">Maximum number of concurrent tasks.</param>
        public AsyncCommandDispatcher(ICommandDispatcher inner, ICommandStorage storage, int maxConcurrentTasks)
        {
            if (inner == null) throw new ArgumentNullException("inner");
            if (storage == null) throw new ArgumentNullException("storage");
            if (maxConcurrentTasks < 1 || maxConcurrentTasks > 100)
                throw new ArgumentOutOfRangeException("maxConcurrentTasks", maxConcurrentTasks, "1 to 100 is somewhat reasonable.");

            _inner = inner;
            _storage = storage;
            _maxConcurrentTasks = maxConcurrentTasks;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommandDispatcher" /> class.
        /// </summary>
        /// <param name="inner">Inner dispatcher.</param>
        /// <param name="maxConcurrentTasks">Maximum number of concurrent tasks.</param>
        /// <remarks>Uses the memory for storage</remarks>
        public AsyncCommandDispatcher(ICommandDispatcher inner, int maxConcurrentTasks)
        {
            if (inner == null) throw new ArgumentNullException("inner");
            if (maxConcurrentTasks < 1 || maxConcurrentTasks > 100)
                throw new ArgumentOutOfRangeException("maxConcurrentTasks", maxConcurrentTasks, "1 to 100 is somewhat reasonable.");

            _inner = inner;
            _storage = new MemoryStorage();
            _maxConcurrentTasks = maxConcurrentTasks;
        }


        #region ICommandDispatcher Members

        /// <summary>
        /// Dispatch the command to the handler
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <remarks>Implementations should throw exceptions unless they are asynchronous or will attempt to retry later.</remarks>
        public void Dispatch(CommandState command)
        {
            _storage.Enqueue(command);

            if (_closing)
                return;

            // Not very much thread safe.
            if (Interlocked.Read(ref _currentWorkers) < _maxConcurrentTasks)
            {
                Interlocked.Increment(ref _currentWorkers);
                ThreadPool.QueueUserWorkItem(DispatchCommandNow);
            }
        }

        /// <summary>
        /// Close the dispatcher gracefully.
        /// </summary>
        /// <remarks>Should make sure that all non-persitent commands are executed or stored before exeting.</remarks>
        public void Close()
        {
            _closing = true;
            _closingEvent.Wait(TimeSpan.FromSeconds(10));
        }

        #endregion

        /// <summary>
        /// We should start dispatching commands.
        /// </summary>
        /// <param name="state"></param>
        private void DispatchCommandNow(object state)
        {
            try
            {
                DispatchCommands();
            }
            catch(Exception err)
            {
                UncaughtException(this, new AsyncDispatcherExceptionEventArgs(err));
            }
            finally
            {
                Interlocked.Decrement(ref _currentWorkers);
            }
        }

        /// <summary>
        /// Dispatch as many commands as possible.
        /// </summary>
        private void DispatchCommands()
        {
            var command = _storage.Dequeue();
            while (command != null)
            {
                _inner.Dispatch(command);
                command = _storage.Dequeue();
            }
        }

        /// <summary>
        /// One of the worker tasks caught an exception.
        /// </summary>
        /// <remarks>Shouldn't really be possible since it only happens if the inner dispatcher screws up.</remarks>
        public event EventHandler<AsyncDispatcherExceptionEventArgs> UncaughtException = delegate { };
    }
}