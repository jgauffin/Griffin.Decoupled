using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Decoupled.Queries;
using NSubstitute;

namespace Griffin.Decoupled.Tests.Queries
{
    public class IocDispatcherTests
    {
        public void Dispatcher()
        {
            var serviceLocator = Substitute.For<IServiceLocator>();

            var  dispatcher = new IocQueryDispatcher(serviceLocator);
        }
    }
}
