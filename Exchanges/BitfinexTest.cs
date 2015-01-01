using System.IO;
using BackendTest.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arbitrage.Exchanges
{
    [TestClass]
    public class BitfinexTest
    {
        
        private readonly WebRequestHelper webHelper = new WebRequestHelper();
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
            using (Stream stream = factory.ToStream("mockedresponse"))
            {
                webHelper.MockRequest(stream);
                TradeOrder order = CreateOrder();
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
                TradeOrder order = CreateOrder();
                client.TradeOrderCancel(order, "reason");
            }
        }

        [TestMethod]
        public void TestAdjustBalances()
        {
            using (Stream stream = factory.ToStream("mockedresponse"))
            {
                webHelper.MockRequest(stream);
                TradeOrder order = CreateOrder();
                client.AdjustBalances(order);
            }
        }

        [TestMethod]
        public void TestRefreshTradeOrder()
        {
            using (Stream stream = factory.ToStream("mockedresponse"))
            {
                webHelper.MockRequest(stream);
                TradeOrder order = CreateOrder();
                client.RefreshTradeOrder(order);
            }
        }

        private ClientBitfinex CreateClient()
        {
            ClientBitfinex c = new ClientBitfinex();
            c.ApiPassword = "pwd";
            c.ApiKey = "key";
            c.Init();
            return c;

        }

        private TradeOrder CreateOrder()
        {
            TradeOrder order = new TradeOrder();
            return order;


        }
    }
}
