using System;

namespace Griffin.Decoupled
{
    /// <summary>
    /// A scoped inversion of control container
    /// </summary>
    /// <remarks>It's purpose is to be able to store and return scoped services.</remarks>
    public interface IScopedContainer : IDisposable, IServiceLocator
    {
    }
}