using System;
using System.Data.SqlClient;
using Arbitrage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace Arbitrage
{
    [TestClass]
    public class TradeOrderTest
    {
        [TestMethod]
        public void TestFind()
        {
            Mock.SetupStatic(typeof(Sql));
            SqlConnection conn = Mock.Create<SqlConnection>();
            Mock.Arrange(() => Sql.GetConnection(Arg.AnyString)).Returns(conn);
            TradeOrder.Find(123);

        }
    }
}
