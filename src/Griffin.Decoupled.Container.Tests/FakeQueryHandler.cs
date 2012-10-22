using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Container;
using Griffin.Decoupled.Queries;

namespace Griffin.Decoupled.Container.Tests
{
    public class FakeQueryHandler : IExecuteQuery<FakeQuery, string>
    {
        /// <summary>
        /// Invoke the query
        /// </summary>
        /// <param name="query">Query to execute</param>
        /// <returns>Result from query. </returns>
        /// <remarks>Collection queries should return an empty result if there aren't any matches. Queries for a single object should return <c>null</c>.</remarks>
        public string Execute(FakeQuery query)
        {
            return null;
        }
    }
}
