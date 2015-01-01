using System;
using System.Data.SqlClient;
using Arbitrage;
using BackendTest.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace Arbitrage
{
   

    [TestClass]
    public class TradeOrderTest
    {
        private SqlHelper setup = new SqlHelper();
        [TestMethod]
        public void TestFind()
        {
            setup.SetupSql();
            SqlDataReader dr = Mock.Create<SqlDataReader>();
            Mock.Arrange(() => new SqlCommand().ExecuteReader()).Returns(dr);
            TradeOrder.Find(123);

        }
    }
}
