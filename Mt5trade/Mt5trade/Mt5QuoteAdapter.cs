using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MtApi5;

namespace Mt5trade
{
    class Mt5QuoteAdapter : IQuote
    {
        private readonly Mt5Quote mQuote;

        public Mt5QuoteAdapter(Mt5Quote quote)
        {
            mQuote = quote;
        }

        public string Instrument { get { return mQuote.Instrument; } }
        public double Bid { get { return mQuote.Bid; } }
        public double Ask { get { return mQuote.Ask; } }
    }
}
