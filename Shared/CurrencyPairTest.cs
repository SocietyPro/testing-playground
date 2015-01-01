using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO.Pipes;
using System.Security.Policy;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace Arbitrage
{
    [TestClass]
    public class CurrencyPairTest
    {
        [ClassInitialize()]
        public static void Init(TestContext context)
        {
            Console.WriteLine("Enable Trace");
            //DebugView.IsTraceEnabled = true;
        }

        [TestInitialize]
        public void Setup()
        {
            //Mock.Reset();
            Thread.Sleep(1000);
            Console.WriteLine("Setup");
        }

        [TestCleanup]
        public void Teardown()
        {
            Console.WriteLine("Teardown");
            //Console.WriteLine(DebugView.LastTrace);
        }

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
        [Ignore]//Test how replace a constructor in a method
        public void TestContructorWithMockingNew()
        {
            Boolean mocked = false;
            Mock.Arrange(() => new CurrencyExchange(Arg.IsAny<Currency>(), Arg.IsAny<Exchange>())).DoInstead<Currency, Exchange>((x, y) => mocked = true);
            CurrencyExchange ce = new CurrencyExchange(Currency.BTC, Exchange.BTCe);
            CurrencyPair pair = new CurrencyPair(ce, Currency.CNY);   
            Assert.IsTrue(mocked);
        }

     

        [TestMethod]
        public void TestFiat()
        {
            CurrencyPair pair = new CurrencyPair(Exchange.Kraken, "USDBTC");
            Assert.AreEqual(Currency.USD, pair.Fiat);
        }

        [TestMethod]
        public void TestId()
        {
            CurrencyPair pair = new CurrencyPair("BBTC_OUSD");
            Assert.AreEqual("BBTC_OUSD",pair.Id);
        }

        [TestMethod]
        public void TestNewDatabaseId()
        {
            CurrencyPair pair = new CurrencyPair("BBTC_OEUR");
            
            SqlConnection conn = Mock.Create<SqlConnection>();
            
            
            Mock.Arrange(() => Sql.GetConnection(Arg.AnyString)).Returns(conn);

            Mock.Arrange(() => Sql.DbNullToInt(Arg.AnyObject)).CallOriginal();
            Mock.Arrange(() => new SqlCommand().ExecuteScalar()).Returns(0).InSequence();
            Mock.Arrange(() => new SqlCommand().ExecuteScalar()).Returns(18).InSequence();
            
            Assert.AreEqual(18, pair.DatabaseId);
            Assert.AreEqual("BBTC_OEUR", pair.Id);
        }

        [TestMethod]
        public void TestExistingDatabaseId()
        {
            
            CurrencyPair pair = new CurrencyPair("BBTC_OUSD");
            PrivateAccessor accessor = new PrivateAccessor(pair);
            
            SqlConnection conn = Mock.Create<SqlConnection>();
            
            Mock.Arrange(() => Sql.GetConnection(Arg.AnyString)).Returns(conn);
            Mock.Arrange(() => Sql.DbNullToInt(Arg.AnyObject)).CallOriginal();
            Mock.Arrange(() => new SqlCommand().ExecuteScalar()).Returns(12);
            
            Assert.AreEqual(12, pair.DatabaseId);
            Assert.AreEqual("BBTC_OUSD", pair.Id);
            
        }

        
    }
}
