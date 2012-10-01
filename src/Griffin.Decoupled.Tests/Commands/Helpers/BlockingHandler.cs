﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Griffin.Decoupled.Commands;

namespace Griffin.Decoupled.Tests.Commands.Helpers
{
    class BlockingHandler<T> : IHandleCommand<T> where T : class, ICommand
    {
        ManualResetEventSlim _event = new ManualResetEventSlim(false);

        /// <summary>
        /// Invoke the command
        /// </summary>
        /// <param name="command">Command to run</param>
        public void Invoke(T command)
        {
            _event.Set();
        }

        public void Reset()
        {
            _event.Reset();
        }

        public bool Wait(TimeSpan span)
        {
            return _event.Wait(span);
        }
    }
}