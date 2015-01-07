using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Arbitrage.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace Arbitrage.Exchanges
{
    [TestClass]
    public class BitfinexTest
    {
        private ClientBitfinex client;
        private PrivateAccessor accessor;

        [TestInitialize]
        public void Setup()
        {
            client = Mock.Create<ClientBitfinex>(Behavior.CallOriginal);

            accessor = new PrivateAccessor(client);
        }

        [TestMethod]
        public void Test_ExecuteTradeOrderSupportedPair()
        {
            TradeOrder order = Mock.Create<TradeOrder>(Behavior.CallOriginal);
            order.Volume = 10;
            order.Price = 41;
            Mock.Arrange(() => order.IsBuy).Returns(false);
            
            order.Pair = new CurrencyPair(Exchange.Bitfinex, Currency.USD, Currency.BTC);
            MockReadStatus();
            MockSendRequest();
            HashSet<CurrencyPair> pairs = Mock.Create<HashSet<CurrencyPair>>();
            Mock.Arrange(() => pairs.Contains(Arg.IsAny<CurrencyPair>())).Returns(true);
            client.SupportedPairs = pairs;
            accessor.CallMethod("_ExecuteTradeOrder", order);
            Assert.AreEqual(10, order.Volume);
            Assert.AreEqual(41, order.Price);
        }

        [TestMethod]
        public void Test_ExecuteTradeOrderUnsupportedPair()
        {
            MockDebugAssert();
            TradeOrder order = Mock.Create<TradeOrder>(Behavior.CallOriginal);
            order.Pair = new CurrencyPair(Exchange.Bitfinex, Currency.USD, Currency.BTC);
            order.Volume = 10;
            order.Price = new Decimal(0.1);
            Mock.Arrange(() => order.IsBuy).Returns(true);

            MockReadStatus();
            MockSendRequest();
            HashSet<CurrencyPair> pairs = Mock.Create<HashSet<CurrencyPair>>();
            Mock.Arrange(() => pairs.Contains(Arg.IsAny<CurrencyPair>())).Returns(false);
            client.SupportedPairs = pairs;
            accessor.CallMethod("_ExecuteTradeOrder", order);
           
            Assert.AreEqual(10, order.Volume);
            Assert.AreEqual(new Decimal(0.1), order.Price);
        }

        [TestMethod]
        public void Test_RefreshTradeOrder()
        {
            TradeOrder order = new TradeOrder();
            order.Id = "1";
            MockReadStatus();
            MockSendRequest();
            accessor.CallMethod("_RefreshTradeOrder", order);
        }
 
        [TestMethod]
        public void Test_RefreshBalance()
        {
            Mock.NonPublic.Arrange(client, "SetBalance", ArgExpr.IsAny<Currency>(), ArgExpr.IsAny<decimal>(), ArgExpr.IsAny<decimal>(), ArgExpr.IsAny<decimal>()).DoNothing();
            JArray array = new JArray();
            JObject jsonOrder = createJsonBalance();
          
            array.Add(jsonOrder);
          
            Mock.Arrange(()=>JArray.Parse(Arg.AnyString)).Returns(array);

            MockReadStatus();
            MockSendRequest();
            accessor.CallMethod("_RefreshBalance");
        }
 
        

        [TestMethod]
        public void TestReadStatusBuy()
        {
           
            CurrencyPair pair = Mock.Create<CurrencyPair>();
            Mock.Arrange(() => pair.IsBuy).Returns(true);
            TradeOrder order = new TradeOrder();
            JObject jsonObject = CreateJsonOrder(200, 50);

            DateTime tradeDate = new DateTime(2014,1,1);
            MockDateTimeNow(tradeDate);

            accessor.CallMethod("ReadStatus", order, jsonObject);
            Assert.AreEqual("1", order.Id);
            Assert.AreEqual(200, order.VolumeReceived);
            Assert.AreEqual(10000, order.VolumeExecuted);
            Assert.AreEqual(tradeDate, order.DateUpdated);
        }

        [TestMethod]
        public void TestReadStatusSell()
        {
            CurrencyPair pair = Mock.Create<CurrencyPair>();
            Mock.Arrange(() => pair.IsBuy).Returns(false);
            TradeOrder order = new TradeOrder();
            JObject jsonObject = CreateJsonOrder(200, 50);

            DateTime tradeDate = new DateTime(2014,1,1);
            MockDateTimeNow(tradeDate);

            accessor.CallMethod("ReadStatus", order, jsonObject);
            Assert.AreEqual("1", order.Id);
            Assert.AreEqual(10000, order.VolumeReceived);
            Assert.AreEqual(200, order.VolumeExecuted);
            Assert.AreEqual(tradeDate, order.DateUpdated);
        }

        [TestMethod]
        public void Test_TradeOrderCancel()
        {
            MockReadStatus();
            MockSendRequest();
            Mock.NonPublic.Arrange(client, "_RefreshTradeOrder", ArgExpr.IsAny<TradeOrder>()).DoNothing().OccursAtMost(2);
            Mock.NonPublic.Arrange(client, "TradeOrderUpdate", ArgExpr.IsAny<TradeOrder>()).DoNothing().OccursOnce();
            Mock.Arrange(() => System.Threading.Thread.Sleep(Arg.AnyInt)).DoNothing();
            
            TradeOrder order = Mock.Create<TradeOrder>();
            Mock.Arrange(() => order.Id).Returns("1");
            Mock.Arrange(() => order.Active).Returns(false).InSequence().Returns(false).InSequence();
    
            bool result = (bool)accessor.CallMethod("_TradeOrderCancel", order);
            Assert.IsTrue(result);
            
            Assert.IsFalse(order.IsCancelling);
        }
 
        [TestMethod]
        public void Test_RefreshTradeOrders()
        {
            JArray array = new JArray();
            JObject jsonOrder = CreateJsonOrder(50, 50);
            array.Add(jsonOrder);
            Mock.Arrange(()=>JArray.Parse(Arg.AnyString)).Returns(array);
            MockSendRequest();
            Mock.NonPublic.Arrange(client, "ReportOrderNotMonitored", ArgExpr.IsAny<string>()).DoNothing();
            MockReadStatus();
            accessor.CallMethod("_RefreshTradeOrders");
        }
 
        [TestMethod]
        public void TestSendRequestSuccess()
        {
            client.ApiPassword = "pwd";
            client.ApiKey = "key";
            WebHelper webHelper = new WebHelper();
            TestObjectFactory factory = new TestObjectFactory();
            string method = "send";
            Dictionary<string, object> options = new Dictionary<string, object>();
            string error = "";

            using (Stream stream = factory.ToStream("mockedresponse"))
            {
                webHelper.MockRequest(stream);
                string result = (string)accessor.CallMethod("SendRequest", method, options, error);
                Assert.AreEqual("", error);
                Assert.AreEqual("mockedresponse", result);
            }
           
        }

        [TestMethod]
        public void TestSendRequestError()
        {
            Mock.SetupStatic(typeof(JObject));
            JObject obj = new JObject();
            obj["message"] = "Mocked error";
            Mock.Arrange(() => JObject.Parse(Arg.AnyString)).Returns(obj);
            client.ApiPassword = "pwd";
            client.ApiKey = "key";
            WebHelper webHelper = new WebHelper();
            TestObjectFactory factory = new TestObjectFactory();
            Dictionary<string, object> options = new Dictionary<string, object>();
            
            using (Stream stream = factory.ToStream("mockedresponse"))
            {
                webHelper.MockRequestError(stream);
                string result = (string) accessor.CallMethod("SendRequest", "send", options, "");
               
                Assert.IsNull(result);
            }

        }
 
 
       
        private JObject CreateJsonOrder(int executedAmount, int executionPrice)
        {
            JObject jsonObject = new JObject();
            jsonObject["id"] = 1;
            jsonObject["is_live"] = "true";
            jsonObject["executed_amount"] = executedAmount;
            jsonObject["avg_execution_price"] = executionPrice;
            return jsonObject;
        }

        private JObject createJsonBalance()
        {
            JObject jsonOrder = new JObject();
            jsonOrder["id"] = "1";
            jsonOrder["type"] = "exchange";
            jsonOrder["currency"] = "usd";
            jsonOrder["amount"] = "51.5";
            jsonOrder["available"] = "400.20";
            return jsonOrder;
        }

        private void MockDateTimeNow(DateTime tradeDate)
        {
            Mock.SetupStatic(typeof(DateTime));
            Mock.Arrange(() => DateTime.UtcNow).Returns(tradeDate);
        }

       private void MockReadStatus()
        {
            Mock.SetupStatic(typeof(JObject));
            Mock.Arrange(() => JObject.Parse(Arg.AnyString)).DoNothing();
            Mock.NonPublic.Arrange(client, "ReadStatus", ArgExpr.IsAny<TradeOrder>(), ArgExpr.IsAny<JObject>()).DoNothing().OccursOnce();
        }

        private void MockSendRequest()
        {
            Mock.NonPublic.Arrange<string>(client, "SendRequest", ArgExpr.IsAny<string>(), ArgExpr.IsAny<Dictionary<string, object>>(), ArgExpr.Ref(ArgExpr.IsAny<string>())).Returns("response");
        }

        private void MockDebugAssert()
        {
            Mock.SetupStatic(typeof(Debug));
            Mock.Arrange(() => Debug.Assert(Arg.AnyBool, Arg.AnyString)).DoNothing();
        }
    }
}
