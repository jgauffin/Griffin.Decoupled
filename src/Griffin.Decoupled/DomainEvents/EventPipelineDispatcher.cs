using System;
using Griffin.Decoupled.DomainEvents.Pipeline.Messages;
using Griffin.Decoupled.Pipeline;
using Griffin.Decoupled.Pipeline.Messages;

namespace Griffin.Decoupled.DomainEvents
{
    /// <summary>
    /// A dispatcher which uses a pipeline that allows you to easily customize the event handling.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The recommended approach is to use the <see cref="EventPipelineBuilder"/> to construct the pipeline
    /// </para>
    /// <para>You can for instance just add another handler to the pipeline to get support for event sourcing.</para>
    /// </remarks>
    public class EventPipelineDispatcher : IDomainEventDispatcher
    {
        private readonly IPipeline _pipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPipelineDispatcher" /> class.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        public EventPipelineDispatcher(IPipeline pipeline)
        {
            if (pipeline == null) throw new ArgumentNullException("pipeline");
            _pipeline = pipeline;
        }

        #region IDomainEventDispatcher Members

        /// <summary>
        /// Dispatch domain event.
        /// </summary>
        /// <typeparam name="T">Domain event type</typeparam>
        /// <param name="domainEvent">The domain event</param>
        public void Dispatch<T>(T domainEvent) where T : class, IDomainEvent
        {
            _pipeline.Send(new DispatchEvent(domainEvent));
        }

        /// <summary>
        /// Close the dispatcher gracefully.
        /// </summary>
        /// <remarks>Should make sure that all events are propagated before returning.</remarks>
        public void Close()
        {
            _pipeline.Send(new Shutdown());
        }

        #endregion
    }
}