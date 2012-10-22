namespace Griffin.Decoupled.Queries
{
    /// <summary>
    /// Takes the specified query, finds the executor and executes the query.
    /// </summary>
    public interface IQueryDispatcher
    {
        /// <summary>
        /// Dispatch the query to the correct executer (and invoke the executer)
        /// </summary>
        /// <typeparam name="TResult">Expected result</typeparam>
        /// <param name="query">Query to execute</param>
        /// <returns>Result</returns>
        TResult Execute<TResult>(IQuery<TResult> query);
    }
}