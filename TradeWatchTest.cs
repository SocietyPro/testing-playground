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

        [ClassInitialize]
        public static void init()
        {
            Mock.SetupStatic(typeof(WebSocketServer));
            WebSocketServer server = Mock.Create<WebSocketServer>(Behavior.Strict);
            Mock.Arrange(() => WebSocketServer.Current).Returns(server);
            Mock.Arrange(() => server.Broadcast(Arg.IsAny<TradeWatch>(), Arg.IsAny<Subscription>())).DoNothing();

            Mock.SetupStatic(typeof(Database));
        }

        [TestInitialize]
        public void setup()
        {
            Mock.SetupStatic(typeof(TradeWatch));
            watch = Mock.Create<TradeWatch>(Behavior.CallOriginal);
            watch.Id = new Guid();
            
            db = Mock.Create<Database>(Behavior.Strict);

            

            ExchangeRate exchangeRate = Mock.Create<ExchangeRate>();
            Dictionary<Currency, decimal> rates = new Dictionary<Currency, decimal>();
            rates.Add(Currency.USD,new decimal(1.1));
            rates.Add(Currency.CNY,new decimal(10.8));
            exchangeRate.Rates = rates;
            db.ExchangeRate = exchangeRate;

            ExchangeBase exchangeBase = Mock.Create<ExchangeBase>(Behavior.Strict);
            Mock.Arrange(() => exchangeBase.ExecuteTradeOrder(Arg.IsAny<TradeOrder>())).DoInstead((TradeOrder order) => order.Id="ID");
            Mock.Arrange(() => exchangeBase.PreventPlacingMakerOrderAtMarketPrice).Returns(true);
            Mock.Arrange(() => exchangeRate.GetRate(Arg.IsAny<Currency>())).Returns(new Decimal(1));
            
            Mock.Arrange(() => db.GetExchange(Arg.IsAny<Exchange>())).Returns(exchangeBase);
            Mock.Arrange(() => Database.Current).Returns(db);

            Mock.SetupStatic(typeof(TradeLog));
            Mock.Arrange(() => TradeLog.Log(Arg.AnyString)).DoNothing();

            Mock.Arrange(() => TradeWatch.SaveToDisk()).DoNothing();

            Mock.SetupStatic(typeof(Debug));
            Mock.Arrange(() => Debug.Assert(Arg.Matches<bool>(x => x == false))).Throws(new Exception("Debug Assertion"));

        }

        

        [TestMethod]
        public void TestCreateWithoutVolume()
        {
            
            TradeOrder order = new TradeOrder();
            
            watch.Order = order;
            watch.Create();
            Assert.AreEqual("The volume is not specified",order.Error);
        }

        [TestMethod]
        public void TestCreateWithoutPrice()
        {           
            TradeOrder order = new TradeOrder();
            order.Volume = 100;
            watch.Order = order;
            watch.Create();
            Assert.AreEqual("The price is not specified",order.Error);
        }

        [TestMethod]
        public void TestCreateOrderPriceGreaterThanLastTradePrice()
        {
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

        private void MockLastTrade(double price)
        {
            Trade trade = new Trade();
            trade.Price = new decimal(price);
            Mock.Arrange(() => db.LastPriceGet(Arg.IsAny<CurrencyPair>())).Returns(trade);
        }
        
    }
}
