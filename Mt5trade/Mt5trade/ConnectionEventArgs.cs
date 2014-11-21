using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mt5trade
{
    class ConnectionEventArgs: EventArgs
    {
        public ConnectionState Status { get; private set; }
        public String ConnectionMessage { get; private set; }

        public ConnectionEventArgs(ConnectionState status, string message)
        {
            Status = status;
            ConnectionMessage = message;
        }
    }
}
