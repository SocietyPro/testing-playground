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
        private readonly SqlHelper sqlHelper = new SqlHelper();
        private readonly SerializationHelper<TradeOrder> serializationHelper = new SerializationHelper<TradeOrder>();
        
        public void TestFindNoItem()
        {
            TradeOrder order = createTradeOrder("1");
            sqlHelper.MockConnection();
            sqlHelper.MockDataReaderWithOneLine("result");
            
            List<TradeOrder> list = TradeOrder.Find(123);
            Assert.AreEqual(0, list.Count);
            
        }

        public void TestFindOneItem()
        {
            TradeOrder order = createTradeOrder("1");
            sqlHelper.MockConnection();
            sqlHelper.MockDataReaderWithOneLine("result");
            serializationHelper.MockPrivateDeserializer(order);
            
            List<TradeOrder> list = TradeOrder.Find(123);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("1", list[0].Id);
        }

        public void TestSelectNonExisting()
        {
            sqlHelper.MockConnection();
            sqlHelper.MockEmptyDataReader();
            Assert.IsNull(TradeOrder.Select(1));
        }

        public void TestSelectExisting()
        {
            TradeOrder order = createTradeOrder("1");
            sqlHelper.MockConnection();
            sqlHelper.MockDataReaderWithOneLine("result");
            serializationHelper.MockPrivateDeserializer(order);
            
            TradeOrder result = TradeOrder.Select(1);
            Assert.AreEqual("1", result.Id);
        }
       

        private TradeOrder createTradeOrder(string id)
        {
           TradeOrder order = new TradeOrder();
           order.Id = "1";
           return order;
        }
    }
}
