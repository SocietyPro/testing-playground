using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arbitrage
{
    [TestClass]
    public class TradeWatchTest
    {
        [TestMethod]
        [Ignore]
        public void TestCreate()
        {
            TradeWatch watch = new TradeWatch();
            watch.Id = new Guid();
            TradeOrder order = new TradeOrder();
            watch.Order = order;
            watch.Create();
        }
    }
}
