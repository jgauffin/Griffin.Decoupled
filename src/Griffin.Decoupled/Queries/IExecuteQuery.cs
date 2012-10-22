namespace Griffin.Decoupled.Queries
{
    /// <summary>
    /// Interface for the class that will execute the query (i.e. generate the result)
    /// </summary>
    /// <typeparam name="TQuery">Query to execute</typeparam>
    /// <typeparam name="TResult">Result to return</typeparam>
    public interface IExecuteQuery<in TQuery, out TResult>
        where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Invoke the query
        /// </summary>
        /// <param name="query">Query to execute</param>
        /// <returns>Result from query. </returns>
        /// <remarks>Collection queries should return an empty result if there aren't any matches. Queries for a single object should return <c>null</c>.</remarks>
        TResult Execute(TQuery query);
    }
}