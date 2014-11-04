using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MtApi5;

namespace Mt5trade
{
    public partial class Form1 : Form
    {
        private MtApi5Client apiClient = new MtApi5Client();

        public Form1()
        {
            InitializeComponent();

            apiClient.QuoteAdded += apiClient_QuoteAdded;
            apiClient.QuoteRemoved += apiClient_QuoteRemoved;
            apiClient.QuoteUpdated += apiClient_QuoteUpdated;
            apiClient.ConnectionStateChanged += apiClient_ConnectionStateChanged;
        }

        private void RunOnUIThread(Action action)
        {
            this.BeginInvoke(action);
        }

        void apiClient_ConnectionStateChanged(object sender, Mt5ConnectionEventArgs e)
        {
            RunOnUIThread(() =>
            {
                toolStripStatusConnection.Text = e.Status.ToString();

                switch (e.Status)
                {
                    case Mt5ConnectionState.Connected:
                        onConnected();
                        break;
                    case Mt5ConnectionState.Disconnected:
                    case Mt5ConnectionState.Failed:
                        onDisconnected();
                        break;
                }
            });
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            string serverName = textBoxServerName.Text;

            int port;
            int.TryParse(textBoxPort.Text, out port);

            if (string.IsNullOrEmpty(serverName))
                apiClient.BeginConnect(port);
            else
                apiClient.BeginConnect(serverName, port);
        }

        private void onConnected()
        {
            var quotes = apiClient.GetQuotes();

            if (quotes != null)
            {
                foreach (var quote in quotes)
                {
                    addNewQuote(quote);
                }
            }
        }

        private void onDisconnected()
        {
            listViewQuotes.Items.Clear();
        }

        void apiClient_QuoteUpdated(object sender, string symbol, double bid, double ask)
        {
            this.BeginInvoke((Action)(() =>
            {
                changeQuote(symbol, bid, ask);
            }));
        }

        void apiClient_QuoteRemoved(object sender, Mt5QuoteEventArgs e)
        {
            string instrument = e.Quote.Instrument;

            RunOnUIThread(() =>
            {
                removeQuote(e.Quote);
            });
        }

        void apiClient_QuoteAdded(object sender, Mt5QuoteEventArgs e)
        {
            RunOnUIThread(() =>
            {
                addNewQuote(e.Quote);
            });
        }

        private void addNewQuote(Mt5Quote quote)
        {
            if (quote != null
                && string.IsNullOrEmpty(quote.Instrument) == false
                && listViewQuotes.Items.ContainsKey(quote.Instrument) == false)
            {
                var item = new ListViewItem(quote.Instrument);
                item.Name = quote.Instrument;
                item.SubItems.Add(quote.Bid.ToString());
                item.SubItems.Add(quote.Ask.ToString());
                item.SubItems.Add("1");
                listViewQuotes.Items.Add(item);
            }
            else
            {
                var item = listViewQuotes.Items[quote.Instrument];
                int feedCount = 0;
                int.TryParse(item.SubItems[3].Text, out feedCount);
                feedCount++;
                item.SubItems[3].Text = feedCount.ToString();
            }
        }

        private void removeQuote(Mt5Quote quote)
        {
            if (quote != null
                && string.IsNullOrEmpty(quote.Instrument) == false
                && listViewQuotes.Items.ContainsKey(quote.Instrument) == true)
            {
                var item = listViewQuotes.Items[quote.Instrument];
                int feedCount = 0;
                int.TryParse(item.SubItems[3].Text, out feedCount);
                feedCount--;
                if (feedCount <= 0)
                {
                    listViewQuotes.Items.RemoveByKey(quote.Instrument);
                }
                else
                {
                    item.SubItems[3].Text = feedCount.ToString();
                }
            }
        }

