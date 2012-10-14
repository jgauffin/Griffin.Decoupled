using System.Collections.Generic;

namespace Griffin.Decoupled.Queries
{
    /// <summary>
    /// Represents a result from a query.
    /// </summary>
    /// <typeparam name="T">Type of return model</typeparam>
    /// <remarks>Since we use paging for most of our queries we'll have to return a result which contains the total amount of items.</remarks>
    /// <seealso cref="BasicConditions{TYourType,TResult}"/>
    public interface IQueryResult<out T> where T : class
    {
        /// <summary>
        /// Gets all matching items
        /// </summary>
        IEnumerable<T> Items { get; }

        /// <summary>
        /// Gets total number of items (useful when paging is used)
        /// </summary>
        int TotalCount { get; }
    }
}