using System;
using System.Collections.Generic;

namespace Griffin.Decoupled.Tests
{
    public interface IServiceLocator
    {
        IEnumerable<T> ResolveAll<T>() where T : class;
        T Resolve<T>() where T : class;
    }

    public interface IRootServiceLocator
    {
        IScopedServiceLocator CreateScope();
    }

    public interface IScopedServiceLocator : IDisposable
    {
    }
}