using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Arbitrage.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace Arbitrage
{
    [TestClass]
    public class TradeTest
    {
        private readonly SqlHelper sqlHelper = new SqlHelper();
        [TestMethod]
        public void TestFindEmpty()
        {
            sqlHelper.MockConnection();
            Mock.SetupStatic(typeof(Trade), Behavior.CallOriginal);

            Trade trade = new Trade();



            SqlDataReader dr = Mock.Create<SqlDataReader>();
            Mock.Arrange(() => dr.Read()).Returns(false);
            Mock.Arrange(() => new SqlCommand().ExecuteReader()).Returns(dr);
            Mock.Arrange(() => Trade.Deserialize(Arg.IsAny<byte[]>())).Returns(trade);
            
            Mock.Arrange(() => new SqlCommand().ExecuteScalar()).Returns(12);
            
            CurrencyPair pair = new CurrencyPair(Exchange.BTCChina,Currency.USD, Exchange.Kraken, Currency.BTC);
            DateTime date = new DateTime(2011, 6,1);
            List<Trade> list = Trade.Find(pair,date);
            Assert.AreEqual(0, list.Count);
        }
    }
}
