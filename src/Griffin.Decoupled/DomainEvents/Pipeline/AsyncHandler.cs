using System;
using System.Collections.Concurrent;
using System.Threading;
using Griffin.Decoupled.DomainEvents.Pipeline.Messages;
using Griffin.Decoupled.Pipeline;
using Griffin.Decoupled.Pipeline.Messages;

namespace Griffin.Decoupled.DomainEvents.Pipeline
{
    /// <summary>
    /// Will dispatch events asynchronously
    /// </summary>
    public class AsyncHandler : IDownstreamHandler
    {
        private readonly int _maxWorkers = 5;
        private readonly ConcurrentQueue<DispatchEvent> _queue = new ConcurrentQueue<DispatchEvent>();
        private readonly ManualResetEventSlim _shutdownEvent = new ManualResetEventSlim();
        private IDownstreamContext _context;
        private long _currentWorkers;
        private bool _shutDown;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncHandler" /> class.
        /// </summary>
        /// <param name="maxWorkers">The maximum number of domain events which can be dispatched simulteneously.</param>
        public AsyncHandler(int maxWorkers)
        {
            if (maxWorkers < 1 || maxWorkers > 10)
                throw new ArgumentOutOfRangeException("maxWorkers", maxWorkers,
                                                      "1-10 tasks are acceptable. More threads wont make your app run any faster.");

            _maxWorkers = maxWorkers;
        }

        #region IDownstreamHandler Members

        /// <summary>
        /// Send a message to the command handler
        /// </summary>
        /// <param name="context">my context</param>
        /// <param name="message">Message to send, typically <see cref="DispatchEvent"/>.</param>
        public void HandleDownstream(IDownstreamContext context, IDownstreamMessage message)
        {
            _context = context;
            var msg = message as DispatchEvent;
            if (msg != null)
            {
                _queue.Enqueue(msg);

                // Not very much thread safe.
                if (Interlocked.Read(ref _currentWorkers) < _maxWorkers)
                {
                    Interlocked.Increment(ref _currentWorkers);
                    ThreadPool.QueueUserWorkItem(DispatchEventsNow);
                }
            }
            else if (message is Shutdown)
            {
                Close();
            }
        }

        #endregion

        /// <summary>
        /// Close the dispatcher gracefully.
        /// </summary>
        /// <remarks>Should make sure that all events are propagated before returning.</remarks>
        public void Close()
        {
            if (Interlocked.Read(ref _currentWorkers) > 0)
            {
                _shutDown = true;
                if (!_shutdownEvent.Wait(TimeSpan.FromSeconds(5)))
                    throw new InvalidOperationException("Failed to wait on all events before shutting down.");
            }
        }


        private void DispatchEventsNow(object state)
        {
            try
            {
                while (DispatchEventNow())
                {
                }
            }
            finally
            {
                Interlocked.Decrement(ref _currentWorkers);
                if (_currentWorkers == 0 && _shutDown)
                    _shutdownEvent.Set();
            }
        }

        private bool DispatchEventNow()
        {
            DispatchEvent theEvent;
            if (_queue.TryDequeue(out theEvent))
            {
                _context.SendDownstream(theEvent);
                return true;
            }

            return false;
        }
    }
}