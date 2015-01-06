using System;
using System.IO;
using Arbitrage.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Telerik.JustMock;

namespace Arbitrage.Exchanges
{
    [TestClass]
    public class ExchangeBaseTest
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
        [Ignore]
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
        [Ignore]
        public void TestTradeOrderCancel()
        {
            JObject mock = Mock.Create<JObject>();
            Mock.Arrange(() => JObject.Parse(Arg.AnyString)).Returns(mock);
            Mock.Arrange(() => mock["id"]).Returns("1");
            using (Stream stream = factory.ToStream("mockedresponse"))
            {
                webHelper.MockRequest(stream);
                ClientBitfinex client = CreateClient();
                TradeOrder order = factory.CreateTradeOrder("1");
                Assert.IsTrue(client.TradeOrderCancel(order, "reason"));
            }
        }

        [TestMethod]
        [Ignore]
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
        [Ignore]
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
        [Ignore]
        public void TestAsyncRefreshBalance()
        {
            client.AsyncRefreshBalance();
        }

        private ClientBitfinex CreateClient()
        {
            ClientBitfinex c = Mock.Create<ClientBitfinex>(Behavior.CallOriginal);
            c.ApiPassword = "pwd";
            c.ApiKey = "key";
            c.Init();
            return c;
        }
    }
}
