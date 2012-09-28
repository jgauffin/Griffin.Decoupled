namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// Command which is stored
    /// </summary>
    public class StoredCommand
    {
        /// <summary>
        /// Gets or sets actual command
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// Gets or sets number of attempts to execute this command
        /// </summary>
        /// <remarks>Default = 0</remarks>
        public int Attempts { get; set; }
    }
}