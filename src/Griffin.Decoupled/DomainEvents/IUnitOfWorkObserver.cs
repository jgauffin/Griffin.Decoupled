namespace Griffin.Decoupled.DomainEvents
{
    /// <summary>
    /// The observer is used to hold of domain events if there are a current unit of work implementation running.
    /// </summary>
    /// <remarks>
    /// <para>This is done to allow us to not dispatch domain events until everything (i.e. the domain enteties which generated the events) have been saved successfully to the database. 
    /// The events wouldn't be valid otherwise.</para>
    /// </remarks>
    public interface IUnitOfWorkObserver
    {
        /// <summary>
        /// A UoW has been created for the current thread.
        /// </summary>
        /// <param name="unitOfWork">The unit of work that was created.</param>
        void Create(object unitOfWork);

        /// <summary>
        /// A UoW has been released for the current thread.
        /// </summary>
        /// <param name="unitOfWork">UoW which was released. Must be same as in <see cref="Create"/>.</param>
        /// <param name="successful"><c>true</c> if the UoW was saved successfully; otherwise <c>false</c>.</param>
        void Released(object unitOfWork, bool successful);
    }
}