using System;
using Arbitrage.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arbitrage
{
    [TestClass]
    public class TradeWatchTest
    {
        private SqlHelper setup = new SqlHelper();
        private TestObjectFactory factory = new TestObjectFactory();

        [TestMethod]
        public void TestCreate()
        {
            TradeWatch watch = new TradeWatch();
            watch.Id = new Guid();
            TradeOrder order = factory.CreateTradeOrder("1");
            watch.Order = order;
            watch.Create();
        }

        [TestMethod]
        public void TestExecuteTakerOrderIfNeeded()
        {
            TradeWatch watch = new TradeWatch();
            TradeOrder order = factory.CreateTradeOrder("1");
            watch.Order = order;
            watch.Id = new Guid();
            watch.ExecuteTakerOrderIfNeeded();
        }
    }
}
