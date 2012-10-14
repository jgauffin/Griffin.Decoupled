using System;

namespace Griffin.Decoupled.Queries
{
    /// <summary>
    /// Uses an inversion of control container to find and execute the queries.
    /// </summary>
    public class IocQueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceLocator _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="IocQueryDispatcher" /> class.
        /// </summary>
        /// <param name="container">The container. </param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <remarks>Do note that we expect a child/scoped container if one exits. If you can't provide that you'll probably better off
        /// implementing your own IoC support directly by creating a class which inherits <see cref="IQueryDispatcher"/>.</remarks>
        public IocQueryDispatcher(IServiceLocator container)
        {
            if (container == null) throw new ArgumentNullException("container");
            _container = container;
        }

        #region IQueryDispatcher Members

        /// <summary>
        /// Execute the query
        /// </summary>
        /// <typeparam name="TResult">Expected result</typeparam>
        /// <param name="query">Query to execute</param>
        /// <returns>
        /// Result
        /// </returns>
        public TResult Execute<TResult>(IQuery<TResult> query)
        {
            var handlerType = typeof (IExecuteQuery<,>)
                .MakeGenericType(query.GetType(), typeof (TResult));

            dynamic handler = _container.Resolve(handlerType);
            return handler.Handle((dynamic) query);
        }

        #endregion
    }
}