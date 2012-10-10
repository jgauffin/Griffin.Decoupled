using System;
using System.Collections.Concurrent;
using System.Threading;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.Commands.Pipeline
{
    /// <summary>
    /// Will enqueue messages and send them asynchronously
    /// </summary>
    /// <remarks>All queued messages are queued i memory. Thus you need to store them somewhere (using for instance <see cref="StorageHandler"/>) if you do not want to risk to loosing them on shutdown or application crash.</remarks>
    internal class AsyncHandler : IDownstreamHandler
    {
        private readonly ManualResetEventSlim _closingEvent = new ManualResetEventSlim(false);
        private readonly ConcurrentQueue<DispatchCommand> _commands = new ConcurrentQueue<DispatchCommand>();
        private readonly int _maxConcurrentTasks;
        private bool _closing;
        private IDownstreamContext _context;
        private long _currentWorkers;


        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncHandler" /> class.
        /// </summary>
        /// <param name="maxConcurrentTasks">Maximum number of concurrent tasks.</param>
        public AsyncHandler(int maxConcurrentTasks)
        {
            if (maxConcurrentTasks < 1 || maxConcurrentTasks > 100)
                throw new ArgumentOutOfRangeException("maxConcurrentTasks", maxConcurrentTasks,
                                                      "1 to 100 is somewhat reasonable.");

            _maxConcurrentTasks = maxConcurrentTasks;
        }

        #region IDownstreamHandler Members

        public void HandleDownstream(IDownstreamContext context, object message)
        {
            _context = context;

            var sendCmd = message as DispatchCommand;
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
            DispatchCommand command;
            if (!_commands.TryDequeue(out command))
                return false;

            try
            {
                _context.SendDownstream(command);
            }
            catch (Exception err)
            {
                _context.SendUpstream(new PipelineFailure(this, command, "AsyncHandler failed to dispatch commands.",
                                                          err));
            }
            return true;
        }


        private void EnqueueCommand(DispatchCommand dispatchCmd)
        {
            _commands.Enqueue(dispatchCmd);

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