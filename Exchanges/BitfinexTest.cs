using System;
using System.IO;
using BackendTest.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace Arbitrage.Exchanges
{
    [TestClass]
    public class BitfinexTest
    {
        
        private readonly WebHelper webHelper = new WebHelper();
        private readonly TestObjectFactory factory = new TestObjectFactory();
        private ClientBitfinex client;

        [TestInitialize]
        public void Setup()
        {
            client = CreateClient();
        }

        [TestMethod]
        public void TestTradeHistory()
        {
            
            using (Stream stream = factory.ToStream("mockedresponse"))
            {
                webHelper.MockRequest(stream);
                string result = client.GetTradeHistory();
                Assert.AreEqual("mockedresponse", result);
            }
            
        }

        [TestMethod]
        public void TestExecuteTradeOrder()
        {
            Mock.Arrange(()=>client.SupportedPairs.Contains(Arg.IsAny<CurrencyPair>())).Returns(true);
            using (Stream stream = factory.ToStream("mockedresponse"))
            {
                webHelper.MockRequest(stream);
                TradeOrder order = factory.CreateTradeOrder("1");
                client.ExecuteTradeOrder(order);
            }
        }

        [TestMethod]
        public void TestTradeOrderCancel()
        {
            using (Stream stream = factory.ToStream("mockedresponse"))
            {
                webHelper.MockRequest(stream);
                ClientBitfinex client = CreateClient();
                TradeOrder order = factory.CreateTradeOrder("1");
                client.TradeOrderCancel(order, "reason");
            }
        }

        [TestMethod]
        public void TestAdjustBalances()
        {
            using (Stream stream = factory.ToStream("mockedresponse"))
            {
                webHelper.MockRequest(stream);
                TradeOrder order = factory.CreateTradeOrder("1");
                client.AdjustBalances(order);
            }
        }

        [TestMethod]
        public void TestRefreshTradeOrder()
        {
            TradeOrder order = factory.CreateTradeOrder("1");
            JObject jsonObject = Mock.Create<JObject>();
            Mock.Arrange(() => JObject.Parse(Arg.AnyString)).Returns(jsonObject);
            Mock.Arrange(() => order.Save()).DoNothing();

            using (Stream stream = factory.ToStream("mockedresponse"))
            {
                webHelper.MockRequest(stream);
                
                client.RefreshTradeOrder(order);
            }
        }

        [TestMethod]
        public void TestAsyncRefreshBalance()
        {
            client.AsyncRefreshBalance();
        }

        private ClientBitfinex CreateClient()
        {
           
            ClientBitfinex c = new ClientBitfinex();
            
            c.ApiPassword = "pwd";
            c.ApiKey = "key";
            
            c.Init();
            return c;

        }

        
    }
}
