using System;
using System.Reflection;
using System.Threading;

namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// Will queue & store all commands and then invoke them in turn.
    /// </summary>
    public class AsyncDispatcher : ICommandDispatcher
    {
        private readonly ManualResetEventSlim _closingEvent = new ManualResetEventSlim(false);
        private readonly ICommandDispatcher _inner;
        private readonly MethodInfo _invokeMethod;
        private readonly int _maxConcurrentTasks;
        private readonly ICommandStorage _storage;
        private bool _closing;
        private long _currentWorkers;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncDispatcher" /> class.
        /// </summary>
        /// <param name="inner">Inner dispatcher.</param>
        /// <param name="storage">Where we should store commands.</param>
        /// <param name="maxConcurrentTasks">Maximum number of concurrent tasks.</param>
        public AsyncDispatcher(ICommandDispatcher inner, ICommandStorage storage, int maxConcurrentTasks)
        {
            if (inner == null) throw new ArgumentNullException("inner");
            if (storage == null) throw new ArgumentNullException("storage");

            _inner = inner;
            _storage = storage;
            _maxConcurrentTasks = maxConcurrentTasks;
            _invokeMethod = GetType().GetMethod("DispatchToInner", BindingFlags.NonPublic | BindingFlags.Instance);
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
            _storage.Enqueue(new StoredCommand {Command = command});

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

        private void DispatchCommandNow(object state)
        {
            var command = _storage.Dequeue();
            if (command != null)
            {
                _invokeMethod.MakeGenericMethod(command.GetType()).Invoke(this, new object[] {command});
            }
        }

        /// <summary>
        /// Invoked through reflection
        /// </summary>
        /// <typeparam name="T">Type of command</typeparam>
        /// <param name="command">Command to execute</param>
        private void DispatchToInner<T>(T command) where T : class, ICommand
        {
            if (command == null) throw new ArgumentNullException("command");
            _inner.Dispatch(command);
        }
    }
}