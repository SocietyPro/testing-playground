using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arbitrage
{
    [TestClass]
    public class BalanceTest
    {
        [TestMethod]
        public void TestAmountUnk()
        {
            Balance balance = new Balance();
            Assert.AreEqual(0, balance.Amount);
            balance.Amount = 150;
            Assert.AreEqual(150, balance.Amount);
            Assert.IsTrue(balance.Dirty);
            Assert.AreEqual("BalanceUNK => Total:150 Available: Frozen:0",balance.ToString());
        }

        [TestMethod]
        public void TestAmountFrozenBtc()
        {
            Balance balance = new Balance(Exchange.Bitfinex, Currency.BTC);
            balance.Amount = 150;
            balance.AmountFrozen = 10;
            Assert.AreEqual(150, balance.Amount);
            Assert.AreEqual(140, balance.AmountAvailable);
            Assert.IsTrue(balance.Dirty);
            Assert.AreEqual("BalanceBTC => Total:150 Available: Frozen:10",balance.ToString());
        }

        [TestMethod]
        public void TestAmountAvailable()
        {
            Balance balance = new Balance(Exchange.OKCoin, Currency.EUR);
            balance.Amount = 100;
            balance.AmountFrozen = 10;
            balance.AmountAvailable = 80;
            Assert.AreEqual(100, balance.Amount);
            Assert.AreEqual(80, balance.AmountAvailable);
            Assert.AreEqual(10, balance.AmountFrozen);
            Assert.IsTrue(balance.Dirty);
            Assert.AreEqual("BalanceEUR => Total:100 Available:80 Frozen:10",balance.ToString());
        }
    }
}
