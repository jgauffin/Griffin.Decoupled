using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// Command contract
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Get command id
        /// </summary>
        Guid Id { get; }
    }
}
