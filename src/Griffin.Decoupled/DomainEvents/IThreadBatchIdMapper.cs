using System;

namespace Griffin.Decoupled.DomainEvents
{
    /// <summary>
    /// Used to map each UoW to a batch id and keep track of all UoWs that exists for a thread.
    /// </summary>
    public interface IThreadBatchIdMapper
    {
        /// <summary>
        /// Gets if the current thread has an active UoW
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Create a new thread/uow mapping
        /// </summary>
        /// <param name="unitOfWork">Used UoW</param>
        /// <returns>Guid associated with the UoW</returns>
        Guid Create(object unitOfWork);

        /// <summary>
        /// A UoW has been rolledback/comitted
        /// </summary>
        /// <param name="unitOfWork">Uow which was previously specified in the <see cref="Create"/> method</param>
        /// <returns></returns>
        Guid Release(object unitOfWork);

        /// <summary>
        /// Get latest batch id for the current thread.
        /// </summary>
        /// <returns>Guid if a UoW is active on the current thread; otherwise <see cref="Guid.Empty"/>.</returns>
        Guid GetBatchId();
    }
}