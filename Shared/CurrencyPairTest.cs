using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using System.Security.Permissions;
using BackendTest.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace Arbitrage
{
    [TestClass]
    public class CurrencyPairTest
    {

        private SqlHelper sqlHelper = new SqlHelper();
        private CacheHelper<CurrencyPair> cacheHelper = new CacheHelper<CurrencyPair>();
        private TestObjectFactory factory = new TestObjectFactory();

        [TestMethod]
        public void TestConstructorWithExchangeAndString()
        {
            CurrencyPair pair = new CurrencyPair(Exchange.Kraken, "USDBTC");
            Assert.AreEqual(Currency.USD, pair.From.Currency);
            Assert.AreEqual(Currency.BTC, pair.To.Currency);
            Assert.AreEqual(Exchange.Kraken, pair.To.Exchange);
            Assert.AreEqual(Exchange.Kraken, pair.From.Exchange);

            pair = new CurrencyPair(Exchange.OKCoin, "EUR_BTC");
            Assert.AreEqual(Currency.EUR, pair.From.Currency);
            Assert.AreEqual(Currency.BTC, pair.To.Currency);
            Assert.AreEqual(Exchange.OKCoin, pair.To.Exchange);
            Assert.AreEqual(Exchange.OKCoin, pair.From.Exchange);
        }

        [TestMethod]
        public void TestConstructorWithId()
        {
            CurrencyPair pair = new CurrencyPair("BBTC_OUSD");
            Assert.AreEqual(Currency.BTC, pair.From.Currency);
            Assert.AreEqual(Currency.USD, pair.To.Currency);
            Assert.AreEqual(Exchange.OKCoin, pair.To.Exchange);
            Assert.AreEqual(Exchange.BTCe, pair.From.Exchange);

        }

        [TestMethod]
        public void TestFiat()
        {
            CurrencyPair pair = new CurrencyPair(Exchange.Kraken, "USDBTC");
            Assert.AreEqual(Currency.USD, pair.Fiat);

            pair = new CurrencyPair(Exchange.BTCChina, "BTCCNY");
            Assert.AreEqual(Currency.CNY, pair.Fiat);
            Assert.AreEqual("[BTCChina]BTC/CNY", pair.ToString());

        }

        [TestMethod]
        public void TestId()
        {
            CurrencyPair pair = new CurrencyPair("BBTC_OUSD");
            Assert.AreEqual("BBTC_OUSD",pair.Id);
            Assert.AreEqual("[BTCe]BTC/[OKCoin]USD", pair.ToString());
        }

        [TestMethod]
        public void TestInverse()
        {
            CurrencyPair pair = new CurrencyPair();
            pair.Id = "BBTC_OUSD";
            CurrencyPair reverse = pair.Inverse();
            Assert.AreEqual("OUSD_BBTC", reverse.Id);
        }

        [TestMethod]
        public void TestNewDatabaseId()
        {
            CurrencyPair pair = new CurrencyPair("BBTC_OEUR");
            
            sqlHelper.SetupSql();
            Mock.Arrange(() => new SqlCommand().ExecuteScalar()).Returns(0).InSequence();
            Mock.Arrange(() => new SqlCommand().ExecuteScalar()).Returns(18).InSequence();
            
            Assert.AreEqual(18, pair.DatabaseId);
            Assert.AreEqual("BBTC_OEUR", pair.Id);
        }

        [TestMethod]
        public void TestExistingDatabaseId()
        {
            CurrencyPair pair = new CurrencyPair("BBTC_OUSD");
            sqlHelper.SetupSql();
            cacheHelper.ClearTheBitch(pair);

            Mock.Arrange(() => new SqlCommand().ExecuteScalar()).Returns(12);
            
            Assert.AreEqual(12, pair.DatabaseId);
            Assert.AreEqual("BBTC_OUSD", pair.Id);
            
        }

        [TestMethod]
        public void TestPairNameAtExchanger()
        {
            CurrencyExchange exchange = new CurrencyExchange(Currency.CAD, Exchange.Vos);
            CurrencyPair pair = new CurrencyPair(exchange, Currency.LTC);
            Assert.AreEqual("CADLTC", pair.PairNameAtExchanger());

            exchange = new CurrencyExchange(Currency.BTC, Exchange.Kraken);
            pair = new CurrencyPair(exchange, Currency.LTC);
            Assert.AreEqual("XBTLTC", pair.PairNameAtExchanger());

        }

        [TestMethod]
        public void TestIsBuy()
        {
            CurrencyPair pair = new CurrencyPair("BBTC_VUNK");
            Assert.IsTrue(pair.IsBuy);
            pair = new CurrencyPair("BBTC_VCAD");
            Assert.IsFalse(pair.IsBuy);
           
        }
    }
}
