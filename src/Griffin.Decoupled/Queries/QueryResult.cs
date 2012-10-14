using System;
using System.Collections.Generic;

namespace Griffin.Decoupled.Queries
{
    /// <summary>
    /// Default implementation. See the <see cref="IQueryResult{T}"/> documentation.
    /// </summary>
    /// <typeparam name="T">Returned data type</typeparam>
    public class QueryResult<T> : IQueryResult<T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResult{T}" /> class.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="totalCount">The total count.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public QueryResult(IEnumerable<T> items, int totalCount)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (totalCount < 0) throw new ArgumentOutOfRangeException("totalCount", totalCount, "Must be 0 or larger");

            TotalCount = totalCount;
            Items = items;
        }

        #region IQueryResult<T> Members

        /// <summary>
        /// Gets all matching items
        /// </summary>
        public IEnumerable<T> Items { get; private set; }

        /// <summary>
        /// Gets total number of items (useful when paging is used)
        /// </summary>
        public int TotalCount { get; private set; }

        #endregion
    }
}