using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arbitrage
{
    [TestClass]
    public class CurrencyExchangeTest
    {
        [TestMethod]
        public void TestConstructorFromId()
        {
            CurrencyExchange ce = new CurrencyExchange("BBTC");
            Assert.AreEqual(Exchange.BTCe, ce.Exchange);
            Assert.AreEqual(Currency.BTC, ce.Currency);

            ce = new CurrencyExchange("OUSD");
            Assert.AreEqual(Exchange.OKCoin, ce.Exchange);
            Assert.AreEqual(Currency.USD, ce.Currency);

            ce = new CurrencyExchange("CUNK");
            Assert.AreEqual(Exchange.BTCChina, ce.Exchange);
            Assert.AreEqual(Currency.UNK, ce.Currency);
        }
    }
}
