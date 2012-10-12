using System;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.Commands.Pipeline.Messages
{
    /// <summary>
    /// The pipeline itself (one of the handlers) have failed.
    /// </summary>
    public class PipelineFailure : IUpstreamMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineFailure" /> class.
        /// </summary>
        /// <param name="handler">Up/Down handler which failed.</param>
        /// <param name="message">Message which caused the failure.</param>
        /// <param name="errorMsg">Error descrption.</param>
        /// <param name="exception">Exception which was thrown when the messages was being handled..</param>
        public PipelineFailure(object handler, object message, string errorMsg, Exception exception)
        {
            if (handler == null) throw new ArgumentNullException("handler");
            if (message == null) throw new ArgumentNullException("message");
            if (errorMsg == null) throw new ArgumentNullException("errorMsg");
            Handler = handler;
            Message = message;
            ErrorMsg = errorMsg;
            Exception = exception;
        }

        /// <summary>
        /// Gets pipeline handler that failed.
        /// </summary>
        public object Handler { get; private set; }

        /// <summary>
        /// Gets message which cause the exception
        /// </summary>
        public object Message { get; set; }

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
            return string.Format("'{0}' failed to handle a message '{1}' due to: {2}\r\n{3}", Handler.GetType().FullName,
                                 Message, ErrorMsg, Exception);
        }
    }
}