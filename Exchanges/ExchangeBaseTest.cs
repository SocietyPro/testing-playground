using System;
using System.IO;
using Arbitrage.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace Arbitrage.Exchanges
{
    [TestClass]
    public class ExchangeBaseTest
    {
        private readonly WebHelper webHelper = new WebHelper();
        private readonly TestObjectFactory factory = new TestObjectFactory();
        private ExchangeBase client;

        [TestInitialize]
        public void Setup()
        {
            client = CreateClient();
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
        public void TestTradeOrderCancel()
        {
            Mock.SetupStatic(typeof(TradeLog));
            Mock.NonPublic.Arrange<bool>(client, "_TradeOrderCancel", ArgExpr.IsAny<TradeOrder>()).Returns(true).OccursOnce();
            Mock.Arrange(() => TradeLog.Log(Arg.Matches<string>(x => x.Contains("Canceled Trade Order success")))).OccursOnce();
            Mock.NonPublic.Arrange(client, "_RefreshTradeOrder", (ArgExpr.IsAny<TradeOrder>())).DoNothing().OccursOnce();
            Mock.NonPublic.Arrange(client, "TradeOrderUpdate", (ArgExpr.IsAny<TradeOrder>())).DoNothing().OccursOnce();
            
            TradeOrder order = factory.CreateTradeOrder("1");
            Assert.IsTrue(client.TradeOrderCancel(order, "reason"));
            Assert.IsFalse(order.Active);
            
        }

        [TestMethod]
        public void TestAdjustBalances()
        {
            Mock.SetupStatic(typeof(WebSocketServer));
            Mock.SetupStatic(typeof(TradeWatch));
            Mock.SetupStatic(typeof(FundInfo));
            Mock.Arrange(()=> WebSocketServer.Current.Broadcast(Arg.IsAny<Balance>(), Arg.IsAny<Subscription>())).DoNothing().Occurs(2);
            Mock.Arrange(() => TradeWatch.SaveToDisk()).DoNothing();
            Mock.Arrange(() => FundInfo.GetFundInfo(Arg.AnyBool,Arg.AnyString)).DoNothing();

            TradeOrder order = factory.CreateTradeOrder("1");
            Balance balance = new Balance(Exchange.Bitfinex, Currency.CNY);
            balance.AmountAvailable = 5000;
            Balance toBalance = new Balance(Exchange.Bitfinex, Currency.BTC);
            toBalance.AmountAvailable = 100;
            client.Balances.TryAdd(Currency.CNY, balance);
            client.Balances.TryAdd(Currency.BTC, toBalance);
            client.AdjustBalances(order);
            
        }

        [TestMethod]
        public void TestRefreshTradeOrder()
        {
            Mock.NonPublic.Arrange(client, "_RefreshTradeOrder", ArgExpr.IsAny<TradeOrder>()).DoNothing().OccursOnce();
            TradeOrder order = factory.CreateTradeOrderWithoutOperations("1");
            order.Dirty = true;
            client.RefreshTradeOrder(order);
        }

        [TestMethod]
        [Ignore]
        public void TestAsyncRefreshBalance()
        {
            client.AsyncRefreshBalance();
        }

        private ExchangeBase CreateClient()
        {
            ClientBitfinex c = Mock.Create<ClientBitfinex>(Behavior.CallOriginal);
            c.ApiPassword = "pwd";
            c.ApiKey = "key";
            c.Init();
            return c;
        }
    }
}
