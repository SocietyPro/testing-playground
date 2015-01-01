using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace Arbitrage
{
    [TestClass]
    public class TradeTest
    {
        [TestMethod]
        public void TestFind()
        {
            SqlConnection conn = Mock.Create<SqlConnection>();
            
            SqlDataReader dr = Mock.Create<SqlDataReader>();
            Mock.SetupStatic(typeof(Sql));

            SqlCommand cmd = Mock.Create<SqlCommand>(Constructor.Mocked);
            Mock.Arrange(()=>Sql.GetConnection(Arg.AnyString)).Returns(conn);
            Mock.Arrange(()=>cmd.ExecuteReader()).Returns(dr);
            

            CurrencyPair pair = new CurrencyPair(Exchange.BTCChina,Currency.USD, Exchange.Kraken, Currency.BTC);
            DateTime date = new DateTime(2011, 6,1);
            List<Trade> list = Trade.Find(pair,date);
            Assert.AreEqual(1, list.Count);
        }
    }
}
