using System;

namespace Griffin.Decoupled.Commands.Pipeline.Messages
{
    /// <summary>
    /// The pipeline itself (one of the handlers) have failed.
    /// </summary>
    public class PipelineFailure
    {
        public PipelineFailure(object handler, object pipelineMessage, string errorMsg, Exception exception)
        {
            Handler = handler;
            PipelineMessage = pipelineMessage;
            ErrorMsg = errorMsg;
            Exception = exception;
        }

        /// <summary>
        /// Gets pipeline handler that failed.
        /// </summary>
        public object Handler { get; private set; }

        public object PipelineMessage { get; set; }

        /// <summary>
        /// Gets the pipeline handlers own description of what happened.
        /// </summary>
        public string ErrorMsg { get; private set; }

        /// <summary>
        /// Gets exception that was thrown
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("'{0}' failed to handle a message '{1}' due to: {2}\r\n{3}", Handler.GetType().FullName, PipelineMessage, ErrorMsg, Exception);
        }
    }
}