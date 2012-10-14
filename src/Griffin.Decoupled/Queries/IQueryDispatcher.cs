namespace Griffin.Decoupled.Queries
{
    /// <summary>
    /// Takes the specified query, finds the executor and executes the query.
    /// </summary>
    public interface IQueryDispatcher
    {
        /// <summary>
        /// Execute the query
        /// </summary>
        /// <typeparam name="TResult">Expected result</typeparam>
        /// <param name="query">Query to execute</param>
        /// <returns>Result</returns>
        TResult Execute<TResult>(IQuery<TResult> query);
    }
}