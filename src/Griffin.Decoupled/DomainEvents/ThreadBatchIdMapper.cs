using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Griffin.Decoupled.DomainEvents
{
    /// <summary>
    /// Used to map UnitOfWork objects to Guids which is easier to persist.
    /// </summary>
    public class ThreadBatchIdMapper : IThreadBatchIdMapper
    {
        // no need for locks since every thread can only check it's own id.
        private readonly Dictionary<int, ICollection<UowGuidMapping>> _threadMappings =
            new Dictionary<int, ICollection<UowGuidMapping>>();

        #region IThreadBatchIdMapper Members

        /// <summary>
        /// Gets if the current thread has an active UoW
        /// </summary>
        public bool IsActive
        {
            get
            {
                ICollection<UowGuidMapping> mappings;
                return _threadMappings.TryGetValue(Thread.CurrentThread.ManagedThreadId, out mappings) && mappings.Any();
            }
        }

        /// <summary>
        /// Create a new thread/uow mapping
        /// </summary>
        /// <param name="unitOfWork">Used UoW</param>
        /// <returns>Guid associated with the UoW</returns>
        public Guid Create(object unitOfWork)
        {
            if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");

            ICollection<UowGuidMapping> mappings;
            if (!_threadMappings.TryGetValue(Thread.CurrentThread.ManagedThreadId, out mappings))
            {
                mappings = new List<UowGuidMapping>();
                _threadMappings[Thread.CurrentThread.ManagedThreadId] = mappings;
            }


            var guid = Guid.NewGuid();
            mappings.Add(new UowGuidMapping(unitOfWork, guid));
            return guid;
        }

        /// <summary>
        /// A UoW has been rolledback/comitted
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        public Guid Release(object unitOfWork)
        {
            if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");

            ICollection<UowGuidMapping> mappings;
            if (!_threadMappings.TryGetValue(Thread.CurrentThread.ManagedThreadId, out mappings))
                throw new InvalidOperationException(
                    string.Format("You have not registered unitOfWork '{0}' with hashCode '{1}'.", unitOfWork,
                                  unitOfWork.GetHashCode()));

            var batchId = Guid.Empty;
            foreach (var mapping in mappings.Where(mapping => mapping.UoW == unitOfWork))
            {
                batchId = mapping.Guid;
                mappings.Remove(mapping);
                break;
            }
            if (batchId == Guid.Empty)
                throw new InvalidOperationException(
                    string.Format("You have not registered unitOfWork '{0}' with hashCode '{1}'.", unitOfWork,
                                  unitOfWork.GetHashCode()));

            return batchId;
        }

        /// <summary>
        /// Get latest batch id for the current thread.
        /// </summary>
        /// <returns></returns>
        public Guid GetBatchId()
        {
            ICollection<UowGuidMapping> mappings;
            if (!_threadMappings.TryGetValue(Thread.CurrentThread.ManagedThreadId, out mappings))
            {
                return Guid.Empty;
            }

            var lastMapping = mappings.LastOrDefault();
            return lastMapping == null ? Guid.Empty : lastMapping.Guid;
        }

        #endregion

        #region Nested type: UowGuidMapping

        private class UowGuidMapping
        {
            public UowGuidMapping(object unitOfWork, Guid guid)
            {
                UoW = unitOfWork;
                Guid = guid;
            }

            public Guid Guid { get; private set; }
            public object UoW { get; private set; }
        }

        #endregion
    }
}