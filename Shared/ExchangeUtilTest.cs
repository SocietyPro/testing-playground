using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arbitrage
{
    [TestClass]
    public class ExchangeUtilTest
    {
        [TestMethod]
        public void TestExchangePrefix()
        {
            Assert.AreEqual("C",ExchangeUtil.ExchangePrefix(Exchange.BTCChina));
            Assert.AreEqual("B", ExchangeUtil.ExchangePrefix(Exchange.BTCe));
            Assert.AreEqual("F", ExchangeUtil.ExchangePrefix(Exchange.Bitfinex));
            Assert.AreEqual("K", ExchangeUtil.ExchangePrefix(Exchange.Kraken));
            Assert.AreEqual("O", ExchangeUtil.ExchangePrefix(Exchange.OKCoin));
            Assert.AreEqual("V", ExchangeUtil.ExchangePrefix(Exchange.Vos));
        }

        [TestMethod]
        public void TestPrefixToExchange()
        {
            Assert.AreEqual(Exchange.BTCChina, ExchangeUtil.PrefixToExchange("C"));
            Assert.AreEqual(Exchange.BTCe, ExchangeUtil.PrefixToExchange("B"));
            Assert.AreEqual(Exchange.Bitfinex, ExchangeUtil.PrefixToExchange("F"));
            Assert.AreEqual(Exchange.Kraken, ExchangeUtil.PrefixToExchange("K"));
            Assert.AreEqual(Exchange.OKCoin, ExchangeUtil.PrefixToExchange("O"));
            
        }
    }
}
