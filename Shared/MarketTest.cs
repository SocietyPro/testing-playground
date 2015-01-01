using System;
using Arbitrage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendTest.Shared
{
    [TestClass]
    public class MarketTest
    {
        [TestMethod]
        public void TestEquality()
        {
            CurrencyExchange exchange = new CurrencyExchange(Currency.BTC, Exchange.Bitfinex);
            CurrencyPair pair = new CurrencyPair(exchange, Currency.EUR);
            
            CurrencyPair pair2 = new CurrencyPair(exchange, Currency.LTC);
            Market market = new Market(pair);
            Market market2 = new Market(pair2);
            market.Equals(market2);
        }
    }
}
