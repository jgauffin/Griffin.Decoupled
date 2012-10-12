namespace Griffin.Decoupled.Pipeline
{
    /// <summary>
    /// The pipeline which is used to handle the dispatching
    /// </summary>
    public interface IPipeline
    {
        /// <summary>
        /// Invoke a message
        /// </summary>
        /// <param name="message">Message to invoke</param>
        void Send(IDownstreamMessage message);

        /// <summary>
        /// Set upstream destination
        /// </summary>
        /// <param name="handler">Will receive all upstream messages</param>
        void SetDestination(IUpstreamHandler handler);

        /// <summary>
        /// MUST be called before the pipeline can be used.
        /// </summary>
        void Start();
    }
}