using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Get command id
        /// </summary>
        public Guid Id { get; private set; }
    }
}
