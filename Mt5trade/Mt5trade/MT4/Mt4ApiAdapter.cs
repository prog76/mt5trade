using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MtApi;
using System.Drawing;

namespace Mt5trade.MT4
{
    class Mt4ApiAdapter: IApiAdapter
    {
        private readonly MtApiClient apiClient = new MtApiClient();

        public Mt4ApiAdapter()
        {
            apiClient.QuoteAdded += apiClient_QuoteAdded;
            apiClient.QuoteRemoved += apiClient_QuoteRemoved;
            apiClient.QuoteUpdated += apiClient_QuoteUpdated;
            apiClient.ConnectionStateChanged += apiClient_ConnectionStateChanged;

        }

        void apiClient_ConnectionStateChanged(object sender, MtConnectionEventArgs e)
        {
            ConnectionStateChanged.FireEvent(this
                , new ConnectionEventArgs(ConvertConnectionState(e.Status), e.ConnectionMessage));
        }

        void apiClient_QuoteUpdated(object sender, string symbol, double bid, double ask)
        {
            if (QuoteUpdated != null)
            {
                QuoteUpdated(this, symbol, bid, ask);
            }
        }

        void apiClient_QuoteRemoved(object sender, MtQuoteEventArgs e)
        {
            Mt4QuoteAdapter quoteAdapter = new Mt4QuoteAdapter(e.Quote);
            QuoteRemoved.FireEvent(this, new QuoteEventArgs(quoteAdapter));
        }

        void apiClient_QuoteAdded(object sender, MtQuoteEventArgs e)
        {
            Mt4QuoteAdapter quoteAdapter = new Mt4QuoteAdapter(e.Quote);
            QuoteAdded.FireEvent(this, new QuoteEventArgs(quoteAdapter));
        }

        private static ConnectionState ConvertConnectionState(MtConnectionState state)
        {
            switch (state)
            {
                case MtConnectionState.Connected:
                    return ConnectionState.Connected;
                case MtConnectionState.Connecting:
                    return ConnectionState.Connecting;
                case MtConnectionState.Disconnected:
                    return ConnectionState.Disconnected;
                case MtConnectionState.Failed:
                    return ConnectionState.Failed;
            }

            return ConnectionState.Failed;
        }

        public void BeginConnect(string host, int port)
        {
            apiClient.BeginConnect(host, port);
        }

        public void BeginConnect(int port)
        {
            apiClient.BeginConnect(port);
        }

        public void BeginDisconnect()
        {
            apiClient.BeginDisconnect();
        }

        public ulong OrderBuy(string symbol, double price, double volume)
        {
            return SendOrder(symbol, volume, price, TradeOperation.OP_BUY);
        }

        public ulong OrderSell(string symbol, double price, double volume)
        {
            return SendOrder(symbol, volume, price, TradeOperation.OP_SELL);
        }

        public bool OrderCloseAll()
        {
            return apiClient.OrderCloseAll();
        }

        public IEnumerable<IQuote> GetQuotes()
        {
            List<IQuote> quotes = null;
            var mtQuotes = apiClient.GetQuotes();
            if (mtQuotes != null)
            {
                quotes = new List<IQuote>();
                foreach (var mtQuote in mtQuotes)
                {
                    quotes.Add(new Mt4QuoteAdapter(mtQuote));
                }
            }

            return quotes;
        }

        public ConnectionState ConnectionState 
        {
            get 
            { 
                return ConvertConnectionState(apiClient.ConnectionState);
            } 
        }

        public event QuoteHandler QuoteUpdated;
        public event EventHandler<QuoteEventArgs> QuoteAdded;
        public event EventHandler<QuoteEventArgs> QuoteRemoved;
        public event EventHandler<ConnectionEventArgs> ConnectionStateChanged;

        private ulong SendOrder(string symbol, double volume, double price, TradeOperation command)
        {
            int orderId = apiClient.OrderSend(symbol, command, volume, price, 10, 0, 0, "Trade Request from Mt5Trade", 0, DateTime.Now, Color.Green);
            return orderId > 0 ? (ulong) orderId : 0;
        }
    }
}
