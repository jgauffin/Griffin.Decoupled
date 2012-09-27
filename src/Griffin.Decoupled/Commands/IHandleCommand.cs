namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// Handler of a specific command
    /// </summary>
    /// <typeparam name="T">Type of command to handle</typeparam>
    /// <remarks>Read the interface name as <c>I Handle Command "TheCommandName"</c></remarks>
    public interface IHandleCommand<in T> where T : class, ICommand
    {
        /// <summary>
        /// Invoke the command
        /// </summary>
        /// <param name="command">Command to run</param>
        void Invoke(T command);
    }
}