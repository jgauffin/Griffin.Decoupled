using System;
using System.Reflection;
using Griffin.Decoupled.Queries;

namespace Griffin.Decoupled.Container
{
    /// <summary>
    /// Uses Griffin.Container to dispatch the events.
    /// </summary>
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly Griffin.Container.IServiceLocator _serviceLocator;
        private MethodInfo _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDispatcher" /> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <exception cref="System.ArgumentNullException">serviceLocator</exception>
        /// <remarks>Typically registered as:
        /// <code>
        /// <![CDATA[
        /// registrar.RegisterService<IQueryDispatcher>(x => new QueryDispatcher(x), Lifetime.Scoped);
        /// ]]>
        /// </code>
        /// Since that will make it work both with all lifetimes.
        /// </remarks>
        public QueryDispatcher(Griffin.Container.IServiceLocator serviceLocator)
        {
            if (serviceLocator == null) throw new ArgumentNullException("serviceLocator");
            _serviceLocator = serviceLocator;
            _method = GetType().GetMethod("ExecuteInternal", BindingFlags.NonPublic|BindingFlags.Instance);
        }

        /// <summary>
        /// Execute the query
        /// </summary>
        /// <typeparam name="TResult">Expected result</typeparam>
        /// <param name="query">Query to execute</param>
        /// <returns>Result</returns>
        public TResult Execute<TResult>(IQuery<TResult> query)
        {
            var handlerType = typeof(IExecuteQuery<,>)
                .MakeGenericType(query.GetType(), typeof(TResult));


            var method = _method.MakeGenericMethod(query.GetType(), typeof (TResult));
            return (TResult)method.Invoke(this, new object[] {query});

            // couldn't get dynamic to work with my WinForms sample project
            /*
            dynamic handler = _serviceLocator.Resolve(handlerType);
            
            return handler.Execute((dynamic)query);
             * */
        }

        private TResult ExecuteInternal<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>
        {
            var handler = _serviceLocator.Resolve<IExecuteQuery<TQuery, TResult>>();
            return handler.Execute(query);
        }
    }
}
