using System;
using BackendTest.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arbitrage
{
    [TestClass]
    public class TradeWatchTest
    {
        private SqlHelper setup = new SqlHelper();

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
