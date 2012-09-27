using System.Runtime.CompilerServices;

namespace Griffin.Decoupled.DomainEvents
{
    /// <summary>
    /// The domain event implementation
    /// </summary>
    /// <remarks>
    /// <para>
    /// Domain events are events which are generated when the user have took some action (and not the system). A correct event
    /// would be <c>UserRegistered</c>, <c>ReplyPosted</c> (forum/comment), <c>ArticleUpvoted</c> while <c>RowUpdated</c>, <c>OrderItemUpdated</c> is not. However
    /// <c>QuantityUpdated</c> would (as opposed to the <c>OrderItemUpdated</c> example).
    /// </para>
    /// <para>
    /// The purpose with these events is to be able to decouple your application by letting any other part of your system take action on something which have happened.
    /// You can for instance have a <c>SendRegistrationEmail</c> class which subscribes on the <c>UserRegistered</c> event. The possibilites are endless.
    /// </para>
    /// <para>
    /// Domain event should only contain as little information as possible. For instance do not include the entire <c>User</c> but only <c>UserId</c>. And include information
    /// which is only relevent to the actual event.
    /// </para>
    /// <para>To get started look at the instructions on the <see cref="DomainEvent"/> class.</para>
    /// </remarks>
    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }
}