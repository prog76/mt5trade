using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mt5trade
{
    public delegate void QuoteHandler(object sender, string symbol, double bid, double ask);

    interface IApiAdapter
    {
        void BeginConnect(string host, int port);
        void BeginConnect(int port);
        void BeginDisconnect();
        ulong OrderBuy(string symbol, double price, double volume);
        ulong OrderSell(string symbol, double price, double volume);
        bool OrderCloseAll();
        IEnumerable<IQuote> GetQuotes();

        ConnectionState ConnectionState { get; }

        event QuoteHandler QuoteUpdated;
        event EventHandler<QuoteEventArgs> QuoteAdded;
        event EventHandler<QuoteEventArgs> QuoteRemoved;
        event EventHandler<ConnectionEventArgs> ConnectionStateChanged;
    }
}
