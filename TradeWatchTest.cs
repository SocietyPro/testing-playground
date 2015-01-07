using System;
using System.Diagnostics;
using Arbitrage.Exchanges;
using Arbitrage.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using System.Collections.Generic;

namespace Arbitrage
{
    [TestClass]
    public class TradeWatchTest
    {
        private TradeWatch watch;

        private static Database db;

        private ExchangeBase exchangeBase;

        [ClassInitialize]
        public static void init()
        {

            Mock.SetupStatic(typeof(TradeWatch));
            MockWebSocketServer();

            Mock.SetupStatic(typeof(Database));

            MockTradeLog();


        }

        

        

        [TestInitialize]
        public void setup()
        {
            
            watch = Mock.Create<TradeWatch>(Behavior.CallOriginal);
            watch.Id = new Guid();
            Mock.Arrange(() => TradeWatch.SaveToDisk()).DoNothing();

            db = Mock.Create<Database>(Behavior.Strict);
            Mock.Arrange(() => Database.Current).Returns(db);

            ExchangeRate exchangeRate = Mock.Create<ExchangeRate>();
            Dictionary<Currency, decimal> rates = new Dictionary<Currency, decimal>();
            rates.Add(Currency.USD,new decimal(1.1));
            rates.Add(Currency.CNY,new decimal(10.8));
            exchangeRate.Rates = rates;
            db.ExchangeRate = exchangeRate;

            exchangeBase = Mock.Create<ExchangeBase>(Behavior.Strict);
            Mock.Arrange(() => exchangeRate.GetRate(Arg.IsAny<Currency>())).Returns(new Decimal(1));
            
            Mock.Arrange(() => db.GetExchange(Arg.IsAny<Exchange>())).Returns(exchangeBase);
           
        }

        

        [TestMethod]
        public void TestCreateWithoutVolume()
        {
            Mock.Arrange(() => exchangeBase.ExecuteTradeOrder(Arg.IsAny<TradeOrder>())).DoInstead((TradeOrder o) => o.Id="ID");
            Mock.Arrange(() => exchangeBase.PreventPlacingMakerOrderAtMarketPrice).Returns(true);

            TradeOrder order = new TradeOrder();
            
            watch.Order = order;
            watch.Create();
            Assert.AreEqual("The volume is not specified",order.Error);
        }

        [TestMethod]
        public void TestCreateWithoutPrice()
        {           
            Mock.Arrange(() => exchangeBase.ExecuteTradeOrder(Arg.IsAny<TradeOrder>())).DoInstead((TradeOrder o) => o.Id="ID");
            Mock.Arrange(() => exchangeBase.PreventPlacingMakerOrderAtMarketPrice).Returns(true);

            TradeOrder order = new TradeOrder();
            order.Volume = 100;
            watch.Order = order;
            watch.Create();
            Assert.AreEqual("The price is not specified",order.Error);
        }

        [TestMethod]
        public void TestCreateOrderPriceGreaterThanLastTradePrice()
        {
            Mock.Arrange(() => exchangeBase.ExecuteTradeOrder(Arg.IsAny<TradeOrder>())).DoInstead((TradeOrder o) => o.Id="ID");
            Mock.Arrange(() => exchangeBase.PreventPlacingMakerOrderAtMarketPrice).Returns(true);
            MockLastTrade(12.1);
            TradeOrder order = new TradeOrder();
            order.Volume = 100;
            order.Price = 25;
            order.Pair = new CurrencyPair(Exchange.Bitfinex, Currency.BTC, Currency.USD);
            watch.Order = order;
            watch.Create();
            Assert.IsNull(order.Error);
        }

        [TestMethod]
        public void TestCreateOrderPriceSmallerThanLastTradePrice()
        {
            
            watch.PairMaker = new CurrencyPair(Exchange.Bitfinex, Currency.BTC, Currency.USD);
            watch.PairTaker = new CurrencyPair(Exchange.BTCChina, Currency.BTC, Currency.CNY);

            Mock.Arrange(() => exchangeBase.ExecuteTradeOrder(Arg.IsAny<TradeOrder>())).DoInstead((TradeOrder o) => o.Id="ID");
            Mock.Arrange(() => exchangeBase.PreventPlacingMakerOrderAtMarketPrice).Returns(true);

            MockLastTrade(12.1);
            TradeOrder order = new TradeOrder();
            order.Volume = 100;
            order.Price = 10;
            order.Pair = new CurrencyPair(Exchange.Bitfinex, Currency.BTC, Currency.USD);
            watch.Order = order;
            watch.Create();
            Assert.IsNull(order.Error);
            Assert.AreEqual(new Decimal(12.11), order.Price);
            Assert.AreEqual(-106.7981818, (double)watch.Delta,0.0001);
                                        
        }

        [TestMethod]
        public void TestCancelActiveOrder()
        {
            Mock.Arrange(() => exchangeBase.TradeOrderCancel(Arg.IsAny<TradeOrder>(), Arg.AnyString)).DoInstead((TradeOrder o)=>o.IsCancelling = false);
           
            TradeOrder order = Mock.Create<TradeOrder>(Behavior.CallOriginal);
            order.Volume = 100;
            order.Price = 10;
            order.Pair = new CurrencyPair(Exchange.Bitfinex, Currency.BTC, Currency.USD);
            order.Active = true;
            
            watch.Order = order;
            TradeWatch.Trades.Add(watch);
            watch.Cancel("reason");
            Assert.IsNull(order.Error);
        }

        [TestMethod]
        public void TestCancelExecutedOrder()
        {
            Mock.Arrange(() => exchangeBase.TradeOrderCancel(Arg.IsAny<TradeOrder>(), Arg.AnyString)).DoInstead((TradeOrder o)=>o.IsCancelling = false);
            
            TradeOrder order = Mock.Create<TradeOrder>(Behavior.CallOriginal);
            Mock.Arrange(() => order.Status).Returns(TradeOrderStatus.Executed);
            order.Volume = 100;
            order.Price = 10;
            order.Pair = new CurrencyPair(Exchange.Bitfinex, Currency.BTC, Currency.USD);
            order.Active = true;
            
            watch.Order = order;
            TradeWatch.Trades.Add(watch);
            watch.Cancel("reason");
            Assert.IsNull(order.Error);
        }


        private void MockLastTrade(double price)
        {
            Trade trade = new Trade();
            trade.Price = new decimal(price);
            Mock.Arrange(() => db.LastPriceGet(Arg.IsAny<CurrencyPair>())).Returns(trade);
        }

        private static void MockWebSocketServer()
        {
            Mock.SetupStatic(typeof(WebSocketServer));
            WebSocketServer server = Mock.Create<WebSocketServer>(Behavior.Strict);
            Mock.Arrange(() => WebSocketServer.Current).Returns(server);
            Mock.Arrange(() => server.Broadcast(Arg.IsAny<TradeWatch>(), Arg.IsAny<Subscription>())).DoNothing();
        }
     
        private static void MockTradeLog()
        {
            Mock.SetupStatic(typeof(TradeLog));
            Mock.Arrange(() => TradeLog.Log(Arg.AnyString)).DoNothing();
        }
    }
}
