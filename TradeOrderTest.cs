using System;
using System.Collections.Generic;
using Arbitrage.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arbitrage
{
    [TestClass]
    public class TradeOrderTest
    {
        private readonly SqlHelper sqlHelper = new SqlHelper();
        private readonly SerializationHelper<TradeOrder> serializationHelper = new SerializationHelper<TradeOrder>();
        private readonly TestObjectFactory factory = new TestObjectFactory();
        
        [TestMethod]
        public void TestFindNoItem()
        {
            
            sqlHelper.MockEmptyDataReader();
            
            List<TradeOrder> list = TradeOrder.Find(123);
            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void TestFindOneItem()
        {
            TradeOrder order = factory.CreateTradeOrder("1");
            sqlHelper.MockDataReaderWithOneLine("result");
            serializationHelper.MockPrivateDeserializer(order);
            
            List<TradeOrder> list = TradeOrder.Find(123);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("1", list[0].Id);
        }

        [TestMethod]
        public void TestSelectNonExisting()
        {
            sqlHelper.MockEmptyDataReader();
            Assert.IsNull(TradeOrder.Select(1));
        }

        [TestMethod]
        public void TestSelectExisting()
        {
            TradeOrder order = factory.CreateTradeOrder("1");
            sqlHelper.MockDataReaderWithOneLine("result");
            serializationHelper.MockPrivateDeserializer(order);
            
            TradeOrder result = TradeOrder.Select(1);
            Assert.AreEqual("1", result.Id);
        }

        [TestMethod]
        public void TestSave()
        {
            serializationHelper.MockPrivateSerializer();
            TradeOrder order = factory.CreateTradeOrder("1");
            sqlHelper.MockNonQuery();
            order.Save();
        }

        
    }
}