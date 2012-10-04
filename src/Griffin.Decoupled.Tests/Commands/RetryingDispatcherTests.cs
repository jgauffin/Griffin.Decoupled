using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Griffin.Decoupled.Commands;
using Griffin.Decoupled.Commands.Pipeline;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Griffin.Decoupled.Tests.Commands.Helpers;
using NSubstitute;
using Xunit;

namespace Griffin.Decoupled.Tests.Commands
{
    public class RetryingDispatcherTests
    {
        [Fact]
        public void InvalidAttemptCount()
        {
            var storage = new TestStorage();

            Assert.Throws<ArgumentOutOfRangeException>(() => new RetryingHandler(0, storage));
            Assert.Throws<ArgumentOutOfRangeException>(() => new RetryingHandler(100, storage));
            new RetryingHandler(10, storage);

        }
        [Fact]
        public void FailAll()
        {
            var evt = new ManualResetEvent(false);
            var storage = new TestStorage();
            var dispatcher = new RetryingHandler(3, storage);
            var context = new DownContext(x => { throw new Exception("Constant failure"); }, x => { if (x is CommandAborted) evt.Set(); });
            var state = new SendCommand(new FakeCommand()) { Attempts = 2 };

            dispatcher.HandleDownstream(context, state);

            Assert.True(evt.WaitOne(TimeSpan.FromMilliseconds(500)));
            Assert.Equal(3, state.Attempts);
            Assert.Equal(0, storage.StoredItems.Count());
        }

        [Fact]
        public void FailOne()
        {
            int counter = 0;
            bool gotFailed = false;
            bool gotAbort = false;
            var context = new DownContext(x =>
                {
                    if (counter < 1) throw new ExternalException("something extenral fail");
                    counter++;
                }, x =>
                    {
                        gotFailed = x is CommandFailed;
                        if (x is CommandAborted)
                            gotAbort = true;
                    });
            var storage = new TestStorage();
            var dispatcher = new RetryingHandler(3, storage);

            dispatcher.HandleDownstream(context, new SendCommand(new FakeCommand()));

            Assert.Equal(1, storage.StoredItems.Count());
            Assert.True(gotFailed);
            Assert.False(gotAbort);
        }
    }
}
