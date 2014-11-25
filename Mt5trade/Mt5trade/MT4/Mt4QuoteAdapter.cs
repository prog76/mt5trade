using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MtApi;

namespace Mt5trade.MT4
{
    class Mt4QuoteAdapter : IQuote
    {
        private readonly MtQuote mQuote;

        public Mt4QuoteAdapter(MtQuote quote)
        {
            mQuote = quote;
        }

        public string Instrument { get { return mQuote.Instrument; } }
        public double Bid { get { return mQuote.Bid; } }
        public double Ask { get { return mQuote.Ask; } }
    }
}
