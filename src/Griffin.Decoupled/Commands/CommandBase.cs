using System;

namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// Basic implementation
    /// </summary>
    public class CommandBase : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBase" /> class.
        /// </summary>
        protected CommandBase()
        {
            CommandId = Guid.NewGuid();
        }

        #region ICommand Members

        /// <summary>
        /// Get command id
        /// </summary>
        public Guid CommandId { get; private set; }

        #endregion
    }
}