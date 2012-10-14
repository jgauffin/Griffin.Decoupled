using System;
using System.Linq;
using System.Linq.Expressions;

namespace Griffin.Decoupled.Queries
{
    /// <summary>
    /// Extensions for <see cref="BasicConditions{TYourType,TResult}"/>
    /// </summary>
    public static class BasicConditionsExtensions
    {
        /// <summary>
        /// Apply the query information to a LINQ statement 
        /// </summary>
        /// <typeparam name="TEntity">Model/aggregate type</typeparam>
        /// <typeparam name="TResult">Kind of result to return</typeparam>
        /// <param name="instance">contraints instance</param>
        /// <param name="query">LINQ queryable</param>
        /// <returns>Modified query</returns>
        public static IQueryable<TEntity> ApplyTo<TEntity, TResult>(
            this BasicConditions<TEntity, TResult> instance, IQueryable<TEntity> query) where TEntity : class
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (query == null) throw new ArgumentNullException("query");

            if (!string.IsNullOrEmpty(instance.SortPropertyName))
            {
                query = instance.SortOrder == SortOrder.Ascending
                            ? query.OrderBy(instance.SortPropertyName)
                            : query.OrderByDescending(instance.SortPropertyName);
            }

            if (instance.PageNumber >= 1)
            {
                query = query.Skip((instance.PageNumber - 1)*instance.PageSize).Take(instance.PageSize);
            }

            return query;
        }

        /// <summary>
        /// Execute LINQ query and fill a result.
        /// </summary>
        /// <typeparam name="TEntity">Model/aggregate type</typeparam>
        /// <typeparam name="TResult">Kind of result to return</typeparam>
        /// <param name="instanz">The instanz.</param>
        /// <param name="constraints">The constraints.</param>
        /// <returns>Search reuslt</returns>
        public static IQueryResult<TEntity> ToSearchResult<TEntity, TResult>(this IQueryable<TEntity> instanz,
                                                                             BasicConditions<TEntity, TResult>
                                                                                 constraints)
            where TEntity : class
        {
            if (instanz == null) throw new ArgumentNullException("instanz");
            if (constraints == null) throw new ArgumentNullException("constraints");

            var totalCount = instanz.Count();
            var limitedQuery = constraints.ApplyTo(instanz);
            return new QueryResult<TEntity>(limitedQuery, totalCount);
        }

        /// <summary>
        /// Apply ordering to a LINQ query
        /// </summary>
        /// <typeparam name="T">Model type</typeparam>
        /// <param name="source">Linq query</param>
        /// <param name="propertyName">Property to sort by</param>
        /// <param name="values">DUNNO?</param>
        /// <returns>Ordered query</returns>
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName, params object[] values)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            var type = typeof (T);
            var property = type.GetProperty(propertyName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            var resultExp = Expression.Call(typeof (Queryable), "OrderBy", new[] {type, property.PropertyType},
                                            source.Expression, Expression.Quote(orderByExp));
            return source.Provider.CreateQuery<T>(resultExp);
        }

        /// <summary>
        /// Apply ordering to a LINQ query
        /// </summary>
        /// <typeparam name="T">Model type</typeparam>
        /// <param name="source">Linq query</param>
        /// <param name="propertyName">Property to sort by</param>
        /// <param name="values">DUNNO?</param>
        /// <returns>Ordered query</returns>
        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName,
                                                         params object[] values)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            var type = typeof (T);
            var property = type.GetProperty(propertyName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            var resultExp = Expression.Call(typeof (Queryable), "OrderByDescending", new[] {type, property.PropertyType},
                                            source.Expression, Expression.Quote(orderByExp));
            return source.Provider.CreateQuery<T>(resultExp);
        }
    }
}