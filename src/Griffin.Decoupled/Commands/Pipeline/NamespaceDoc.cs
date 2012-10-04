using System.Runtime.CompilerServices;

namespace Griffin.Decoupled.Commands.Pipeline
{
    /// <summary>
    /// A pipeline is a way to allow several handlers to process something (in this case command requests and any errors)
    /// in a decoupled way. Each handler do not really know what exists before or after itself. It just knows that it
    /// should either send the request to the next handler (downstream) or send an error to back up towards the original
    /// invoker (upstream).
    /// </summary>
    /// <remarks>
    /// <para>The messages wich is sent through the pipeline is documented in the Messages namespace. Do note
    /// that every handler have to manually send the message to the next one. Messages that a handler can't process
    /// should be sent to the next one directly, don't choke on them.</para>
    /// </remarks>
    [CompilerGenerated]
    public class NamespaceDoc
    {
    }
}