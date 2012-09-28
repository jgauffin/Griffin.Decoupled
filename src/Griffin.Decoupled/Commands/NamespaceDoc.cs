using System.Runtime.CompilerServices;

namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// Command implementation
    /// </summary>
    /// <remarks>
    /// <para>
    /// Do interpret commands as use cases and not operations in your system when you are using this library. It makes everything so much easier for everyone. 
    /// </para>
    /// <para>
    /// The easiest way to determine if something is an operation or an use case is simply ask a user ;) Or just thinking would I user have described that? For instance,
    /// a user would not have defined <c>UpdateRowForAccount</c>. But more likely <c>ActivateAccount</c>. That distinction is important since a command should
    /// contain everything that is required in an transaction. That's important so that we can scale out the command to another system if required.
    /// </para>
    /// <para>
    /// A command should not be able to return a result either. If you have to act upon a command, simply act upon one of the domain events that the command generates.
    /// </para>
    /// <para>As opposed to the domain events there may only be one handler per command.</para>
    /// <para>Commands are invoked though the <see cref="CommandDispatcher"/>. Read it's class documentation to figure out how to configure it.</para>
    /// </remarks>
    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }
}