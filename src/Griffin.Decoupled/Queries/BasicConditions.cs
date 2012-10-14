using System;
using System.Linq.Expressions;

namespace Griffin.Decoupled.Queries
{
    /// <summary>
    /// Contains basic restrictions like paging and sorting
    /// </summary>
    /// <typeparam name="TResult">Entity type. Used for the sorting</typeparam>
    /// <typeparam name="TYourType">Your own class type</typeparam>
    /// <seealso cref="IQueryResult{T}"/>
    public class BasicConditions<TYourType, TResult> : IQuery<TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicConditions{TYourType,TResult}" /> class.
        /// </summary>
        public BasicConditions()
        {
            PageNumber = -1;
            PageSize = -1;
        }

        /// <summary>
        /// Gets page number (one based index)
        /// </summary>
        /// <remarks>Not used = -1</remarks>
        public int PageNumber { get; private set; }

        /// <summary>
        /// Gets page size
        /// </summary>
        /// <remarks>Not used = -1</remarks>
        public int PageSize { get; private set; }

        /// <summary>
        /// Gets property to sort on.
        /// </summary>
        public string SortPropertyName { get; private set; }

        /// <summary>
        /// Gets sort order
        /// </summary>
        public SortOrder SortOrder { get; protected set; }

        /// <summary>
        /// Use paging
        /// </summary>
        /// <param name="pageNumber">Page to get (one based index).</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <returns>Current instance</returns>
        public void Page(int pageNumber, int pageSize)
        {
            if (pageNumber < 1 || pageNumber > 1000)
                throw new ArgumentOutOfRangeException("pageNumber", "Page number must be between 1 and 1000.");
            if (pageSize < 1 || pageNumber > 1000)
                throw new ArgumentOutOfRangeException("pageSize", "Page size must be between 1 and 1000.");

            PageSize = pageSize;
            PageNumber = pageNumber;
        }

        /// <summary>
        /// Sort ascending by a property
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Current instance</returns>
        public void SortBy(string propertyName)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");
            ValidatePropertyName(propertyName);

            SortOrder = SortOrder.Ascending;
            SortPropertyName = propertyName;
        }

        /// <summary>
        /// Sort descending by a property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Current instance</returns>
        public void SortByDescending(string propertyName)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");
            ValidatePropertyName(propertyName);

            SortOrder = SortOrder.Descending;
            SortPropertyName = propertyName;
        }

        /// <summary>
        /// Property to sort by (ascending)
        /// </summary>
        /// <param name="property">The property.</param>
        public void SortBy(Expression<Func<TYourType, object>> property)
        {
            if (property == null) throw new ArgumentNullException("property");
            var expression = property.GetMemberInfo();
            var name = expression.Member.Name;
            SortBy(name);
        }

        /// <summary>
        /// Property to sort by (descending)
        /// </summary>
        /// <param name="property">The property</param>
        public void SortByDescending(Expression<Func<TYourType, object>> property)
        {
            if (property == null) throw new ArgumentNullException("property");
            var expression = property.GetMemberInfo();
            var name = expression.Member.Name;
            SortByDescending(name);
        }

        /// <summary>
        /// Make sure that the property exists in the model.
        /// </summary>
        /// <param name="name">The name.</param>
        protected virtual void ValidatePropertyName(string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (typeof (TYourType).GetProperty(name) == null)
            {
                throw new ArgumentException(string.Format("'{0}' is not a public property of '{1}'.", name,
                                                          typeof (TYourType).FullName));
            }
        }
    }
}