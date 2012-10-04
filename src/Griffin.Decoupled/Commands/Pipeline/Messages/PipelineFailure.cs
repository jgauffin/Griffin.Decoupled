using System;

namespace Griffin.Decoupled.Commands.Pipeline.Messages
{
    /// <summary>
    /// The pipeline itself (one of the handlers) have failed.
    /// </summary>
    public class PipelineFailure
    {
        public PipelineFailure(object handler, string errorMsg, Exception exception)
        {
            Handler = handler;
            ErrorMsg = errorMsg;
            Exception = exception;
        }

        /// <summary>
        /// Gets pipeline handler that failed.
        /// </summary>
        public object Handler { get; private set; }

        /// <summary>
        /// Gets the pipeline handlers own description of what happened.
        /// </summary>
        public string ErrorMsg { get; private set; }

        /// <summary>
        /// Gets exception that was thrown
        /// </summary>
        public Exception Exception { get; private set; }
    }
}