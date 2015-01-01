using System;
using System.Collections.Generic;
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
            TradeOrder order = new TradeOrder();
            order.Id = "1";
            setup.SetupSql();
            SqlDataReader dr = Mock.Create<SqlDataReader>();
            Mock.SetupStatic(typeof(TradeOrder), Behavior.CallOriginal);
            byte[] result = System.Text.Encoding.UTF8.GetBytes("result");

            Mock.NonPublic.Arrange<TradeOrder>(typeof(TradeOrder), "Deserialize", ArgExpr.IsAny<byte[]>()).Returns(order);
            Mock.Arrange(() => dr.Read()).Returns(true).InSequence();
            Mock.Arrange(() => dr.Read()).Returns(false).InSequence();
            Mock.Arrange(() => dr[0]).Returns(result);
            Mock.Arrange(() => new SqlCommand().ExecuteReader()).Returns(dr);

            Mock.Arrange(() => TradeOrder.Find(Arg.AnyInt)).CallOriginal();
            List<TradeOrder> list = TradeOrder.Find(123);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("1", list[0].Id);
        }
    }
}
