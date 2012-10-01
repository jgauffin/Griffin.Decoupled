using System;

namespace Griffin.Decoupled.RavenDb
{
    /// <summary>
    /// Unit of work abstraction
    /// </summary>
    /// <remarks>Roll back if being disposed and <c>SaveChanges()</c> have not been invoked.</remarks>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Commit changes
        /// </summary>
        void SaveChanges();
    }
}