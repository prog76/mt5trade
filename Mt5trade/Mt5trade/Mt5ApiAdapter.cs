using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MtApi5;

namespace Mt5trade
{
    class Mt5ApiAdapter: IApiAdapter
    {
        private readonly MtApi5Client apiClient = new MtApi5Client();

        public Mt5ApiAdapter()
        {
            apiClient.QuoteAdded += apiClient_QuoteAdded;
            apiClient.QuoteRemoved += apiClient_QuoteRemoved;
            apiClient.QuoteUpdated += apiClient_QuoteUpdated;
            apiClient.ConnectionStateChanged += apiClient_ConnectionStateChanged;

        }

        void apiClient_ConnectionStateChanged(object sender, Mt5ConnectionEventArgs e)
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

        void apiClient_QuoteRemoved(object sender, Mt5QuoteEventArgs e)
        {
            Mt5QuoteAdapter quoteAdapter = new Mt5QuoteAdapter(e.Quote);
            QuoteRemoved.FireEvent(this, new QuoteEventArgs(quoteAdapter));
        }

        void apiClient_QuoteAdded(object sender, Mt5QuoteEventArgs e)
        {
            Mt5QuoteAdapter quoteAdapter = new Mt5QuoteAdapter(e.Quote);
            QuoteAdded.FireEvent(this, new QuoteEventArgs(quoteAdapter));
        }

        private static ConnectionState ConvertConnectionState(Mt5ConnectionState state)
        {
            switch (state)
            {
                case Mt5ConnectionState.Connected:
                    return ConnectionState.Connected;
                case Mt5ConnectionState.Connecting:
                    return ConnectionState.Connecting;
                case Mt5ConnectionState.Disconnected:
                    return ConnectionState.Disconnected;
                case Mt5ConnectionState.Failed:
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
            throw new NotImplementedException();
        }

        public bool OrderBuy(string symbol, double price, double volume)
        {
            throw new NotImplementedException();
        }

        public bool OrderSell(string symbol, double price, double volume)
        {
            throw new NotImplementedException();
        }

        public bool OrderCloseAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IQuote> GetQuotes()
        {
            throw new NotImplementedException();
        }

        public event IApiAdapter.QuoteHandler QuoteUpdated;
        public event EventHandler<QuoteEventArgs> QuoteAdded;
        public event EventHandler<QuoteEventArgs> QuoteRemoved;
        public event EventHandler<ConnectionEventArgs> ConnectionStateChanged;
    }
}
