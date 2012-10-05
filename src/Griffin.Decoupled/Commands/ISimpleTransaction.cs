using System;

namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// The transaction adapter.
    /// </summary>
    /// <remarks>Should rollback on disposal if <c>Commit()</c> has not been called.</remarks>
    public interface ISimpleTransaction : IDisposable
    {
        /// <summary>
        /// Commit transaction
        /// </summary>
        void Commit();
    }
}