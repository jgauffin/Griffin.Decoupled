using System.Runtime.CompilerServices;

namespace Griffin.Decoupled.Pipeline
{
    /// <summary>
    /// A pipeline is a way to allow several handlers to process something, in this case command requests or domain events (and any errors),
    /// in a decoupled way. Each handler do not really know what exists before or after itself. It just knows that it
    /// should either send the request to the next handler (downstream) or send an error to back up towards the original
    /// invoker (upstream).
    /// </summary>
    /// <remarks>
    /// <para>The messages wich is sent through the pipeline is documented in the <c>*.Pipeline.Messages</c> namespace under Commands/DomainEvents. Do note
    /// that every handler have to manually send the message to the next one. Messages that a handler can't process
    /// should be sent to the next one directly, don't choke on them.</para>
    /// <para>Each handler is also repsonsible of catching exceptions. However, do not treat them, but send them upstream in the appropiate message (which is defined
    /// in the <c>.Messages</c> namespace.</para>
    /// <para>You also have be careful when specifying the last handler in each direction, since it may NOT try to pass messages to the next (non existant) handler.
    /// The pipeline will catch those messages and throw an exception.
    /// </para>
    /// </remarks>
    [CompilerGenerated]
    public class NamespaceDoc
    {
    }
}