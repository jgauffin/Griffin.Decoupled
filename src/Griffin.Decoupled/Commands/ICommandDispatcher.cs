namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// Contract for the command dispatcher implementations.
    /// </summary>
    public interface ICommandDispatcher
    {
        /// <summary>
        /// Dispatch the command to the handler
        /// </summary>
        /// <typeparam name="T">Type of command</typeparam>
        /// <param name="command">Command to execute</param>
        /// <remarks>Implementations should throw exceptions unless they are asynchronous or will attempt to retry later.</remarks>
        void Dispatch<T>(T command) where T : class, ICommand;

        /// <summary>
        /// Close the dispatcher gracefully.
        /// </summary>
        /// <remarks>Should make sure that all non-persitent commands are executed or stored before exeting.</remarks>
        void Close();
    }
}