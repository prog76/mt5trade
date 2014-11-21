using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mt5trade
{
    interface IQuote
    {
        public string Instrument { get; }
        public double Bid { get; }
        public double Ask { get; }
    }
}
