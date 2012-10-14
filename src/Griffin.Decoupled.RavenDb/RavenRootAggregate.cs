﻿using System;
using Raven.Client;

namespace Griffin.Decoupled.RavenDb
{
    /// <summary>
    /// Base class for all RavenDB root aggregates
    /// </summary>
    /// <typeparam name="TEntity">Type of enitity/Model</typeparam>
    public class RavenRootAggregate<TEntity> : IRootAggregate<TEntity, string> where TEntity : class
    {
        private readonly IDocumentSession _session;

        /// <summary>
        /// Initializes a new instance of the <see cref="RavenRootAggregate{TEntity}" /> class.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public RavenRootAggregate(IDocumentSession session)
        {
            if (session == null) throw new ArgumentNullException("session");

            _session = session;
        }

        #region IRootAggregate<TEntity,string> Members

        /// <summary>
        /// Load the entity from the database.
        /// </summary>
        /// <param name="key">Primary key</param>
        /// <returns>Entity if found; otherwise <c>null</c>.</returns>
        public TEntity Load(string key)
        {
            if (key == null) throw new ArgumentNullException("key");

            return _session.Load<TEntity>(key);
        }

        /// <summary>
        /// Create or update.
        /// </summary>
        /// <param name="entity">Entity being saved.</param>
        public void Save(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            _session.Store(entity);
        }

        /// <summary>
        /// Delete the entity with the specified key
        /// </summary>
        /// <param name="key">Key of the item to delete</param>
        public void Delete(string key)
        {
            if (key == null) throw new ArgumentNullException("key");

            var item = Load(key);
            if (item != null)
                Delete(item);

        }

        /// <summary>
        /// Delete the entity
        /// </summary>
        /// <param name="entity">Entity to delete</param>
        public void Delete(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            _session.Delete(entity);
        }

        #endregion
    }
}