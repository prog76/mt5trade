using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MtApi5;

namespace Mt5trade.MT5
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
            apiClient.BeginDisconnect();
        }

        public ulong OrderBuy(string symbol, double price, double volume)
        {
            return SendOrder(symbol, price, volume, ENUM_ORDER_TYPE.ORDER_TYPE_BUY);
        }

        public ulong OrderSell(string symbol, double price, double volume)
        {
            return SendOrder(symbol, price, volume, ENUM_ORDER_TYPE.ORDER_TYPE_SELL);
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
                    quotes.Add(new Mt5QuoteAdapter(mtQuote));
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

        private ulong SendOrder(string symbol, double price, double volume, ENUM_ORDER_TYPE type)
        {
            //make trader request to MT terminal
            var request = new MqlTradeRequest
            {
                Action = ENUM_TRADE_REQUEST_ACTIONS.TRADE_ACTION_DEAL, 
                Symbol = symbol,
                Type = type, 
                Price = price, 
                Volume = volume, 
                Comment = "Trade Request from Mt5Trade"
            };
            MqlTradeResult result;

            bool sended = apiClient.OrderSend(request, out result);
            return (sended == true) ? result.Order : 0;
        }
    }
}
