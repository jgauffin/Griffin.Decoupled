namespace Griffin.Decoupled
{
    /// <summary>
    /// I, Robot anyone?
    /// </summary>
    /// <typeparam name="TEntity">Your entity/model type (for instance <c>User</c>)</typeparam>
    /// <typeparam name="TKey">Type of the primary key (like <c>string</c> or <c>int</c>)</typeparam>
    /// <remarks><para>Used to abstract away the work with root aggregates from the DB. The purpose is to simplify unit tests
    /// and make it easier to spot what each command is using form the data source.
    /// </para>
    /// <para>Please do not use this interface directly, but
    /// do instead create more specialized interfaces for your root aggregates. You might for instance need a <c>LoadByUserName</c> method.
    /// </para>
    /// <para>There is really no need to use this interface if you are using a schemaless DB with in-memory support like RavenDB. Then just
    /// whip up a new DB and use it in your unit tests.</para>
    /// </remarks>
    public interface IRootAggregate<TEntity, in TKey> where TEntity : class
    {
        /// <summary>
        /// Load the entity from the database.
        /// </summary>
        /// <param name="key">Primary key</param>
        /// <returns>Entity if found; otherwise <c>null</c>.</returns>
        TEntity Load(TKey key);

        /// <summary>
        /// Create or update.
        /// </summary>
        /// <param name="entity">Entity being saved.</param>
        void Save(TEntity entity);

        /// <summary>
        /// Delete the entity with the specified key
        /// </summary>
        /// <param name="key">Key of the item to delete</param>
        void Delete(TKey key);

        /// <summary>
        /// Delete the entity
        /// </summary>
        /// <param name="entity">Entity to delete</param>
        void Delete(TEntity entity);
    }
}