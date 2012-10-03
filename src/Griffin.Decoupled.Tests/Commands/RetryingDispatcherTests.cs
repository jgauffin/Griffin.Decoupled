using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Griffin.Decoupled.Commands;
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
            var inner = Substitute.For<ICommandDispatcher>();
            var storage = new TestStorage();

            Assert.Throws<ArgumentOutOfRangeException>(() => new RetryingDispatcher(inner, 0, storage));
            Assert.Throws<ArgumentOutOfRangeException>(() => new RetryingDispatcher(inner, 100, storage));
            new RetryingDispatcher(inner, 10, storage);

        }
        [Fact]
        public void FailAll()
        {
            var evt = new ManualResetEvent(false);
            var inner = new BlockingDispatcher(x=> { throw new ExternalException("something extenral fail");});
            var storage = new TestStorage();
            var dispatcher = new RetryingDispatcher(inner, 3, storage);
            dispatcher.CommandFailed += (sender, args) => evt.Set();
            var state = new SendCommand(new FakeCommand()) {Attempts = 2};

            dispatcher.Dispatch(state);

            Assert.True(evt.WaitOne(TimeSpan.FromMilliseconds(500)));
            Assert.Equal(3, state.Attempts);
            Assert.Equal(0, storage.StoredItems.Count());
        }

        [Fact]
        public void FailOne()
        {
            bool failed = false;
            int counter = 0;
            var state = new SendCommand(new FakeCommand());
            var storage = new TestStorage();
            var inner = new BlockingDispatcher(x =>
                {
                    if (counter < 1) throw new ExternalException("something extenral fail");
                    counter++;
                });
            var dispatcher = new RetryingDispatcher(inner, 3, storage);
            dispatcher.CommandFailed += (sender, args) => failed = true;

            dispatcher.Dispatch(new SendCommand(new FakeCommand()));
            
            Assert.Equal(1, storage.StoredItems.Count());
        }
    }
}
