using Griffin.Container;
using Griffin.Decoupled.Queries;
using NSubstitute;
using Xunit;

namespace Griffin.Decoupled.Container.Tests
{
    public class QueryDispatcherTests
    {
        [Fact]
        public void Dispatch()
        {
            var serviceLocator = Substitute.For<Griffin.Container.IServiceLocator>();
            var handler = Substitute.For<IExecuteQuery<FakeQuery, string>>();
            serviceLocator.Resolve(typeof(IExecuteQuery<FakeQuery, string>)).Returns(handler);

            var dispatcher = new QueryDispatcher(serviceLocator);
            dispatcher.Execute(new FakeQuery());

        }

        [Fact]
        public void DispatchReal()
        {
            var handler = Substitute.For<IExecuteQuery<FakeQuery, string>>();
            var registrar = new ContainerRegistrar();
            registrar.RegisterInstance(handler);

            var dispatcher = new QueryDispatcher(registrar.Build());
            dispatcher.Execute(new FakeQuery());

        }
    }
}