using System;
using System.Collections.Generic;
using Arbitrage.Exchanges;
using Arbitrage.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace Arbitrage
{
    [TestClass]
    public class TradeWatchTest
    {
        private TradeWatch watch;

        private static Database db;

        private ExchangeBase exchangeBase;
        

        [ClassInitialize]
        public static void Init()
        {

            Mock.SetupStatic(typeof(TradeWatch));
            WebHelper.MockWebSocketServer();
            Mock.SetupStatic(typeof(Database));
            LogHelper.MockTradeLog();

        }

        

        [TestInitialize]
        public void Setup()
        {
            CreateTradeWatch();

            db = Mock.Create<Database>(Behavior.Strict);
            Mock.Arrange(() => Database.Current).Returns(db);

            ExchangeRate exchangeRate = CreateExchangeRate();
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
            MockLastTrade(12.1m);
            TradeOrder order = CreateTradeOrder(25);
            
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

            MockLastTrade(12.1m);
            TradeOrder order = CreateTradeOrder();
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

            TradeOrder order = CreateTradeOrder();
            
            watch.Order = order;
            TradeWatch.Trades.Add(watch);
            watch.Cancel("reason");
            Assert.IsNull(order.Error);
        }

        [TestMethod]
        public void TestCancelExecutedOrder()
        {
            Mock.Arrange(() => exchangeBase.TradeOrderCancel(Arg.IsAny<TradeOrder>(), Arg.AnyString)).DoInstead((TradeOrder o)=>o.IsCancelling = false);
            
            TradeOrder order = CreateTradeOrder();
            Mock.Arrange(() => order.Status).Returns(TradeOrderStatus.Executed);
            
            watch.Order = order;
            TradeWatch.Trades.Add(watch);
            watch.Cancel("reason");
            Assert.IsNull(order.Error);
        }

        [TestMethod]
        public void TestDeleteExecutedOrder()
        {
            Mock.Arrange(() => exchangeBase.MonitoringTradeOrderRemove(Arg.IsAny<TradeOrder>())).DoNothing();
            
            TradeOrder order = CreateTradeOrder();
            watch.Order = order;
            Mock.Arrange(() => order.Status).Returns(TradeOrderStatus.Executed);
            TradeWatch.Trades.Add(watch);
            List<TradeOrder> orderList = new List<TradeOrder>();
            orderList.Add(order);
            watch.TakerOrders=orderList;
           
            watch.Delete();
            Assert.IsNull(order.Error);
            Assert.IsTrue(watch.IsDelete);
            Assert.IsFalse(TradeWatch.Trades.Contains(watch));
        }

        [TestMethod]
        public void TestImproveActiveOrder()
        {
            TraderSession session = Mock.Create<TraderSession>(Behavior.Strict);
            Mock.Arrange(() => session.SetTradeWatch(Arg.IsAny<TradeWatch>())).DoNothing();
            db.Session = session;
            Mock.Arrange(() => exchangeBase.TradeOrderCancel(Arg.IsAny<TradeOrder>(), Arg.AnyString)).DoInstead((TradeOrder o)=>o.IsCancelling = false);
            
            TradeOrder order = CreateTradeOrder();
            watch.ImproveBy(order,new Decimal(21.2),"test_reason");
            Assert.AreEqual(0, watch.NbTakerOrders);
        }

        [TestMethod]
        public void TestImprovePartiallyExecutedOrder()
        {
            TraderSession session = Mock.Create<TraderSession>(Behavior.Strict);
            Mock.Arrange(() => session.SetTradeWatch(Arg.IsAny<TradeWatch>())).DoNothing();
            db.Session = session;
            Mock.Arrange(() => exchangeBase.TradeOrderCancel(Arg.IsAny<TradeOrder>(), Arg.AnyString)).DoInstead((TradeOrder o)=>o.IsCancelling = false);
            Mock.Arrange(() => exchangeBase.RefreshTradeOrder(Arg.IsAny<TradeOrder>(), Arg.AnyDateTime)).DoNothing();
            Mock.Arrange(() => exchangeBase.ExecuteTradeOrder(Arg.IsAny<TradeOrder>())).DoInstead((TradeOrder o) => o.Active=true);
            Mock.Arrange(() => exchangeBase.MonitoringTradeOrderAdd(Arg.IsAny<TradeOrder>())).DoNothing();
           

            TradeOrder order = CreateTradeOrder();
            Mock.Arrange(() => order.Status).Returns(TradeOrderStatus.PartiallyExecuted);
            Mock.Arrange(() => order.Active).Returns(false);

            List<TradeOrder> orderList = new List<TradeOrder>();
            orderList.Add(order);
            watch.TakerOrders=orderList;
           

            watch.ImproveBy(order,new Decimal(21.2),"test_reason");
            Assert.AreEqual(1, watch.NbTakerOrders);
        }

        [TestMethod]
        public void TestSetOrderToTopOfOpenBook()
        {
            TradeOrder order = CreateTradeOrder();
            watch.Order = order;
            watch.SetOrderToTopOfOpenBook(order);
               
        }

        [TestMethod]
        public void TestExecuteTakerOrderIfNeeded()
        {
            TradeOrder order = CreateTradeOrder();
            watch.Order = order;
            watch.ExecuteTakerOrderIfNeeded();
               
        }

        [TestMethod]
        public void TestCheckForRecreation()
        {
            MockLastTrade(12.5m);
            Mock.Arrange(() => exchangeBase.TradeOrderCancel(Arg.IsAny<TradeOrder>(), Arg.AnyString)).DoInstead((TradeOrder o) => o.IsCancelling = false);
           
            TradeOrder order = CreateTradeOrder();
            Mock.Arrange(() => order.Active).Returns(false);
            Mock.Arrange(() => order.Status).Returns(TradeOrderStatus.Canceled);
            watch.Order = order;
            watch.PairTaker = new CurrencyPair(Exchange.BTCChina, Currency.CNY, Currency.USD);
            watch.PairMaker = new CurrencyPair(Exchange.BTCChina, Currency.USD, Currency.CNY);

            Mock.NonPublic.Arrange(watch, "StopMonitoring").DoNothing();

            TradeWatch.Trades.Add(watch);

            watch.CheckForRecreation();
               
        }

        private void MockLastTrade(decimal price)
        {
            Trade trade = new Trade();
            trade.Price = price;
            Mock.Arrange(() => db.LastPriceGet(Arg.IsAny<CurrencyPair>())).Returns(trade);
        }

        private ExchangeRate CreateExchangeRate()
        {
            ExchangeRate exchangeRate = Mock.Create<ExchangeRate>();
            Dictionary<Currency, decimal> rates = new Dictionary<Currency, decimal>();
            rates.Add(Currency.USD, new decimal(1.1));
            rates.Add(Currency.CNY, new decimal(10.8));
            exchangeRate.Rates = rates;
            return exchangeRate;
        }

        private void CreateTradeWatch()
        {
            watch = Mock.Create<TradeWatch>(Behavior.CallOriginal);
            watch.Id = new Guid();
            Mock.Arrange(() => TradeWatch.SaveToDisk()).DoNothing();
        }
        
        private TradeOrder CreateTradeOrder()
        {
            return CreateTradeOrder(10);
        }

        private TradeOrder CreateTradeOrder(decimal price)
        {
            TradeOrder order = Mock.Create<TradeOrder>(Behavior.CallOriginal);
            Mock.Arrange(() => order.Status).Returns(TradeOrderStatus.Active);
            order.Volume = 100;
            order.Price = price;
            order.Pair = new CurrencyPair(Exchange.Bitfinex, Currency.BTC, Currency.USD);
            order.Active = true;
            return order;
        }
    }
}
