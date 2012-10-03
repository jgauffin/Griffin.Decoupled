using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffin.Decoupled.Commands
{
    interface IDispatcherFeature
    {
        void Dispatch(CommandState command);
    }

}
