using System;
using Arbitrage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arbitrage
{
    [TestClass]
    public class PriceAmountTest
    {
        [TestMethod]
        public void TestStandardPrice()
        {
            PriceAmount amount = new PriceAmount();
            amount.Price = new decimal(12.0);
            amount.Amount = new decimal(100.00);
            Assert.AreEqual(new Decimal(1200.00), amount.Total);
            Assert.AreEqual("100.00000 @ 12.00000000 = 1,200.00000",amount.ToString());
            
        }

        [TestMethod]
        public void TestReverse()
        {
            PriceAmount amount = new PriceAmount();
            amount.Price = new decimal(113.24);
            amount.Amount = new decimal(432.57);
            Assert.AreEqual(48984.2268, (Double)amount.Total);
            PriceAmount reversed = amount.Reverse();
            Assert.AreEqual(432.57, (Double)reversed.Total);
        }

    }
}
