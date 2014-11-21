using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mt5trade
{
    class QuoteEventArgs : EventArgs
    {
        public IQuote Quote { get; private set; }

        public QuoteEventArgs(IQuote quote)
        {
            Quote = quote;
        }
    }
}
