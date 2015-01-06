using Arbitrage.Exchanges;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arbitrage
{
    [TestClass]
    public class DatabaseTest
    {
        [TestMethod]
        public void TestGetExchange()
        {
            ExchangeBase exchangeBase = Database.Current.GetExchange(Exchange.BTCChina);
            Assert.IsNotNull(exchangeBase);
        }
    }
}
