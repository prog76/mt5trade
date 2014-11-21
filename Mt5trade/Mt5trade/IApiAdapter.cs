using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mt5trade
{
    interface IApiAdapter
    {
        public delegate void QuoteHandler(object sender, string symbol, double bid, double ask);

        public void BeginConnect(string host, int port);
        public void BeginConnect(int port);
        public void BeginDisconnect();
        public bool OrderBuy(string symbol, double price, double volume);
        public bool OrderSell(string symbol, double price, double volume);
        public bool OrderCloseAll();
        public IEnumerable<IQuote> GetQuotes();

        public ConnectionState ConnectionState { get; }

        public event QuoteHandler QuoteUpdated;
        public event EventHandler<QuoteEventArgs> QuoteAdded;
        public event EventHandler<QuoteEventArgs> QuoteRemoved;
        public event EventHandler<ConnectionEventArgs> ConnectionStateChanged;
    }
}