        private void changeQuote(string symbol, double bid, double ask)
        {
            if (string.IsNullOrEmpty(symbol) == false)
            {
                if (listViewQuotes.Items.ContainsKey(symbol) == true)
                {
                    var item = listViewQuotes.Items[symbol];
                    item.SubItems[1].Text = bid.ToString();
                    item.SubItems[2].Text = ask.ToString();
                }

                if (checkBoxFollowPrice.Checked == true)
                {
                    if (symbol.Equals(textBoxOrderSymbol.Text))
                    {
                        textBoxBuyPrice.Text = ask.ToString();
                        textBoxSellPrice.Text = bid.ToString();
                    }
                }
            }
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            apiClient.BeginDisconnect();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            apiClient.BeginDisconnect();
        }

        private void btbBuy_Click(object sender, EventArgs e)
        {
            string symbol = textBoxOrderSymbol.Text;
            double volume = (double) numericOrderVolume.Value;
            double price = 0;
            double.TryParse(textBoxBuyPrice.Text, out price);

            SendOrder(symbol, price, volume, ENUM_ORDER_TYPE.ORDER_TYPE_BUY);
        }

        private void btnSell_Click(object sender, EventArgs e)
        {
            string symbol = textBoxOrderSymbol.Text;
            double volume = (double)numericOrderVolume.Value;
            double price = 0;
            double.TryParse(textBoxBuyPrice.Text, out price);

            SendOrder(symbol, price, volume, ENUM_ORDER_TYPE.ORDER_TYPE_SELL);
        }

        private void SendOrder(string symbol, double price, double volume, ENUM_ORDER_TYPE orderType)
        {
            if (apiClient.ConnectionState != Mt5ConnectionState.Connected)
            {
                MessageBox.Show("Client is not connected to MetaTrader5 terminal", "Warning");
                return;
            }

            if (string.IsNullOrEmpty(symbol))
            {
                MessageBox.Show("Select symbol for trade", "Warning");
                return;
            }

            if (volume <= 0)
            {
                MessageBox.Show("Volume can't be less or equal zero", "Warning");
                return;
            }

            //make trader request to MT terminal
            var request = new MqlTradeRequest { Action = ENUM_TRADE_REQUEST_ACTIONS.TRADE_ACTION_DEAL
                , Symbol = symbol
                , Type = orderType
                , Price = price
                , Volume = volume
                , Comment = "Test Trade Request"
            };
            MqlTradeResult result;
            bool retVal = apiClient.OrderSend(request, out result);
            
            //TODO: use trade result
            string resultMessage = (retVal == true) 
                ? "OrderSend success. " + MqlTradeResultToString(result)
                : "OrderSend failed. " + MqlTradeResultToString(result);

            addToLog(resultMessage);
        }

        private void listViewQuotes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewQuotes.SelectedItems.Count > 0)
            {
                textBoxOrderSymbol.Text = listViewQuotes.SelectedItems[0].Text;
                textBoxBuyPrice.Text = listViewQuotes.SelectedItems[0].SubItems[2].Text;
                textBoxSellPrice.Text = listViewQuotes.SelectedItems[0].SubItems[1].Text;
            }
        }

        private static string MqlTradeResultToString(MqlTradeResult result)
        {
            return result != null ?
                "Retcode = " + result.Retcode + ";"
                + " Comment = " + result.Comment + ";"
                + " Order = " + result.Order + ";"
                + " Volume = " + result.Volume + ";"
                + " Price = " + result.Price + ";"
                + " Deal = " + result.Deal + ";"
                + " Request_id = " + result.Request_id + ";"
                + " Bid = " + result.Bid + ";"
                + " Ask = " + result.Ask + ";" : string.Empty;
        }

        private void addToLog(string msg)
        {
            listBoxEventLog.Items.Add(msg);
            listBoxEventLog.SetSelected(listBoxEventLog.Items.Count - 1, true);
            listBoxEventLog.SetSelected(listBoxEventLog.Items.Count - 1, false);
        }
    }
}
